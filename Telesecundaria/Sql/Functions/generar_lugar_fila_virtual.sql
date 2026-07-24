CREATE OR REPLACE FUNCTION generar_lugar_fila_virtual()
RETURNS VARCHAR(18) AS $$
DECLARE
    v_ultimo_id INTEGER;
    v_nueva_clave VARCHAR(18);
BEGIN
    -- 1. Buscamos el número más alto después del prefijo 'FILA-' (5 caracteres)
    -- Usamos SUBSTRING desde la posición 6 para extraer solo el número
    SELECT COALESCE(MAX(CAST(SUBSTRING("claveFila" FROM 6) AS INTEGER)), 0)
    INTO v_ultimo_id
    FROM "FilaVirtual";

    -- 2. Construimos la nueva clave:
    -- Prefijo 'FILA-' (5 caracteres) + LPAD de 13 ceros = 18 caracteres totales.
    v_nueva_clave := 'FILA-' || LPAD((v_ultimo_id + 1)::TEXT, 13, '0');

    RETURN v_nueva_clave;
END;
$$ LANGUAGE plpgsql;