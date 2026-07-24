CREATE OR REPLACE FUNCTION generar_clave_revision()
RETURNS VARCHAR(18) AS $$
DECLARE
    v_ultimo_id INTEGER;
    v_nueva_clave VARCHAR(18);
BEGIN
    SELECT COALESCE(MAX(CAST(SUBSTRING("claveRevision" FROM 5) AS INTEGER)), 0)
    INTO v_ultimo_id
    FROM "Revisiones";

    v_nueva_clave := 'REV-' || LPAD((v_ultimo_id + 1)::TEXT, 8, '0');
    RETURN v_nueva_clave;
END;
$$ LANGUAGE plpgsql;