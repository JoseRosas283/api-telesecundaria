CREATE OR REPLACE PROCEDURE sp_eliminar_convocatoria(
    p_claveConvocatoria VARCHAR(18),
    p_nombreUsuario VARCHAR(100)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_estado_actual VARCHAR(15);
    v_clavePublicacion_Activa VARCHAR(18);
BEGIN
    -- 1. IDENTIFICACIÓN DE LA CONVOCATORIA
    SELECT estado INTO v_estado_actual
    FROM "Convocatorias" 
    WHERE "claveConvocatoria" = p_claveConvocatoria;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Error: La convocatoria % no existe.', p_claveConvocatoria;
    END IF;

    -- 2. CANDADO ADMINISTRATIVO BÁSICO
    IF v_estado_actual = 'Cerrada' THEN
        RAISE NOTICE 'Aviso: La convocatoria % ya está cerrada.', p_claveConvocatoria;
        RETURN; -- No hay nada que hacer
    END IF;

    -- 3. SINCRONIZACIÓN WEB Y CANDADOS DE INTEGRIDAD
    -- Buscamos la publicación visible
    SELECT "clavePublicacion" INTO v_clavePublicacion_Activa
    FROM "Publicaciones" 
    WHERE "claveConvocatoria" = p_claveConvocatoria 
      AND estatus_visible = TRUE;

   
    IF v_clavePublicacion_Activa IS NOT NULL THEN
        CALL sp_eliminar_publicacion(v_clavePublicacion_Activa, p_nombreUsuario);
    END IF;

    -- 4. ACTUALIZACIÓN FINAL
    -- Si el CALL anterior falló por los aspirantes, esta línea NUNCA se ejecutará
    UPDATE "Convocatorias" SET 
        estado = 'Cerrada',
		activacion = FALSE
    WHERE "claveConvocatoria" = p_claveConvocatoria;

    RAISE NOTICE 'Éxito: Proceso de clausura completado para la convocatoria %.', p_claveConvocatoria;

END;
$$;