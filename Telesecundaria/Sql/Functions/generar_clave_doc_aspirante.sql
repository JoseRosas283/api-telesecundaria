CREATE OR REPLACE FUNCTION generar_clave_doc_aspirante()
RETURNS VARCHAR(18) AS $$
DECLARE
    v_ultimo_id INTEGER;
    v_nueva_clave VARCHAR(18);
BEGIN
    -- 1. Buscamos el número más alto después del prefijo 'DOCA-'
    -- Usamos SUBSTRING desde la posición 6 para saltar 'DOCA-'
    SELECT COALESCE(MAX(CAST(SUBSTRING("claveDocAspirante" FROM 6) AS INTEGER)), 0)
    INTO v_ultimo_id
    FROM "DocumentosAspirante";

    -- 2. Generamos la nueva clave sumando 1
    -- Usamos LPAD a 8 dígitos para permitir millones de documentos sin perder el formato
    v_nueva_clave := 'DOCA-' || LPAD((v_ultimo_id + 1)::TEXT, 8, '0');

    RETURN v_nueva_clave;
END;
$$ LANGUAGE plpgsql;