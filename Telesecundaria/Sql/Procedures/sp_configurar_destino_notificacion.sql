CREATE OR REPLACE PROCEDURE sp_configurar_destino_notificacion(
    p_nombre_proceso VARCHAR(50), 
    p_tipo_receptor  VARCHAR(80)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_clave_tipo_notif VARCHAR(18);
    v_existe_destino BOOLEAN;
BEGIN
    -- 1. VALIDACIÓN ENCAPSULADA DEL CHECK
    -- Validamos antes de insertar para dar un mensaje amigable
    IF p_tipo_receptor NOT IN ('TutorAspirante', 'Tutor', 'Usuario') THEN
        RAISE EXCEPTION 'Validación: El receptor "%" no es válido. Solo se permite: TutorAspirante, Tutor o Usuario.', p_tipo_receptor;
    END IF;

    -- 2. TRADUCCIÓN DE NOMBRE A CLAVE
    SELECT "claveTipoNotificacion" INTO v_clave_tipo_notif
    FROM "TipoNotificaciones"
    WHERE nombre_proceso = p_nombre_proceso;

    IF v_clave_tipo_notif IS NULL THEN
        RAISE EXCEPTION 'Validación: El proceso "%" no existe en el catálogo de notificaciones.', p_nombre_proceso;
    END IF;

    -- 3. VALIDACIÓN ENCAPSULADA DE UNICIDAD (Evitar repetidos)
    -- Buscamos si ya existe la combinación Proceso + Receptor
    SELECT EXISTS (
        SELECT 1 FROM "DestinoNotificacion" 
        WHERE "claveTipoNotificacion" = v_clave_tipo_notif 
          AND tipo_receptor = p_tipo_receptor
    ) INTO v_existe_destino;

    IF v_existe_destino THEN
        RAISE EXCEPTION 'Validación: El tipo de receptor "%" ya está configurado para el proceso "%". No se admiten duplicados.', 
                        p_tipo_receptor, p_nombre_proceso;
    END IF;

    -- 4. INSERCIÓN LIMPIA
    -- Si llegó aquí, pasó todos los filtros de seguridad
    INSERT INTO "DestinoNotificacion" ("claveTipoNotificacion", tipo_receptor)
    VALUES (v_clave_tipo_notif, p_tipo_receptor);

    RAISE NOTICE 'Éxito: Destino configurado correctamente para el proceso %.', p_nombre_proceso;

END;
$$;
