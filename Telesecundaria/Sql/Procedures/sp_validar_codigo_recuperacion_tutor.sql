CREATE OR REPLACE PROCEDURE sp_validar_codigo_recuperacion_tutor(
    p_correo VARCHAR(100),
    p_codigo VARCHAR(6),
    OUT p_exito BOOLEAN,
    OUT p_mensaje VARCHAR(255),
    OUT p_token_confirmacion VARCHAR(64)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_correo_formateado VARCHAR(100);
    v_clave_tutor VARCHAR(18);
    v_clave_codigo VARCHAR(18);
    v_token VARCHAR(64);
BEGIN
    p_exito := FALSE;
    v_correo_formateado := LOWER(TRIM(p_correo));

    SELECT "claveTutorAspirante" INTO v_clave_tutor
    FROM "TutorAspirante"
    WHERE LOWER(correo) = v_correo_formateado
      AND estado = TRUE;

    IF v_clave_tutor IS NULL THEN
        p_mensaje := 'Código inválido o expirado.';
        RETURN;
    END IF;

    SELECT "claveCodigoRecuperacion" INTO v_clave_codigo
    FROM "CodigosRecuperacionTutor"
    WHERE "claveTutorAspirante" = v_clave_tutor
      AND codigo = p_codigo
      AND usado = FALSE
      AND fecha_expiracion > CURRENT_TIMESTAMP
    ORDER BY fecha_creacion DESC
    LIMIT 1;

    IF v_clave_codigo IS NULL THEN
        p_mensaje := 'Código inválido o expirado.';
        RETURN;
    END IF;

    v_token := encode(gen_random_bytes(32), 'hex');

    UPDATE "CodigosRecuperacionTutor"
    SET usado = TRUE,
        token_confirmacion = v_token,
        token_expiracion = CURRENT_TIMESTAMP + INTERVAL '5 minutes'
    WHERE "claveCodigoRecuperacion" = v_clave_codigo;

    p_exito := TRUE;
    p_mensaje := 'Código validado correctamente.';
    p_token_confirmacion := v_token;
END;
$$;