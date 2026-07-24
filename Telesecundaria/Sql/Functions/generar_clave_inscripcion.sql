CREATE OR REPLACE FUNCTION generar_clave_inscripcion()
RETURNS VARCHAR(18) AS $$
DECLARE
    v_ultimo_id INTEGER;
    v_nueva_clave VARCHAR(18);
BEGIN
    SELECT COALESCE(MAX(CAST(SUBSTRING("claveInscripcion" FROM 5) AS INTEGER)), 0)
    INTO v_ultimo_id
    FROM "Inscripciones";

    v_nueva_clave := 'INS-' || LPAD((v_ultimo_id + 1)::TEXT, 8, '0');
    RETURN v_nueva_clave;
END;
$$ LANGUAGE plpgsql;