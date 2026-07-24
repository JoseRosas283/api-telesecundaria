CREATE OR REPLACE FUNCTION generar_clave_modulo()
RETURNS VARCHAR(18) AS $$
DECLARE
    v_ultimo_id INTEGER;
    v_nueva_clave VARCHAR(18);
BEGIN
    -- 1. Buscamos el número más alto después del prefijo 'MODU-'
    -- Substring desde la posición 6 para saltar 'MODU-'
    SELECT COALESCE(MAX(CAST(SUBSTRING("claveModulo" FROM 6) AS INTEGER)), 0)
    INTO v_ultimo_id
    FROM "Modulos";

    -- 2. Construimos la nueva clave:
    -- Prefijo 'MODU-' (5 caracteres) + LPAD de 13 ceros = 18 caracteres totales.
    v_nueva_clave := 'MODU-' || LPAD((v_ultimo_id + 1)::TEXT, 13, '0');

    RETURN v_nueva_clave;
END;
$$ LANGUAGE plpgsql;