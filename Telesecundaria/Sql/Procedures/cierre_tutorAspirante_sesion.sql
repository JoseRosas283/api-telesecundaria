CREATE OR REPLACE PROCEDURE cierre_tutorAspirante_sesion(
    p_claveToken VARCHAR(20),      -- La PK (ej: TK-000001)
    p_token_original TEXT,         -- Por seguridad, validamos que el token coincida con la PK
    OUT p_exito BOOLEAN,
    OUT p_mensaje VARCHAR
)
LANGUAGE plpgsql
AS $$
BEGIN
    
    UPDATE "TokenConvocatorias"
    SET 
        estado_sesion = FALSE,
        fecha_expiracion = CURRENT_TIMESTAMP
    WHERE "claveTokenConvocatoria" = p_claveToken
      AND token_original = p_token_original
      AND estado_sesion = TRUE;

    -- 2. Verificación
    IF FOUND THEN
        p_exito := TRUE;
        p_mensaje := 'Sesión finalizada. Cerradura bloqueada con éxito.';
    ELSE
        p_exito := FALSE;
        p_mensaje := 'No se pudo cerrar la sesión: El token ya no existe o ya estaba invalidado.';
    END IF;

EXCEPTION
    WHEN OTHERS THEN
        p_exito := FALSE;
        p_mensaje := 'Error en el proceso de logout: ' || SQLERRM;
END;
$$;