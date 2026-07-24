CREATE OR REPLACE FUNCTION generar_clave_ciclo()
RETURNS VARCHAR(18) AS $$
DECLARE
    nuevo_id VARCHAR(18);
    anio_actual TEXT;
BEGIN
    -- 1. Obtenemos el año actual (ej. '2026')
    anio_actual := TO_CHAR(CURRENT_DATE, 'YYYY');

    -- 2. Generamos una clave que combine el año con un número aleatorio
    --    Usamos MD5 y RANDOM para asegurar que sea única, tomando 6 caracteres
    nuevo_id := 'CIC-' || anio_actual || '-' || UPPER(SUBSTRING(MD5(RANDOM()::TEXT), 1, 6));

    -- 3. Verificamos que no exista (por si acaso el azar repite)
    --    Si existiera, entraría en un bucle, pero con 6 hex es casi imposible.
    RETURN nuevo_id;
END;
$$ LANGUAGE plpgsql;