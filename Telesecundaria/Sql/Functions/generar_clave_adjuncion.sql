CREATE OR REPLACE FUNCTION generar_clave_adjuncion()
RETURNS VARCHAR(18) AS $$
DECLARE
    v_ultimo_id INTEGER;
    v_nueva_clave VARCHAR(18);
BEGIN
    -- 1. Buscamos el número más alto después del prefijo 'ADJ-'
    -- Substring desde la posición 5 para saltar 'ADJ-'
    SELECT COALESCE(MAX(CAST(SUBSTRING("claveAdjuncion" FROM 5) AS INTEGER)), 0)
    INTO v_ultimo_id
    FROM "Adjunciones";

    -- 2. Generamos la nueva clave sumando 1
    -- Usamos LPAD a 8 dígitos para tener un margen enorme (hasta 99 millones)
    v_nueva_clave := 'ADJ-' || LPAD((v_ultimo_id + 1)::TEXT, 8, '0');

    RETURN v_nueva_clave;
END;
$$ LANGUAGE plpgsql;