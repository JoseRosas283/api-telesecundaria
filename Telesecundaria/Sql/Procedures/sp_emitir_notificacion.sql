CREATE OR REPLACE PROCEDURE sp_emitir_notificacion(
    p_clave_receptor VARCHAR(18),
    p_titulo VARCHAR(80),
    p_mensaje TEXT,
    p_prioridad SMALLINT,
    p_nombre_proceso VARCHAR(50), -- Cambiado: Recibimos el nombre del proceso (Enum)
    p_datos_json JSONB DEFAULT '{}'
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_clave_notif_nueva VARCHAR(18);
    v_receptor_info RECORD;
    v_clave_tipo_notif VARCHAR(18); -- Variable para la traducción interna
BEGIN
    -- 1. VALIDACIÓN DE CAMPOS DE TEXTO
    IF p_titulo IS NULL OR TRIM(p_titulo) = '' THEN
        RAISE EXCEPTION 'Error: El título de la notificación no puede estar vacío.';
    END IF;

    IF p_mensaje IS NULL OR TRIM(p_mensaje) = '' THEN
        RAISE EXCEPTION 'Error: El cuerpo del mensaje no puede estar vacío.';
    END IF;

    -- 2. VALIDACIÓN DE PRIORIDAD (1 a 3)
    IF p_prioridad <= 0 OR p_prioridad > 3 THEN
        RAISE EXCEPTION 'Error: Prioridad no válida (%). Debe ser 1, 2 o 3.', p_prioridad;
    END IF;

    -- 3. VALIDACIÓN Y OBTENCIÓN DE DATOS DEL RECEPTOR
    SELECT tipo_receptor, correo_destino, estado 
    INTO v_receptor_info
    FROM "Receptores" 
    WHERE "claveReceptor" = p_clave_receptor;

    IF v_receptor_info IS NULL OR v_receptor_info.estado = FALSE THEN
        RAISE NOTICE 'Aviso: El receptor % está desactivado o no existe. Notificación descartada.', p_clave_receptor;
        RETURN;
    END IF;

    -- ============================================================
    -- 4. TRADUCCIÓN DE NOMBRE DE PROCESO A CLAVE (NUEVO)
    -- ============================================================
    SELECT "claveTipoNotificacion" INTO v_clave_tipo_notif
    FROM "TipoNotificaciones"
    WHERE nombre_proceso = p_nombre_proceso; -- Buscamos por tu lista sellada

    IF v_clave_tipo_notif IS NULL THEN
        RAISE EXCEPTION 'Error: El tipo de notificación por proceso "%" no existe.', p_nombre_proceso;
    END IF;

    -- ============================================================
    -- 5. VALIDACIÓN: CRUCE CON EL CATÁLOGO DestinoNotificacion
    -- ============================================================
    IF NOT EXISTS (
        SELECT 1 
        FROM "DestinoNotificacion" 
        WHERE "claveTipoNotificacion" = v_clave_tipo_notif 
          AND tipo_receptor = v_receptor_info.tipo_receptor
    ) THEN
        RAISE EXCEPTION 'Error de integridad: El receptor de tipo % no está autorizado para el proceso "%" según el catálogo de destinos.', 
                        v_receptor_info.tipo_receptor, p_nombre_proceso;
    END IF;

    -- 6. INSERCIÓN DE LA NOTIFICACIÓN (Sistema Interno)
    INSERT INTO "Notificaciones" (
        titulo,
        mensaje,
        prioridad,
        datos,
        "claveTipoNotificacion",
        "claveReceptor"
    ) VALUES (
        TRIM(p_titulo),
        TRIM(p_mensaje),
        p_prioridad,
        p_datos_json,
        v_clave_tipo_notif, -- Usamos la clave encontrada internamente
        p_clave_receptor
    ) RETURNING "claveNotificacion" INTO v_clave_notif_nueva;

    -- 7. DECISIÓN DE ENVÍO EXTERNO (La Cadena)
    IF v_receptor_info.tipo_receptor IN ('Tutor', 'TutorAspirante') THEN
        CALL sp_registrar_envio_pendiente(v_clave_notif_nueva, v_receptor_info.correo_destino);
        RAISE NOTICE 'Notificación % registrada y enviada a cola de correo.', v_clave_notif_nueva;
    ELSE
        RAISE NOTICE 'Notificación % registrada solo para sistema (Usuario Administrativo).', v_clave_notif_nueva;
    END IF;

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Falla en cadena de notificación: %', SQLERRM;
END;
$$;