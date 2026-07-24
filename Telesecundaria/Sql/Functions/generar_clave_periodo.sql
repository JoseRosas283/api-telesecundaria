CREATE OR REPLACE FUNCTION generar_clave_periodo()
RETURNS VARCHAR(18) AS $$
DECLARE
    v_ultimo_id INTEGER;
    v_nueva_clave VARCHAR(18);
BEGIN
    -- Buscamos el número más alto después del prefijo 'PER-'
    SELECT COALESCE(MAX(CAST(SUBSTRING("clavePeriodo" FROM 5) AS INTEGER)), 0)
    INTO v_ultimo_id
    FROM "Periodos";

    -- Generamos la nueva clave (Ej: PER-00000001)
    v_nueva_clave := 'PER-' || LPAD((v_ultimo_id + 1)::TEXT, 8, '0');

    RETURN v_nueva_clave;
END;
$$ LANGUAGE plpgsql;