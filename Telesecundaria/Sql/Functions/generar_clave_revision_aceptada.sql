CREATE OR REPLACE FUNCTION generar_clave_revision_aceptada()
RETURNS VARCHAR(18) AS $$
DECLARE
    v_ultimo_id INTEGER;
    v_nueva_clave VARCHAR(18);
BEGIN
    -- 1. Buscamos el número más alto después del prefijo 'RVA-' (4 caracteres)
    -- Extraemos desde la posición 5 en adelante y convertimos a entero
    SELECT COALESCE(MAX(CAST(SUBSTRING("claveRevisionAceptada" FROM 5) AS INTEGER)), 0)
    INTO v_ultimo_id
    FROM "RevisionesAceptadas";

    -- 2. Construimos la nueva clave:
    -- Prefijo 'RVA-' (4 caracteres) + LPAD de 14 ceros = 18 caracteres totales
    v_nueva_clave := 'RVA-' || LPAD((v_ultimo_id + 1)::TEXT, 14, '0');

    RETURN v_nueva_clave;
END;
$$ LANGUAGE plpgsql;