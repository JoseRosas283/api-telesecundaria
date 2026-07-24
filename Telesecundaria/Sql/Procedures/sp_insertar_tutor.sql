CREATE OR REPLACE PROCEDURE sp_insertar_tutor(
    p_nombre VARCHAR(50),
    p_apellido_paterno VARCHAR(50),
    p_apellido_materno VARCHAR(50),
    p_curp_tutor VARCHAR(18),
    p_telefono VARCHAR(15),
    p_correo VARCHAR(100),
    p_parentesco VARCHAR(50),
    p_estado BOOLEAN DEFAULT TRUE
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_telefono_limpio VARCHAR(15);
    v_correo_formateado VARCHAR(100); -- NUEVA: Para estandarizar el correo antes del INSERT y CALL
    v_clave_tutor_generada VARCHAR(18); -- NUEVA: Para capturar el ID del DEFAULT generar_clave_tutor()
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
    
    v_letra_paterno CHAR(1);
    v_vocal_paterno TEXT;
    v_letra_materno CHAR(1);
    
    -- EXPRESIÓN REGULAR: Para validar que el correo tenga estructura real
    v_regex_curp TEXT := '^[A-Z]{4}[0-9]{6}[HM][A-Z]{5}[0-9A-Z][0-9]$';
    v_regex_correo TEXT := '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$';
BEGIN
    -- ============================================================
    -- 1. VALIDACIÓN DE CAMPOS OBLIGATORIOS (Nulos y Vacíos)
    -- ============================================================
    IF p_nombre IS NULL OR TRIM(p_nombre) = '' THEN
        RAISE EXCEPTION 'Error: El nombre del tutor es obligatorio.';
    END IF;

    IF p_apellido_paterno IS NULL OR TRIM(p_apellido_paterno) = '' THEN
        RAISE EXCEPTION 'Error: El apellido paternal del tutor es obligatorio.'; -- Ajustado para homologar con el de dirección
    END IF;

    IF p_curp_tutor IS NULL OR TRIM(p_curp_tutor) = '' THEN
        RAISE EXCEPTION 'Error: La CURP del tutor es obligatoria.';
    END IF;

    -- AGREGADO: Validación obligatoria del teléfono
    IF p_telefono IS NULL OR TRIM(p_telefono) = '' THEN
        RAISE EXCEPTION 'Error: El teléfono de contacto del tutor es obligatorio.';
    END IF;

    IF p_parentesco IS NULL OR TRIM(p_parentesco) = '' THEN
        RAISE EXCEPTION 'Error: El parentesco es obligatorio (Ej: Padre, Madre, Tutor Legal).';
    END IF;

    -- ============================================================
    -- 2. VALIDACIÓN DEL TELÉFONO (Rango de 10 a 15 caracteres si existe)
    -- ============================================================
    v_telefono_limpio := TRIM(p_telefono);

    -- Evaluamos su longitud de forma directa ya que ahora es obligatorio
    IF v_telefono_limpio !~ '^[0-9]+$' THEN 
        RAISE EXCEPTION 'Error: El teléfono debe contener únicamente números.'; 
    END IF;

    IF LENGTH(v_telefono_limpio) < 10 THEN
        RAISE EXCEPTION 'Error: El teléfono % es demasiado corto. Debe tener mínimo 10 dígitos.', v_telefono_limpio;
    END IF;
    
    IF LENGTH(v_telefono_limpio) > 15 THEN
        RAISE EXCEPTION 'Error: El teléfono % es demasiado largo. Debe tener máximo 15 dígitos.', v_telefono_limpio;
    END IF;

    -- ============================================================
    -- 3. VALIDACIÓN DE FORMATO DE CURP Y DISPONIBILIDAD DE CORREO
    -- ============================================================
    v_curp_formateada := UPPER(TRIM(p_curp_tutor));

    IF LENGTH(v_curp_formateada) <> 18 THEN
        RAISE EXCEPTION 'Error: La CURP del tutor debe tener exactamente 18 caracteres.';
    END IF;

    IF v_curp_formateada !~ v_regex_curp THEN
        RAISE EXCEPTION 'Error: La CURP % no tiene un formato oficial válido.', v_curp_formateada;
    END IF;

    -- ============================================================
    -- 3.1 ADUANA BIOMÉTRICA DE CURP (CRUCE ESTRICTO DE IDENTIDAD)
    -- ============================================================
    -- Estandarización de apellidos eliminando acentos/tildes en memoria para la aduana de letras
    v_paterno_limpio := TRANSLATE(UPPER(TRIM(p_apellido_paterno)), 'ÁÉÍÓÚÜ', 'AEIOUU');
    v_materno_limpio := TRANSLATE(UPPER(TRIM(p_apellido_materno)), 'ÁÉÍÓÚÜ', 'AEIOUU');

    -- A) EXTRACCIÓN Y VALIDACIÓN DE FECHA DE NACIMIENTO
    v_curp_fecha_str := SUBSTRING(v_curp_formateada FROM 5 FOR 6);
    v_curp_anio := SUBSTRING(v_curp_fecha_str FROM 1 FOR 2)::INT;
    v_curp_mes := SUBSTRING(v_curp_fecha_str FROM 3 FOR 2)::INT;
    v_curp_dia := SUBSTRING(v_curp_fecha_str FROM 5 FOR 2)::INT;

    -- Lógica de siglo (RENAPO) adaptada dinámicamente al año corriente
    IF v_curp_anio <= (v_anio_actual - 2000) THEN
        v_curp_anio := 2000 + v_curp_anio;
    ELSE
        v_curp_anio := 1900 + v_curp_anio;
    END IF;

    -- Validar rangos numéricos del calendario antes de intentar la conversión
    IF v_curp_mes < 1 OR v_curp_mes > 12 THEN
        RAISE EXCEPTION 'Fraude de CURP: El mes de nacimiento [%] extraído de la CURP no es válido.', v_curp_mes;
    END IF;

    IF v_curp_dia < 1 OR v_curp_dia > 31 THEN
        RAISE EXCEPTION 'Fraude de CURP: El día de nacimiento [%] extraído de la CURP no es válido.', v_curp_dia;
    END IF;

    -- Intentar conversión real para detectar fechas imposibles (ej. 31 de junio)
    BEGIN
        v_fecha_nacimiento := TO_DATE(v_curp_anio || '-' || v_curp_mes || '-' || v_curp_dia, 'YYYY-MM-DD');
    EXCEPTION WHEN OTHERS THEN
        RAISE EXCEPTION 'Fraude de CURP: La combinación de fecha [%] en la CURP no existe en el calendario real.', v_curp_fecha_str;
    END;

    -- Candados cronológicos institucionales
    IF v_curp_anio < 1910 THEN
        RAISE EXCEPTION 'Error de coherencia cronológica: El año de nacimiento [%] es anterior a 1910.', v_curp_anio;
    END IF;

    IF v_fecha_nacimiento > CURRENT_DATE THEN
        RAISE EXCEPTION 'Error de coherencia cronológica: La fecha extraída de la CURP [%] pertenece al futuro.', TO_CHAR(v_fecha_nacimiento, 'DD-MM-YYYY');
    END IF;

    -- B) CRUCE DE LETRAS CONTRA EL APELLIDO PATERNO
    v_letra_paterno := SUBSTRING(v_paterno_limpio FROM 1 FOR 1);
    
    -- Busca la primera vocal interna verdadera saltándose la primera letra
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

    -- C) CRUCE DE LETRAS CONTRA EL APELLIDO MATERNO
    IF p_apellido_materno IS NOT NULL AND TRIM(p_apellido_materno) <> '' THEN
        v_letra_materno := SUBSTRING(v_materno_limpio FROM 1 FOR 1);
        IF SUBSTRING(v_curp_formateada FROM 3 FOR 1) <> v_letra_materno THEN
            RAISE EXCEPTION 'Incoherencia biográfica: La tercera letra de la CURP [%] no coincide con la inicial del Apellido Materno [%].', 
                SUBSTRING(v_curp_formateada FROM 3 FOR 1), v_letra_materno;
        END IF;
    ELSE
        -- Norma RENAPO para personas con un solo apellido
        IF SUBSTRING(v_curp_formateada FROM 3 FOR 1) <> 'X' THEN
            RAISE EXCEPTION 'Incoherencia biográfica: Al omitir el Apellido Materno, la tercera posición de la CURP debe ser "X" y se recibió "%".', 
                SUBSTRING(v_curp_formateada FROM 3 FOR 1);
        END IF;
    END IF;

    -- Preparamos el correo formateado antes de usarlo en las validaciones e INSERT
    v_correo_formateado := LOWER(TRIM(p_correo));

    -- AGREGADO: Validación estructural del correo electrónico mediante Regex
    IF v_correo_formateado IS NULL OR v_correo_formateado = '' THEN
        RAISE EXCEPTION 'Error: El correo electrónico es obligatorio.';
    END IF;

    IF v_correo_formateado !~ v_regex_correo THEN
        RAISE EXCEPTION 'Error: El correo % no tiene una estructura válida (Ej: usuario@dominio.com).', p_correo;
    END IF;

    -- NUEVA VALIDACIÓN LOCAL: Verificar que no esté ocupado y activo en la tabla Tutores
    IF EXISTS (
        SELECT 1 FROM "Tutores" WHERE "correo" = v_correo_formateado AND "estado" = TRUE
    ) THEN
        RAISE EXCEPTION 'Error: El correo electrónico % ya está vinculado a otra cuenta ACTIVA en Tutores.', v_correo_formateado;
    END IF;

    -- NUEVA VALIDACIÓN TRANSVERSAL: Verificar que no esté ocupado y activo en Receptores (Usuario o TutorAspirante)
    IF EXISTS (
        SELECT 1 FROM "Receptores" WHERE "correo_destino" = v_correo_formateado AND "estado" = TRUE
    ) THEN
        RAISE EXCEPTION 'Bloqueo de Seguridad: El correo % ya está registrado en el sistema bajo otro rol activo (Usuario o TutorAspirante).', v_correo_formateado;
    END IF;

    -- ============================================================
    -- 4. INSERCIÓN DE DATOS ESTANDARIZADOS (Mayúsculas)
    -- ============================================================
    -- Agregamos RETURNING para pescar la clave autogenerada de la tabla
    INSERT INTO "Tutores" (
        "nombre",
        "apellido_paterno",
        "apellido_materno",
        "curp_tutor",
        "telefono",
        "correo",
        "parentesco",
        "estado"
    ) VALUES (
        UPPER(TRIM(p_nombre)),
        UPPER(TRIM(p_apellido_paterno)),
        COALESCE(UPPER(TRIM(p_apellido_materno)), ''), -- Reemplazado por consistencia contra nulos 
        v_curp_formateada,
        v_telefono_limpio,                
        v_correo_formateado,           
        INITCAP(TRIM(p_parentesco)),     
        p_estado
    )
    RETURNING "claveTutor" INTO v_clave_tutor_generada; -- <--- Captura automática

    -- ============================================================
    -- 5. GENERACIÓN AUTOMÁTICA DEL RECEPTOR
    -- ============================================================
    -- Se invoca el procedimiento secundario con el tipo 'Tutor' oficial
    CALL sp_generar_receptor(
        'Tutor', 
        v_clave_tutor_generada, 
        v_correo_formateado
    );

    RAISE NOTICE 'Éxito: Registro del Tutor % creado correctamente con CURP % y Receptor Activo.', 
        UPPER(p_nombre), v_curp_formateada;

EXCEPTION
    -- El juego de cartas por si la CURP ya existe en el sistema
    WHEN unique_violation THEN
        RAISE EXCEPTION 'Error: El tutor con la CURP % ya se encuentra registrado en el sistema.', v_curp_formateada;
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error al registrar el tutor: %', SQLERRM;
END;
$$;
