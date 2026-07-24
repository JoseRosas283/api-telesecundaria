CREATE OR REPLACE PROCEDURE sp_registrar_envio_pendiente(
    p_clave_notificacion VARCHAR(18),
    p_destino VARCHAR(150)
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- 1. VALIDACIÓN DE EXISTENCIA DE LA NOTIFICACIÓN
    IF NOT EXISTS (SELECT 1 FROM "Notificaciones" WHERE "claveNotificacion" = p_clave_notificacion) THEN
        RAISE EXCEPTION 'Error: La notificación % no existe.', p_clave_notificacion;
    END IF;

    IF p_destino IS NULL OR TRIM(p_destino) = '' THEN
        RAISE EXCEPTION 'Error: El destino del envío no puede estar vacío.';
    END IF;

    IF p_destino !~* '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$' THEN
        RAISE EXCEPTION 'Error: El correo "%" no tiene un formato válido.', p_destino;
    END IF;

    -- 4. INSERCIÓN LIMPIA
    -- Los campos estatus ('Pendiente'), reintento_num (0) y claveEnvio (DEFAULT) 
    -- se gestionan automáticamente por la definición de la tabla.
    INSERT INTO "Envios" (
        "claveNotificacion",
        destino
    ) VALUES (
        p_clave_notificacion,
        TRIM(p_destino)
    );

    RAISE NOTICE 'Ticket de envío generado exitosamente para %', p_destino;

EXCEPTION
    WHEN OTHERS THEN
        -- Capturamos el error para que el Orquestador (sp_emitir_notificacion) decida qué hacer
        RAISE EXCEPTION 'Falla en sp_registrar_envio_pendiente: %', SQLERRM;
END;
$$;
