CREATE OR REPLACE FUNCTION generar_clave_asig()
RETURNS VARCHAR(18) AS $$
DECLARE
    v_ultimo_id INTEGER;
    v_nueva_clave VARCHAR(18);
BEGIN
    -- 1. Obtenemos el número máximo actual de la tabla
    -- Usamos COALESCE para que si la tabla está vacía, empiece en 0
    SELECT COALESCE(MAX(SUBSTRING("claveAsignacion" FROM 5)::INTEGER), 0)
    INTO v_ultimo_id
    FROM "AsignacionGrupo";

    -- 2. Incrementamos el contador
    v_ultimo_id := v_ultimo_id + 1;

    -- 3. Formateamos la clave: prefijo 'ASG-' + número con relleno de ceros
    -- Ejemplo: ASG-00000000000001
    v_nueva_clave := 'ASG-' || LPAD(v_ultimo_id::TEXT, 14, '0');

    RETURN v_nueva_clave;
END;
$$ LANGUAGE plpgsql;