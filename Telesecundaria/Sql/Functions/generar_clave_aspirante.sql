CREATE OR REPLACE FUNCTION generar_clave_aspirante()
RETURNS VARCHAR(18) AS $$
DECLARE
    anio TEXT;
    ultimo_id INT;
    siguiente_id INT;
    clave_final VARCHAR(18);
BEGIN
    -- 1. Bloqueo preventivo para evitar que dos aspirantes choquen en milisegundos
    LOCK TABLE "Aspirantes" IN EXCLUSIVE MODE;

    -- 2. Obtener el año actual a dos dígitos (ej. 26)
    anio := TO_CHAR(CURRENT_DATE, 'YY');

    -- 3. Buscar el número más alto para este año
    --    Formato esperado: ASP-26-00000001
    --    'ASP-26-' ocupa 7 espacios, el número empieza en la posición 8
    SELECT COALESCE(MAX(SUBSTRING("claveAspirante" FROM 8)::INT), 0)
    INTO ultimo_id
    FROM "Aspirantes"
    WHERE "claveAspirante" LIKE 'ASP-' || anio || '-%';

    -- 4. Incrementar
    siguiente_id := ultimo_id + 1;

    -- 5. Construir la cadena de 18 caracteres
    --    ASP-(4) + YY(2) + -(1) + 00000000(8) = 15 caracteres base
    --    LPAD a 8 asegura que el número sea largo y profesional
    clave_final := 'ASP-' || anio || '-' || LPAD(siguiente_id::TEXT, 8, '0');

    RETURN clave_final;
END;
$$ LANGUAGE plpgsql;