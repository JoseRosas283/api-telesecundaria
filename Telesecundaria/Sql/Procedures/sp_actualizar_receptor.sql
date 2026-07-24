CREATE OR REPLACE PROCEDURE sp_actualizar_receptor(
    p_claveReceptor VARCHAR(18), 
    p_nuevo_correo VARCHAR(100)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_correo_limpio VARCHAR(100);
    v_existe BOOLEAN;
    v_tiene_notificaciones BOOLEAN;
BEGIN
    -- ==========================================
    -- 1. VALIDACIÓN DE EXISTENCIA (PK)
    -- ==========================================
    SELECT EXISTS (
        SELECT 1 FROM "Receptores" WHERE "claveReceptor" = p_claveReceptor
    ) INTO v_existe;

    IF NOT v_existe THEN
        RAISE EXCEPTION 'Error: La clave de receptor "%" no existe.', p_claveReceptor;
    END IF;

    -- ==========================================
    -- 2. CANDADO DE HISTORIAL (NOTIFICACIONES)
    -- ==========================================
    -- Si el receptor ya "nació" en la tabla de Notificaciones, 
    -- el canal de comunicación ya no es editable.
    SELECT EXISTS (
        SELECT 1 FROM "Notificaciones" WHERE "claveReceptor" = p_claveReceptor
    ) INTO v_tiene_notificaciones;

    IF v_tiene_notificaciones THEN
        RAISE EXCEPTION 'Bloqueo de Integridad: No se puede actualizar el receptor porque ya cuenta con notificaciones emitidas en su historial.';
    END IF;

    -- ==========================================
    -- 3. LIMPIEZA Y VALIDACIÓN DE DATOS
    -- ==========================================
    v_correo_limpio := LOWER(TRIM(p_nuevo_correo));

    IF v_correo_limpio IS NULL OR v_correo_limpio = '' THEN
        RAISE EXCEPTION 'Error: El correo de destino no puede estar vacío.';
    END IF;

    -- ==========================================
    -- 4. ACTUALIZACIÓN
    -- ==========================================
    UPDATE "Receptores" 
    SET "correo_destino" = v_correo_limpio
    WHERE "claveReceptor" = p_claveReceptor;

    RAISE NOTICE 'Sincronización exitosa: El destino del receptor % ha sido actualizado.', p_claveReceptor;

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Fallo en la actualización del receptor: %', SQLERRM;
END;
$$;
