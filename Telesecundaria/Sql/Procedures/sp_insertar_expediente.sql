CREATE OR REPLACE PROCEDURE sp_insertar_expediente(
    p_nombre VARCHAR(80),
    p_apellido_paterno VARCHAR(80),
    p_apellido_materno VARCHAR(80),
    p_curp VARCHAR(18),
    p_tipo_titular VARCHAR(20),
    p_claveEntrega VARCHAR(18) DEFAULT NULL
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_tipo_normalizado VARCHAR(20);
    v_clave_generada VARCHAR(18); -- Variable añadida para capturar el ID
    v_curp_formateada VARCHAR(18); -- NUEVA: Para procesar la CURP limpia
    
    -- NUEVAS VARIABLES PARA ADUANA BIOMÉTRICA DE CURP
    v_paterno_limpio TEXT;
    v_materno_limpio TEXT;
    v_curp_fecha_str TEXT;
    v_curp_anio INT;
    v_curp_mes INT;
    v_curp_dia INT;
    v_fecha_nacimiento DATE;
    v_anio_actual INT := EXTRACT(YEAR FROM CURRENT_DATE);
    v_edad_titular INT;
    
    v_letra_paterno CHAR(1);
    v_vocal_paterno TEXT;
    v_letra_materno CHAR(1);
BEGIN
    -- ============================================================
    -- 1. VALIDACIÓN DE NULOS Y VACÍOS (Excepto Apellido Materno y ClaveEntrega)
    -- ============================================================
    IF p_nombre IS NULL OR TRIM(p_nombre) = '' THEN
        RAISE EXCEPTION 'Error: El nombre es obligatorio.';
    END IF;

    IF p_apellido_paterno IS NULL OR TRIM(p_apellido_paterno) = '' THEN
        RAISE EXCEPTION 'Error: El apellido paterno es obligatorio.';
    END IF;

    IF p_curp IS NULL OR TRIM(p_curp) = '' THEN
        RAISE EXCEPTION 'Error: La CURP es obligatoria.';
    END IF;

    -- ============================================================
    -- 2. VALIDACIÓN DE CURP (Longitud y Expresión Regular estándar)
    -- ============================================================
    v_curp_formateada := UPPER(TRIM(p_curp));

    IF LENGTH(v_curp_formateada) <> 18 THEN
        RAISE EXCEPTION 'Error: La CURP debe tener exactamente 18 caracteres.';
    END IF;

    IF v_curp_formateada !~ '^[A-Z]{4}[0-9]{6}[HM][A-Z]{5}[0-9A-Z]{2}$' THEN
        RAISE EXCEPTION 'Error: El formato de la CURP % es inválido.', v_curp_formateada;
    END IF;

    -- ============================================================
    -- 2.5 ADUANA BIOMÉTRICA DE CURP Y FILTROS DE EDAD INSTITUCIONALES
    -- ============================================================
    -- Eliminamos acentos en memoria para el cruce de letras sin afectar los datos originales
    v_paterno_limpio := TRANSLATE(UPPER(TRIM(p_apellido_paterno)), 'ÁÉÍÓÚÜ', 'AEIOUU');
    v_materno_limpio := TRANSLATE(UPPER(TRIM(p_apellido_materno)), 'ÁÉÍÓÚÜ', 'AEIOUU');

    -- A) Extracción de fecha de la CURP
    v_curp_fecha_str := SUBSTRING(v_curp_formateada FROM 5 FOR 6);
    v_curp_anio := SUBSTRING(v_curp_fecha_str FROM 1 FOR 2)::INT;
    v_curp_mes := SUBSTRING(v_curp_fecha_str FROM 3 FOR 2)::INT;
    v_curp_dia := SUBSTRING(v_curp_fecha_str FROM 5 FOR 2)::INT;

    -- Asignación oficial de siglo (RENAPO) adaptado dinámicamente
    IF v_curp_anio <= (v_anio_actual - 2000) THEN
        v_curp_anio := 2000 + v_curp_anio;
    ELSE
        v_curp_anio := 1900 + v_curp_anio;
    END IF;

    -- Validar rangos del calendario antes de la conversión real
    IF v_curp_mes < 1 OR v_curp_mes > 12 THEN
        RAISE EXCEPTION 'Fraude de CURP: El mes de nacimiento [%] extraído de la CURP no es válido.', v_curp_mes;
    END IF;

    IF v_curp_dia < 1 OR v_curp_dia > 31 THEN
        RAISE EXCEPTION 'Fraude de CURP: El día de nacimiento [%] extraído de la CURP no es válido.', v_curp_dia;
    END IF;

    -- Control de fechas inexistentes en el calendario (ej. 30 de febrero)
    BEGIN
        v_fecha_nacimiento := TO_DATE(v_curp_anio || '-' || v_curp_mes || '-' || v_curp_dia, 'YYYY-MM-DD');
    EXCEPTION WHEN OTHERS THEN
        RAISE EXCEPTION 'Fraude de CURP: La combinación de fecha [%] en la CURP no existe en el calendario real.', v_curp_fecha_str;
    END;

    -- Candados temporales de coherencia
    IF v_curp_anio < 1910 THEN
        RAISE EXCEPTION 'Error de coherencia cronológica: El año de nacimiento [%] es anterior a 1910.', v_curp_anio;
    END IF;

    IF v_fecha_nacimiento > CURRENT_DATE THEN
        RAISE EXCEPTION 'Error de coherencia cronológica: La fecha extraída de la CURP [%] pertenece al futuro.', TO_CHAR(v_fecha_nacimiento, 'DD-MM-YYYY');
    END IF;

    -- B) Cálculo y validación de edad según el Tipo de Titular (LÍMITES DOBLES)
    v_edad_titular := EXTRACT(YEAR FROM AGE(CURRENT_DATE, v_fecha_nacimiento));
    v_tipo_normalizado := INITCAP(TRIM(p_tipo_titular));

    IF v_tipo_normalizado = 'Empleado' THEN
        IF v_edad_titular < 18 THEN
            RAISE EXCEPTION 'Bloqueo Laboral: El titular cuenta con % años. No se permite el registro de Empleados menores de 18 años.', v_edad_titular;
        ELSIF v_edad_titular > 75 THEN
            RAISE EXCEPTION 'Bloqueo Laboral: El titular cuenta con % años. Excede el rango límite institucional de contratación (Máximo 75 años).', v_edad_titular;
        END IF;
    ELSIF v_tipo_normalizado = 'Alumno' THEN
        IF v_edad_titular < 12 OR v_edad_titular > 18 THEN
            RAISE EXCEPTION 'Bloqueo Escolar: El aspirante cuenta con % años. El rango permitido para ingresar al plantel es estrictamente de 12 a 18 años de edad.', v_edad_titular;
        END IF;
    END IF;

    -- C) Cruce de Letras contra Apellido Paterno
    v_letra_paterno := SUBSTRING(v_paterno_limpio FROM 1 FOR 1);
    
    v_vocal_paterno := SUBSTRING(v_paterno_limpio FROM 2 FOR 1);
    IF v_vocal_paterno !~ '[AEIOU]' THEN
        SELECT (regexp_matches(SUBSTRING(v_paterno_limpio FROM 2), '[AEIOU]'))[1] INTO v_vocal_paterno;
    END IF;

    IF SUBSTRING(v_curp_formateada FROM 1 FOR 1) <> v_letra_paterno THEN
        RAISE EXCEPTION 'Incoherencia biográfica: La primera letra de la CURP [%] no coincide con la inicial del Apellido Paterno [%].', 
            SUBSTRING(v_curp_formateada FROM 1 FOR 1), v_letra_paterno;
    END IF;

    IF v_vocal_paterno IS NOT NULL AND SUBSTRING(v_curp_formateada FROM 2 FOR 1) <> v_vocal_paterno THEN
        RAISE EXCEPTION 'Incoherencia biográfica: La segunda letra de la CURP [%] debe ser la primera vocal interna del Apellido Paterno [%].', 
            SUBSTRING(v_curp_formateada FROM 2 FOR 1), v_vocal_paterno;
    END IF;

    -- D) Cruce de Letras contra Apellido Materno
    IF p_apellido_materno IS NOT NULL AND TRIM(p_apellido_materno) <> '' THEN
        v_letra_materno := SUBSTRING(v_materno_limpio FROM 1 FOR 1);
        IF SUBSTRING(v_curp_formateada FROM 3 FOR 1) <> v_letra_materno THEN
            RAISE EXCEPTION 'Incoherencia biográfica: La tercera letra de la CURP [%] no coincide con la inicial del Apellido Materno [%].', 
                SUBSTRING(v_curp_formateada FROM 3 FOR 1), v_letra_materno;
        END IF;
    ELSE
        IF SUBSTRING(v_curp_formateada FROM 3 FOR 1) <> 'X' THEN
            RAISE EXCEPTION 'Incoherencia biográfica: Al omitir el Apellido Materno, la tercera posición de la CURP debe ser "X" y se recibió "%".', 
                SUBSTRING(v_curp_formateada FROM 3 FOR 1);
        END IF;
    END IF;

    -- ============================================================
    -- 3. NORMALIZACIÓN DEL TIPO (Amarre)
    -- ============================================================
    IF v_tipo_normalizado NOT IN ('Alumno', 'Empleado') THEN
        RAISE EXCEPTION 'Error: Tipo de titular no válido. Solo Alumno o Empleado.';
    END IF;

    -- ============================================================
    -- 4. LÓGICA DE NEGOCIO PARA ENTREGAS
    -- ============================================================
    -- Regla A: Si es Empleado, NUNCA lleva entrega (forzamos NULL)
    IF v_tipo_normalizado = 'Empleado' THEN
        IF p_claveEntrega IS NOT NULL THEN
            RAISE NOTICE 'Aviso: Los empleados no gestionan entregas de documentos. Se ignorará la clave proporcionada.';
        END IF;
        p_claveEntrega := NULL;
        
    -- Regla B: Si es Alumno, la entrega es opcional (por alumnos ya inscritos)
    -- Pero si se proporciona una, validamos que exista en la tabla Entregas
    ELSIF v_tipo_normalizado = 'Alumno' AND p_claveEntrega IS NOT NULL THEN
        IF NOT EXISTS (SELECT 1 FROM "Entregas" WHERE "claveEntrega" = p_claveEntrega) THEN
            RAISE EXCEPTION 'Error: La clave de entrega % no existe en el sistema.', p_claveEntrega;
        END IF;
    END IF;

    -- ============================================================
    -- 5. INSERCIÓN FINAL (Añadido RETURNING para obtener la clave)
    -- ============================================================
    INSERT INTO "Expedientes" (
        nombre,
        apellido_paterno,
        apellido_materno,
        curp,
        "tipo_titular",
        "claveEntrega"
    ) VALUES (
        UPPER(TRIM(p_nombre)),
        UPPER(TRIM(p_apellido_paterno)),
        COALESCE(UPPER(TRIM(p_apellido_materno)), ''), -- Control de vacíos homologado
        v_curp_formateada,
        v_tipo_normalizado,
        p_claveEntrega
    ) RETURNING "claveExpediente" INTO v_clave_generada;

    -- NUEVO CAMBIO: Encapsulamiento de alta automática
    IF v_tipo_normalizado = 'Alumno' THEN
        CALL sp_insertar_alumno(v_clave_generada);
    END IF;

    RAISE NOTICE 'Éxito: Expediente de % creado correctamente como %.', UPPER(p_nombre), v_tipo_normalizado;

EXCEPTION
    WHEN unique_violation THEN
        RAISE EXCEPTION 'Error: Ya existe un expediente con la CURP %.', v_curp_formateada;
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error al crear expediente: %', SQLERRM;
END;
$$;
