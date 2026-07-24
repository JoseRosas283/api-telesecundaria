CREATE OR REPLACE FUNCTION generar_clave_grupo()
RETURNS VARCHAR(18) AS $$
DECLARE
    v_ultimo_id INTEGER;
    v_nueva_clave VARCHAR(18);
BEGIN
    -- 1. Buscamos el número más alto después del prefijo 'GRU-'
    -- Usamos SUBSTRING desde la posición 5 para obtener solo los números
    SELECT COALESCE(MAX(CAST(SUBSTRING("claveGrupo" FROM 5) AS INTEGER)), 0)
    INTO v_ultimo_id
    FROM "Grupos";

    -- 2. Generamos la nueva clave sumando 1 al último ID
    -- LPAD asegura que el número tenga 8 dígitos (ej: 00000001)
    v_nueva_clave := 'GRU-' || LPAD((v_ultimo_id + 1)::TEXT, 8, '0');

    RETURN v_nueva_clave;
END;
$$ LANGUAGE plpgsql;