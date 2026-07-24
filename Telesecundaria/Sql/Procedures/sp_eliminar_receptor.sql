CREATE OR REPLACE PROCEDURE sp_eliminar_receptor(
    p_claveReceptor VARCHAR(18)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_existe_receptor BOOLEAN;
    v_tiene_notificaciones BOOLEAN;
BEGIN
    -- ==========================================
    -- 1. VALIDACIÓN DE EXISTENCIA
    -- ==========================================
    SELECT EXISTS (
        SELECT 1 FROM "Receptores" WHERE "claveReceptor" = p_claveReceptor
    ) INTO v_existe_receptor;

    IF NOT v_existe_receptor THEN
        RAISE EXCEPTION 'Error: El receptor con clave "%" no existe.', p_claveReceptor;
    END IF;

    -- ==========================================
    -- 2. VERIFICACIÓN DE HISTORIAL (NOTIFICACIONES)
    -- ==========================================
    SELECT EXISTS (
        SELECT 1 FROM "Notificaciones" WHERE "claveReceptor" = p_claveReceptor
    ) INTO v_tiene_notificaciones;

    -- ==========================================
    -- 3. EJECUCIÓN DE ELIMINACIÓN O DESACTIVACIÓN
    -- ==========================================
    IF v_tiene_notificaciones THEN
        -- Borrado Lógico: Mantenemos el registro por auditoría pero lo desactivamos
        UPDATE "Receptores" 
        SET estado = FALSE 
        WHERE "claveReceptor" = p_claveReceptor;
        
        RAISE NOTICE 'Aviso: El receptor tiene notificaciones. Se ha desactivado (borrado lógico) para preservar el historial.';
    ELSE
        -- Borrado Físico: No hay rastro del receptor en el sistema, se puede eliminar
        DELETE FROM "Receptores" 
        WHERE "claveReceptor" = p_claveReceptor;
        
        RAISE NOTICE 'Éxito: El receptor ha sido eliminado físicamente del sistema.';
    END IF;

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error al procesar la eliminación del receptor: %', SQLERRM;
END;
$$;
