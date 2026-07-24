CREATE OR REPLACE FUNCTION generar_clave_convocatoria()
RETURNS VARCHAR(18) AS $$
DECLARE
    v_ultimo_id INTEGER;
    v_nueva_clave VARCHAR(18);
BEGIN
    -- 1. Buscamos el número más alto después del prefijo 'CONV-'
    -- Usamos SUBSTRING desde la posición 6 para saltar 'CONV-'
    SELECT COALESCE(MAX(CAST(SUBSTRING("claveConvocatoria" FROM 6) AS INTEGER)), 0)
    INTO v_ultimo_id
    FROM "Convocatorias";

    -- 2. Incrementamos y formateamos a 8 dígitos para el correlativo
    -- Ejemplo: CONV-00000001
    v_nueva_clave := 'CONV-' || LPAD((v_ultimo_id + 1)::TEXT, 8, '0');

    RETURN v_nueva_clave;
END;
$$ LANGUAGE plpgsql;