CREATE OR REPLACE PROCEDURE sp_generar_codigo_recuperacion_tutor(
    p_correo VARCHAR(100),
    OUT p_exito BOOLEAN,
    OUT p_mensaje VARCHAR(255),
    OUT p_clave_tutor_aspirante VARCHAR(18),
    OUT p_codigo VARCHAR(6)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_correo_formateado VARCHAR(100);
    v_clave_tutor VARCHAR(18);
BEGIN
    p_exito := FALSE;
    v_correo_formateado := LOWER(TRIM(p_correo));

    SELECT "claveTutorAspirante" INTO v_clave_tutor
    FROM "TutorAspirante"
    WHERE LOWER(correo) = v_correo_formateado
      AND estado = TRUE;

    IF v_clave_tutor IS NULL THEN
        p_mensaje := 'Si el correo está registrado, se enviará un código de verificación.';
        RETURN;
    END IF;

    UPDATE "CodigosRecuperacionTutor"
    SET usado = TRUE
    WHERE "claveTutorAspirante" = v_clave_tutor
      AND usado = FALSE;

    p_codigo := LPAD(FLOOR(RANDOM() * 1000000)::TEXT, 6, '0');

    INSERT INTO "CodigosRecuperacionTutor" ("claveTutorAspirante", codigo, fecha_expiracion)
    VALUES (v_clave_tutor, p_codigo, CURRENT_TIMESTAMP + INTERVAL '40 seconds');

    p_exito := TRUE;
    p_clave_tutor_aspirante := v_clave_tutor;
    p_mensaje := 'Si el correo está registrado, se enviará un código de verificación.';
END;
$$;