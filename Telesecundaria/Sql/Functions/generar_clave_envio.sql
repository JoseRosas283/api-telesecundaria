CREATE OR REPLACE FUNCTION generar_clave_envio()
RETURNS VARCHAR(18) AS $$
DECLARE
    v_ultimo_id INTEGER;
    v_nueva_clave VARCHAR(18);
BEGIN
    SELECT COALESCE(MAX(CAST(SUBSTRING("claveEnvio" FROM 6) AS INTEGER)), 0)
    INTO v_ultimo_id
    FROM "Envios";

    v_nueva_clave := 'ENVI-' || LPAD((v_ultimo_id + 1)::TEXT, 13, '0');

    RETURN v_nueva_clave;
END;
$$ LANGUAGE plpgsql;