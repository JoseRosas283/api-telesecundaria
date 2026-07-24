CREATE OR REPLACE FUNCTION generar_clave_ruta_rechazada()
RETURNS VARCHAR(18) AS $$
DECLARE
    v_ultimo_id INTEGER;
    v_nueva_clave VARCHAR(18);
BEGIN
    -- 1. Buscamos el número más alto después del prefijo 'RTR-'
    -- Substring desde la posición 5 para saltar 'RTR-' (R=1, T=2, R=3, -=4)
    SELECT COALESCE(MAX(CAST(SUBSTRING("claveRuta" FROM 5) AS INTEGER)), 0)
    INTO v_ultimo_id
    FROM "RutasRechazadas";

    -- 2. Construimos la nueva clave:
    -- Prefijo 'RTR-' (4 caracteres) + LPAD de 14 ceros = 18 caracteres totales.
    v_nueva_clave := 'RTR-' || LPAD((v_ultimo_id + 1)::TEXT, 14, '0');

    RETURN v_nueva_clave;
END;
$$ LANGUAGE plpgsql;