CREATE OR REPLACE FUNCTION generar_clave_empleado()
RETURNS VARCHAR(18) AS $$
DECLARE
    v_ultimo_id INTEGER;
    v_clave VARCHAR(18);
BEGIN
    -- Buscamos el número más alto después del prefijo 'EMP-'
    -- Usamos SUBSTRING para ignorar los primeros 4 caracteres ('EMP-') y convertir el resto a número
    SELECT COALESCE(MAX(CAST(SUBSTRING("claveEmpleado" FROM 5) AS INTEGER)), 0)
    INTO v_ultimo_id
    FROM "Empleados";

    -- Generamos la nueva clave sumando 1 y rellenando con ceros a 6 dígitos
    -- Ejemplo: EMP-000001, EMP-000002...
    v_clave := 'EMP-' || LPAD((v_ultimo_id + 1)::TEXT, 6, '0');

    RETURN v_clave;
END;
$$ LANGUAGE plpgsql;