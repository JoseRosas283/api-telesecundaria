CREATE OR REPLACE FUNCTION generar_clave_imagen()
RETURNS VARCHAR(18) AS $$
DECLARE
    v_ultimo_id INTEGER;
    v_nueva_clave VARCHAR(18);
BEGIN
    -- 1. Buscamos el número más alto después del prefijo 'IMG-'
    -- Substring desde la posición 5 para saltar 'IMG-'
    SELECT COALESCE(MAX(CAST(SUBSTRING("claveImagen" FROM 5) AS INTEGER)), 0)
    INTO v_ultimo_id
    FROM "GaleriaImagenes"; -- Asumiendo que tu tabla se llama Imagenes

    -- 2. Construimos la nueva clave:
    -- Prefijo 'IMG-' (4 caracteres) + LPAD de 14 ceros = 18 caracteres totales.
    v_nueva_clave := 'IMG-' || LPAD((v_ultimo_id + 1)::TEXT, 14, '0');

    RETURN v_nueva_clave;
END;
$$ LANGUAGE plpgsql;