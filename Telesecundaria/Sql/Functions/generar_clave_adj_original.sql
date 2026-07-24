CREATE OR REPLACE FUNCTION generar_clave_adj_original()
RETURNS VARCHAR(18) AS $$
DECLARE
    v_ultimo_id INTEGER;
    v_nueva_clave VARCHAR(18);
BEGIN
    -- 1. Buscamos el número más alto después del prefijo 'ADJO-'
    -- Usamos SUBSTRING desde la posición 6 para saltar 'ADJO-'
    SELECT COALESCE(MAX(CAST(SUBSTRING("claveAdjOriginal" FROM 6) AS INTEGER)), 0)
    INTO v_ultimo_id
    FROM "AdjuncionesOriginales";

    -- 2. Incrementamos y formateamos con LPAD a 12 caracteres para mantener la estética
    v_nueva_clave := 'ADJO-' || LPAD((v_ultimo_id + 1)::TEXT, 12, '0');

    RETURN v_nueva_clave;
END;
$$ LANGUAGE plpgsql;