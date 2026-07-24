CREATE OR REPLACE FUNCTION generar_clave_logueo()
RETURNS VARCHAR(18) AS $$
DECLARE
    v_ultimo_id INTEGER;
    v_nueva_clave VARCHAR(18);
BEGIN
    -- 1. Buscamos el número más alto después del prefijo 'LOG-'
    -- Substring desde la posición 5 para saltar 'LOG-' (L=1, O=2, G=3, -=4)
    SELECT COALESCE(MAX(CAST(SUBSTRING("claveLogueo" FROM 5) AS INTEGER)), 0)
    INTO v_ultimo_id
    FROM "Logueos"; -- O la tabla donde se almacene la claveLogueo

    -- 2. Construimos la nueva clave:
    -- Prefijo 'LOG-' (4 caracteres) + LPAD de 14 ceros = 18 caracteres totales.
    v_nueva_clave := 'LOG-' || LPAD((v_ultimo_id + 1)::TEXT, 14, '0');

    RETURN v_nueva_clave;
END;
$$ LANGUAGE plpgsql;