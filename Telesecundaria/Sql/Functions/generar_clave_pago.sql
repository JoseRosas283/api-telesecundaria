CREATE OR REPLACE FUNCTION generar_clave_pago()
RETURNS VARCHAR(18) AS $$
DECLARE
    v_ultimo_id INTEGER;
    v_nueva_clave VARCHAR(18);
BEGIN
    SELECT COALESCE(MAX(CAST(SUBSTRING("clavePago" FROM 5) AS INTEGER)), 0)
    INTO v_ultimo_id
    FROM "Pagos";

    v_nueva_clave := 'PAG-' || LPAD((v_ultimo_id + 1)::TEXT, 8, '0');
    RETURN v_nueva_clave;
END;
$$ LANGUAGE plpgsql;