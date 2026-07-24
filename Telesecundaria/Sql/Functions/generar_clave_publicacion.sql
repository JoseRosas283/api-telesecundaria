CREATE OR REPLACE FUNCTION generar_clave_publicacion()
RETURNS VARCHAR(18) AS $$
DECLARE
    v_ultimo_id INTEGER;
    v_nueva_clave VARCHAR(18);
BEGIN
    SELECT COALESCE(MAX(CAST(SUBSTRING("clavePublicacion" FROM 5) AS INTEGER)), 0)
    INTO v_ultimo_id
    FROM "Publicaciones";

    -- 'PUB-' (4) + 14 dígitos = 18 caracteres
    v_nueva_clave := 'PUB-' || LPAD((v_ultimo_id + 1)::TEXT, 14, '0');

    RETURN v_nueva_clave;
END;
$$ LANGUAGE plpgsql;