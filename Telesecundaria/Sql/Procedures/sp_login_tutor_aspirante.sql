CREATE OR REPLACE PROCEDURE sp_login_tutor_aspirante(
    p_correo VARCHAR(100),
    p_contrasena VARCHAR(255),
    p_ip_origen VARCHAR(45),
    p_dispositivo_origen TEXT,
    OUT p_exito BOOLEAN,
    OUT p_mensaje VARCHAR,
    OUT p_clave_token VARCHAR(20),
    OUT p_token_original TEXT,
    OUT p_nombre_tutor VARCHAR(150)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_clave_tutor VARCHAR(18);
    v_estado_tutor BOOLEAN;
    v_regex_correo TEXT := '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$';
BEGIN
    -- 1. LIMPIEZA (El correo siempre en minúsculas para evitar duplicados)
    p_correo := LOWER(TRIM(p_correo));

    -- 2. VALIDACIONES INICIALES
    IF p_correo !~ v_regex_correo THEN
        p_exito := FALSE;
        p_mensaje := 'El formato del correo electrónico es inválido.';
        RETURN;
    END IF;

    -- 3. VERIFICACIÓN (Usamos alias "t" para evitar ambigüedad con parámetros)
    SELECT t."claveTutorAspirante", 
           TRIM(CONCAT(t.nombre, ' ', t.apellido_paterno, ' ', COALESCE(t.apellido_materno, ''))), 
           t.estado
    INTO v_clave_tutor, p_nombre_tutor, v_estado_tutor
    FROM "TutorAspirante" t
    WHERE t.correo = p_correo 
      AND t.contrasena = p_contrasena; -- Asumiendo texto plano por ahora

    -- 4. VALIDACIÓN DE RESULTADOS
    IF v_clave_tutor IS NULL THEN
        p_exito := FALSE;
        p_mensaje := 'Credenciales incorrectas: Verifique sus datos.';
        p_nombre_tutor := NULL; -- Limpiamos por seguridad
        RETURN;
    END IF;

    IF v_estado_tutor IS FALSE THEN
        p_exito := FALSE;
        p_mensaje := 'Acceso denegado: Esta cuenta se encuentra inactiva.';
        RETURN;
    END IF;

    -- 5. CREACIÓN DE LA SESIÓN
    INSERT INTO "TokenConvocatorias" (
        token_original,
        "claveTutorAspirante",
        fecha_expiracion,
        ip_origen,
        dispositivo_origen,
        estado_sesion
    ) VALUES (
        encode(gen_random_bytes(32), 'hex'), 
        v_clave_tutor,
        CURRENT_TIMESTAMP + INTERVAL '4 hours',
        COALESCE(p_ip_origen, '0.0.0.0'),
        COALESCE(p_dispositivo_origen, 'Desconocido'),
        TRUE
    )
    RETURNING "claveTokenConvocatoria", token_original 
    INTO p_clave_token, p_token_original;

    -- 6. RESPUESTA
    p_exito := TRUE;
    p_mensaje := 'Autenticación exitosa.';

EXCEPTION
    WHEN OTHERS THEN
        p_exito := FALSE;
        p_mensaje := 'Error crítico en el login: ' || SQLERRM;
END;
$$;