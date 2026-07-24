CREATE OR REPLACE PROCEDURE sp_insertar_convocatoria(
    p_titulo VARCHAR(150),
    p_subtitulo VARCHAR(200),
    p_descripcion TEXT,
    p_fecha_inicio_txt VARCHAR(20), 
    p_fecha_fin_txt VARCHAR(20),    
    p_ciclo_escolar VARCHAR(20),
    p_cupo_maximo INTEGER,
    p_nombreUsuario VARCHAR(100), 
    p_claveImagen VARCHAR(18),
    p_destacado_txt VARCHAR(20) -- Cambiamos BOOLEAN por VARCHAR para recibir el texto
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_activacion BOOLEAN;
    v_estado VARCHAR(15);
    v_visible_pub BOOLEAN; 
    v_f_inicio TIMESTAMP;
    v_f_fin TIMESTAMP;
    v_anio_actual INTEGER;
    v_nueva_clave_conv VARCHAR(18);
    v_existe_activa BOOLEAN;
    v_destacado_bool BOOLEAN; -- Nueva variable para la conversión
BEGIN
    -- ==========================================================
    -- 0. CONVERSIÓN DE TEXTO A BOOLEAN (DESTACADO)
    -- ==========================================================
    IF p_destacado_txt = 'Es destacado' THEN
        v_destacado_bool := TRUE;
    ELSIF p_destacado_txt = 'No es destacado' THEN
        v_destacado_bool := FALSE;
    ELSE
        RAISE EXCEPTION 'Error: El valor de destacado debe ser "Es destacado" o "No es destacado".';
    END IF;

    -- ==========================================================
    -- 1. CANDADO DE SEGURIDAD (Exclusividad Reforzada)
    -- ==========================================================
    SELECT EXISTS (
        SELECT 1 FROM "Convocatorias" 
        WHERE estado IN ('Publicada', 'Programada') OR activacion = TRUE
    ) INTO v_existe_activa;

    IF v_existe_activa THEN
        RAISE EXCEPTION 'Operación Bloqueada: Ya existe una convocatoria activa o programada.';
    END IF;

    -- 2. CONVERSIÓN DE FECHAS
    BEGIN
        v_f_inicio := to_timestamp(TRIM(p_fecha_inicio_txt) || ' 00:00:00', 'DD/MM/YYYY HH24:MI:SS');
        v_f_fin := to_timestamp(TRIM(p_fecha_fin_txt) || ' 23:59:59', 'DD/MM/YYYY HH24:MI:SS');
    EXCEPTION WHEN OTHERS THEN
        RAISE EXCEPTION 'Error de formato: Use DD/MM/YYYY.';
    END;

    -- ==========================================================
    -- 3. LÓGICA DE PROGRAMACIÓN DINÁMICA
    -- ==========================================================
    IF v_f_inicio::DATE = CURRENT_DATE THEN
        v_activacion := TRUE;
        v_estado := 'Publicada';
        v_visible_pub := TRUE;
    ELSIF v_f_inicio::DATE > CURRENT_DATE THEN
        v_activacion := FALSE;
        v_estado := 'Programada';
        v_visible_pub := FALSE; 
    ELSE
        RAISE EXCEPTION 'Error: La fecha de inicio (%) no puede ser anterior a hoy (%).', 
                        p_fecha_inicio_txt, CURRENT_DATE;
    END IF;

    -- ==========================================================
    -- 4. VALIDACIONES DE AÑO NATURAL Y VIGENCIA
    -- ==========================================================
    v_anio_actual := EXTRACT(YEAR FROM CURRENT_DATE);

    IF EXTRACT(YEAR FROM v_f_inicio) > v_anio_actual THEN
        RAISE EXCEPTION 'Error: No se permiten convocatorias para el año %. Solo año en curso (%).', 
                        EXTRACT(YEAR FROM v_f_inicio), v_anio_actual;
    END IF;

    IF v_f_fin <= v_f_inicio THEN
        RAISE EXCEPTION 'La fecha de cierre debe ser posterior a la de apertura.';
    END IF;

    IF EXTRACT(YEAR FROM v_f_fin) > v_anio_actual THEN
        RAISE EXCEPTION 'Error: La convocatoria debe finalizar dentro del año actual (%).', v_anio_actual;
    END IF;

    -- 5. INSERCIÓN EN CONVOCATORIAS
    INSERT INTO "Convocatorias" (
        titulo, descripcion, fecha_inicio, fecha_fin, ciclo_escolar,
        cupo_maximo, cupo_disponible, activacion, estado
    ) VALUES (
        p_titulo, p_descripcion, v_f_inicio, v_f_fin, p_ciclo_escolar,
        p_cupo_maximo, p_cupo_maximo, v_activacion, v_estado
    ) RETURNING "claveConvocatoria" INTO v_nueva_clave_conv;

    -- ==========================================================
    -- 6. LLAMADA AL PROCEDIMIENTO DE PUBLICACIONES
    -- ==========================================================
    BEGIN
        CALL sp_insertar_publicacion(
            p_titulo,               -- p_titulo
            p_subtitulo,            -- p_subtitulo
            p_descripcion,          -- p_cuerpo
            'Convocatorias',        -- p_categoria
            p_nombreUsuario,        -- p_nombreUsuario
            p_claveImagen,          -- p_imgPrincipal (Slot 1)
            NULL,                   -- p_imgSecundaria (Slot 2)
            NULL,                   -- p_imgTercera (Slot 3)
            v_nueva_clave_conv,     -- p_claveConvocatoria
            v_destacado_bool,       -- MANDAMOS EL BOOLEAN CONVERTIDO
            v_visible_pub           -- p_estatus_visible
        );
    EXCEPTION WHEN OTHERS THEN
        RAISE EXCEPTION 'Fallo en la Publicación Web: %. Convocatoria cancelada.', SQLERRM;
    END;

    RAISE NOTICE 'Éxito: Convocatoria % registrada como %. (Destacado: %)', 
                 v_nueva_clave_conv, v_estado, p_destacado_txt;
END;
$$;
