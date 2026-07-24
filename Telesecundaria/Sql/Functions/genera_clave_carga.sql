CREATE OR REPLACE FUNCTION genera_clave_carga()
RETURNS VARCHAR(18) AS $$
DECLARE
    v_ultimo_id INTEGER;
    v_clave VARCHAR(18);
BEGIN
    -- 1. Buscamos el número más alto después del prefijo 'CRG-'
    -- SUBSTRING desde el carácter 5 para saltar 'CRG-'
    SELECT COALESCE(MAX(CAST(SUBSTRING("claveCarga" FROM 5) AS INTEGER)), 0)
    INTO v_ultimo_id
    FROM "CargasDocumentos";

    -- 2. Generamos la nueva clave sumando 1
    -- Usamos LPAD a 14 dígitos para completar los 18 caracteres totales (4 del prefijo + 14 números)
    v_clave := 'CRG-' || LPAD((v_ultimo_id + 1)::TEXT, 14, '0');

    RETURN v_clave;
END;
$$ LANGUAGE plpgsql;