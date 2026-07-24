CREATE OR REPLACE PROCEDURE sp_iniciar_sesion(
    p_nombre_usuario VARCHAR,    
    p_contrasena_input VARCHAR,  
    p_direccion_ip VARCHAR,      
    p_agente_usuario TEXT,       
    OUT p_exito BOOLEAN,         
    OUT p_mensaje VARCHAR,       
    OUT p_identificador VARCHAR  
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_clave_db VARCHAR(18);
    v_contrasena_db VARCHAR(255);
    v_estado_db BOOLEAN;
    v_estatus_log VARCHAR(25);
BEGIN
    -- 1. Limpieza
    p_nombre_usuario := TRIM(COALESCE(p_nombre_usuario, ''));
    p_contrasena_input := TRIM(COALESCE(p_contrasena_input, ''));

    -- 2. Validación de campos obligatorios
    IF p_nombre_usuario = '' OR p_contrasena_input = '' THEN
        v_estatus_log := 'Usuario Inexistente';
        p_exito := FALSE;
        p_mensaje := 'Credenciales no válidas.';
        v_clave_db := NULL; 
    ELSE
        -- 3. Búsqueda de usuario
        SELECT "claveUsuario", contrasenia, estado 
        INTO v_clave_db, v_contrasena_db, v_estado_db
        FROM "Usuarios" 
        WHERE nombre_usuario = p_nombre_usuario;

        -- 4. Validación de lógica
        IF v_clave_db IS NULL THEN
            v_estatus_log := 'Usuario Inexistente';
            p_exito := FALSE;
            p_mensaje := 'Credenciales no válidas.';
        ELSIF v_contrasena_db <> p_contrasena_input THEN
            v_estatus_log := 'Contraseña Incorrecta';
            p_exito := FALSE;
            p_mensaje := 'Credenciales no válidas.';
        ELSIF v_estado_db = FALSE THEN
            v_estatus_log := 'Usuario Suspendido';
            p_exito := FALSE;
            p_mensaje := 'Su cuenta ha sido desactivada. Contacte a soporte.';
        ELSE
            -- EXITO
            v_estatus_log := 'Exitoso';
            p_exito := TRUE;
            p_mensaje := 'Acceso correcto.';
            p_identificador := v_clave_db;
        END IF;
    END IF;

    -- 5. REGISTRO DE AUDITORÍA
    -- Este bloque está fuera de las validaciones para que SIEMPRE se intente guardar
    INSERT INTO "Logueos" (
        "claveUsuario", 
        estatus_intento, 
        direccion_ip, 
        agente_usuario,
        fecha_cierre
    ) VALUES (
        v_clave_db, 
        v_estatus_log, 
        p_direccion_ip, 
        p_agente_usuario,
        NULL
    );

EXCEPTION
    WHEN OTHERS THEN
        p_exito := FALSE;
        -- Cambiamos el mensaje para que te diga el error real (ej. error de FK o Constraint)
        p_mensaje := 'Error técnico: ' || SQLERRM;
END;
$$;