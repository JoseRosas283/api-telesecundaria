CREATE OR REPLACE FUNCTION generar_clave_cita()
RETURNS VARCHAR(18) AS $$
DECLARE
    v_ultimo_id INTEGER;
    v_nueva_clave VARCHAR(18);
BEGIN
    -- 1. Buscamos el número más alto después del prefijo 'CITA-'
    -- Substring desde la posición 6 para saltar 'CITA-'
    SELECT COALESCE(MAX(CAST(SUBSTRING("claveCita" FROM 6) AS INTEGER)), 0)
    INTO v_ultimo_id
    FROM "CitasInscripcion";

    -- 2. Construimos la nueva clave:
    -- Prefijo 'CITA-' (5 caracteres) + LPAD de 13 ceros = 18 caracteres totales.
    v_nueva_clave := 'CITA-' || LPAD((v_ultimo_id + 1)::TEXT, 13, '0');

    RETURN v_nueva_clave;
END;
$$ LANGUAGE plpgsql;