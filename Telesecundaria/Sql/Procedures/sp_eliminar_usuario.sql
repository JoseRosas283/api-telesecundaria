CREATE OR REPLACE PROCEDURE sp_eliminar_usuario(
    p_claveUsuario VARCHAR(18)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_nombre_antiguo VARCHAR(50);
    v_claveReceptor_vinculada VARCHAR(18);
BEGIN
    -- 1. VERIFICAR EXISTENCIA Y OBTENER CLAVE DE RECEPTOR
    -- Usamos LEFT JOIN por si el usuario no llegara a tener receptor (caso raro pero posible)
    SELECT u.nombre_usuario, r."claveReceptor"
    INTO v_nombre_antiguo, v_claveReceptor_vinculada
    FROM "Usuarios" u
    LEFT JOIN "Receptores" r ON r."claveEntidad" = u."claveUsuario" AND r."tipoEntidad" = 'Usuario'
    WHERE u."claveUsuario" = p_claveUsuario;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Error: El usuario con clave % no existe.', p_claveUsuario;
    END IF;

    -- ==========================================================
    -- 2. BAJA LÓGICA DE USUARIO (SELLO DE BLOQUEO)
    -- ==========================================================
    UPDATE "Usuarios"
    SET 
        estado = FALSE,
        nombre_usuario = 'BAJA_DEFINITIVA_' || p_claveUsuario
    WHERE "claveUsuario" = p_claveUsuario;

    -- ==========================================================
    -- 3. DESACTIVACIÓN DEL RECEPTOR (SIN ALTERAR EL CORREO)
    -- ==========================================================
    IF v_claveReceptor_vinculada IS NOT NULL THEN
        UPDATE "Receptores"
        SET 
            estado = FALSE 
            -- Mantenemos correo_destino intacto por las validaciones de la tabla
        WHERE "claveReceptor" = v_claveReceptor_vinculada;
        
        RAISE NOTICE 'Sincronización: Receptor % desactivado.', v_claveReceptor_vinculada;
    END IF;

    -- 4. NOTIFICACIÓN DE ÉXITO
    RAISE NOTICE 'Baja Exitosa: El usuario "%" ha sido inhabilitado permanentemente.', v_nombre_antiguo;

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error crítico en la baja definitiva: %', SQLERRM;
END;
$$;
