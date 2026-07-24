CREATE OR REPLACE FUNCTION generar_token_convocatoria()
RETURNS VARCHAR(20) AS $$
DECLARE
    v_ultimo_id INTEGER;
    v_clave VARCHAR(20);
BEGIN
    -- 1. Buscamos el número más alto después del prefijo 'TK-'
    -- Ignoramos los primeros 3 caracteres ('TK-') y convertimos el resto a entero
    SELECT COALESCE(MAX(CAST(SUBSTRING("claveTokenConvocatoria" FROM 4) AS INTEGER)), 0)
    INTO v_ultimo_id
    FROM "TokenConvocatorias";

    -- 2. Generamos la nueva clave sumando 1 y rellenando con ceros a 6 dígitos
    -- Resultado esperado: TK-000001, TK-000002...
    v_clave := 'TK-' || LPAD((v_ultimo_id + 1)::TEXT, 6, '0');

    RETURN v_clave;
END;
$$ LANGUAGE plpgsql;