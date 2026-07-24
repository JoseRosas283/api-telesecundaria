CREATE OR REPLACE FUNCTION generar_clave_tipo_doc()
RETURNS VARCHAR(18) AS $$
DECLARE
    v_ultimo_id INTEGER;
    v_nueva_clave VARCHAR(18);
BEGIN
    -- 1. Buscamos el número más alto después del prefijo 'TIPO-'
    -- Usamos SUBSTRING desde la posición 6 para saltar 'TIPO-'
    SELECT COALESCE(MAX(CAST(SUBSTRING("claveTipoDocumento" FROM 6) AS INTEGER)), 0)
    INTO v_ultimo_id
    FROM "TipoDocumentos";

    -- 2. Incrementamos y formateamos. 
    -- Aunque sean pocos, usamos LPAD para mantener la estética.
    v_nueva_clave := 'TIPO-' || LPAD((v_ultimo_id + 1)::TEXT, 4, '0');

    RETURN v_nueva_clave;
END;
$$ LANGUAGE plpgsql;