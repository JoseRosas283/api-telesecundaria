CREATE OR REPLACE PROCEDURE sp_actualizar_convocatoria(
    p_claveConvocatoria VARCHAR(18),
    p_nombreUsuario VARCHAR(100), 
    p_titulo VARCHAR(150),
    p_subtitulo VARCHAR(200),
    p_descripcion TEXT,
    p_cupo_maximo INTEGER,
    p_claveImagen VARCHAR(18),
    p_destacado_txt VARCHAR(20) 
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_estado_actual VARCHAR(15);
    v_clavePublicacion_Activa VARCHAR(18);
    v_destacado_bool BOOLEAN;
    v_cupo_anterior INTEGER;
    v_cupo_disponible_actual INTEGER;
    v_alumnos_inscritos INTEGER;
BEGIN
    -- 1. CONVERSIÓN DE DESTACADO
    v_destacado_bool := (p_destacado_txt = 'Es destacado');

    -- 2. CAPTURA DE VALORES DE LA TUPLA ANTERIOR
    SELECT estado, "cupo_maximo", "cupo_disponible" 
    INTO v_estado_actual, v_cupo_anterior, v_cupo_disponible_actual
    FROM "Convocatorias" WHERE "claveConvocatoria" = p_claveConvocatoria;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Error: La convocatoria % no existe.', p_claveConvocatoria;
    END IF;

    -- CANDADO: Solo se editan convocatorias vivas
    IF v_estado_actual = 'Cerrada' THEN
        RAISE EXCEPTION 'Bloqueo: No se puede editar una convocatoria que ya está Cerrada.';
    END IF;

    -- 3. CÁLCULO DE INSCRITOS (Tupla Anterior)
    -- Si cupo_maximo = 100 y cupo_disponible = 100 -> 0 inscritos.
    -- Si cupo_maximo = 100 y cupo_disponible = 80  -> 20 inscritos.
    v_alumnos_inscritos := v_cupo_anterior - v_cupo_disponible_actual;

    -- 4. VALIDACIÓN DE CUPO (Regla: Mínimo 1 lugar extra)
    -- Usamos <= para que si intenta igualar a los inscritos, lo rebote.
    IF p_cupo_maximo <= v_alumnos_inscritos THEN
        RAISE EXCEPTION 'Error: El cupo (%) debe ser mayor a los inscritos actuales (%) para no cerrar la convocatoria.', 
                        p_cupo_maximo, v_alumnos_inscritos;
    END IF;

    -- Seguridad extra por si no hay inscritos y mandan 0 o negativo
    IF p_cupo_maximo <= 0 THEN
        RAISE EXCEPTION 'Error: El cupo máximo debe ser al menos 1.';
    END IF;

    -- 5. ACTUALIZACIÓN DINÁMICA
    UPDATE "Convocatorias" SET
        titulo = TRIM(p_titulo),
        descripcion = TRIM(p_descripcion),
        "cupo_maximo" = p_cupo_maximo,
        -- Si v_alumnos_inscritos es 0, se iguala al máximo automáticamente.
        -- Si v_alumnos_inscritos > 0, se resta del nuevo máximo.
        "cupo_disponible" = p_cupo_maximo - v_alumnos_inscritos
    WHERE "claveConvocatoria" = p_claveConvocatoria;

    -- 6. SINCRONIZACIÓN WEB (RELEVO DE VERSIÓN)
    SELECT "clavePublicacion" INTO v_clavePublicacion_Activa
    FROM "Publicaciones" 
    WHERE "claveConvocatoria" = p_claveConvocatoria AND estatus_visible = TRUE;

    IF v_clavePublicacion_Activa IS NOT NULL THEN
        CALL sp_actualizar_publicacion(
            v_clavePublicacion_Activa, 
            p_nombreUsuario,
            p_titulo,
            p_subtitulo,
            p_descripcion,
            p_claveImagen,
            NULL, 
            NULL, 
            v_destacado_bool
        );
    END IF;

    RAISE NOTICE 'Éxito: Convocatoria % actualizada. Cupo: % (Disponibles: %)', 
                 p_claveConvocatoria, p_cupo_maximo, (p_cupo_maximo - v_alumnos_inscritos);
END;
$$;
