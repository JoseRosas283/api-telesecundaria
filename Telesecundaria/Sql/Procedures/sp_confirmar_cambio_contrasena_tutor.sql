CREATE OR REPLACE PROCEDURE sp_confirmar_cambio_contrasena_tutor(
    p_correo VARCHAR(100),
    p_token_confirmacion VARCHAR(64),
    p_nueva_contrasena VARCHAR(255),
    OUT p_exito BOOLEAN,
    OUT p_mensaje VARCHAR(255)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_correo_formateado VARCHAR(100);
    v_clave_tutor VARCHAR(18);
    v_clave_codigo VARCHAR(18);
BEGIN
    p_exito := FALSE;
    v_correo_formateado := LOWER(TRIM(p_correo));

    SELECT "claveTutorAspirante" INTO v_clave_tutor
    FROM "TutorAspirante"
    WHERE LOWER(correo) = v_correo_formateado
      AND estado = TRUE;

    IF v_clave_tutor IS NULL THEN
        p_mensaje := 'Token inválido o expirado.';
        RETURN;
    END IF;

    SELECT "claveCodigoRecuperacion" INTO v_clave_codigo
    FROM "CodigosRecuperacionTutor"
    WHERE "claveTutorAspirante" = v_clave_tutor
      AND token_confirmacion = p_token_confirmacion
      AND token_usado = FALSE
      AND token_expiracion > CURRENT_TIMESTAMP
    ORDER BY fecha_creacion DESC
    LIMIT 1;

    IF v_clave_codigo IS NULL THEN
        p_mensaje := 'Token inválido o expirado.';
        RETURN;
    END IF;

    UPDATE "CodigosRecuperacionTutor"
    SET token_usado = TRUE
    WHERE "claveCodigoRecuperacion" = v_clave_codigo;

    UPDATE "TutorAspirante"
    SET contrasena = p_nueva_contrasena
    WHERE "claveTutorAspirante" = v_clave_tutor;

    p_exito := TRUE;
    p_mensaje := 'Contraseña actualizada correctamente.';
END;
$$;