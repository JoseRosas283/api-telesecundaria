CREATE OR REPLACE FUNCTION generar_clave_tutor_aspirante()
RETURNS VARCHAR(18) AS $$
DECLARE
    v_ultimo_id INTEGER;
    v_nueva_clave VARCHAR(18);
BEGIN
    -- 1. Buscamos el número más alto después del prefijo 'TUT-'
    -- Usamos SUBSTRING desde la posición 5 para saltar 'TUT-'
    SELECT COALESCE(MAX(CAST(SUBSTRING("claveTutorAspirante" FROM '[0-9]+') AS INTEGER)), 0)
    INTO v_ultimo_id
    FROM "TutorAspirante";

    -- 2. Incrementamos y formateamos a 8 dígitos para el correlativo
    -- Ejemplo: TUT-00000001
    v_nueva_clave := 'TUTA-' || LPAD((v_ultimo_id + 1)::TEXT, 8, '0');

    RETURN v_nueva_clave;
END;
$$ LANGUAGE plpgsql;