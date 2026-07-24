CREATE OR REPLACE PROCEDURE sp_actualizar_tutor(
    p_claveTutor VARCHAR(18),
    p_nombre VARCHAR(50),
    p_apellido_paterno VARCHAR(50),
    p_apellido_materno VARCHAR(50),
    p_curp_tutor VARCHAR(18),
    p_telefono VARCHAR(15),
    p_correo VARCHAR(100),
    p_parentesco VARCHAR(50)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_telefono_limpio VARCHAR(15);
    v_correo_formateado VARCHAR(100);
    v_curp_formateada VARCHAR(18);
    v_regex_correo TEXT := '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$';
    
    -- Variables para la aduana biométrica de la CURP
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
    
    -- Variable para verificar la existencia de alumnos vinculados en la tabla histórica
    v_tiene_alumnos BOOLEAN;
    -- Variable tipo RECORD para capturar la tupla actual completa del tutor
    v_tut_actual RECORD;
BEGIN
    -- ============================================================
    -- 1. VALIDACIÓN DE EXISTENCIA FÍSICA
    -- ============================================================
    SELECT * INTO v_tut_actual
    FROM "Tutores"
    WHERE "claveTutor" = p_claveTutor;

    -- Si la clave no existe en la tabla, se detiene de inmediato
    IF v_tut_actual.claveTutor IS NULL THEN
        RAISE EXCEPTION 'Error de Modificación: El tutor con clave % no existe en la base de datos.', p_claveTutor;
    END IF;

    -- ============================================================
    -- 2. CANDADO DE INTEGRIDAD: BLOQUEO POR ALUMNOS VINCULADOS
    -- ============================================================
    SELECT EXISTS (
        SELECT 1 FROM "TutoresAlumnos" WHERE "claveTutor" = p_claveTutor
    ) INTO v_tiene_alumnos;

    -- Si se detecta un lazo con un alumno, se congela cualquier intento de UPDATE
    IF v_tiene_alumnos THEN
        RAISE EXCEPTION 'Bloqueo de Integridad: El tutor % no puede ser modificado porque ya se encuentra vinculado (o estuvo vinculado) a alumnos en la tabla TutoresAlumnos.', p_claveTutor;
    END IF;

    -- =========================================================================
    -- 2.5 FILTRO DE CAMBIOS REALES (ADUANA INTELIGENTE UNIFICADA)
    -- =========================================================================
    v_telefono_limpio   := TRIM(p_telefono);
    v_correo_formateado := LOWER(TRIM(p_correo));
    v_curp_formateada   := UPPER(TRIM(p_curp_tutor));

    IF ROW(v_tut_actual.nombre, v_tut_actual.apellido_paterno, v_tut_actual.apellido_materno, 
           v_tut_actual.curp_tutor, v_tut_actual.telefono, v_tut_actual.correo, v_tut_actual.parentesco)
       IS NOT DISTINCT FROM
       ROW(UPPER(TRIM(p_nombre)), UPPER(TRIM(p_apellido_paterno)), UPPER(TRIM(p_apellido_materno)), 
           v_curp_formateada, v_telefono_limpio, v_correo_formateado, INITCAP(TRIM(p_parentesco)))
    THEN
        RAISE NOTICE 'Éxito: Operación completada. Los datos enviados para el tutor % son idénticos a los actuales; no se realizaron cambios en la base de datos.', p_claveTutor;
        RETURN; 
    END IF;

    -- ============================================================
    -- 3. VALIDACIÓN DE CAMPOS OBLIGATORIOS (Nulos y Vacíos)
    -- ============================================================
    IF p_nombre IS NULL OR TRIM(p_nombre) = '' THEN
        RAISE EXCEPTION 'Error: El nombre del tutor es obligatorio.';
    END IF;

    IF p_apellido_paterno IS NULL OR TRIM(p_apellido_paterno) = '' THEN
        RAISE EXCEPTION 'Error: El apellido paterno del tutor es obligatorio.';
    END IF;

    IF p_curp_tutor IS NULL OR TRIM(p_curp_tutor) = '' THEN
        RAISE EXCEPTION 'Error: La CURP del tutor es obligatoria.';
    END IF;

    IF p_telefono IS NULL OR TRIM(p_telefono) = '' THEN
        RAISE EXCEPTION 'Error: El teléfono de contacto del tutor es obligatorio.';
    END IF;

    IF p_parentesco IS NULL OR TRIM(p_parentesco) = '' THEN
        RAISE EXCEPTION 'Error: El parentesco es obligatorio (Ej: Padre, Madre, Tutor Legal).';
    END IF;

    -- ============================================================
    -- 4. VALIDACIÓN DE RESTRICCIONES DEL TELÉFONO
    -- ============================================================
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
    -- 5. VALIDACIÓN DE FORMATO DE CURP Y UNICIDAD
    -- ============================================================
    IF LENGTH(v_curp_formateada) <> 18 THEN
        RAISE EXCEPTION 'Error: La CURP del tutor debe tener exactamente 18 caracteres.';
    END IF;

    IF v_curp_formateada !~ '^[A-Z]{4}[0-9]{6}[HM][A-Z]{5}[0-9A-Z][0-9]$' THEN
        RAISE EXCEPTION 'Error: La CURP % no tiene un formato oficial válido.', v_curp_formateada;
    END IF;

    -- Si el usuario intentó modificar la CURP actual, validamos que no colisione con otro tutor
    IF v_curp_formateada <> v_tut_actual.curp_tutor THEN
        IF EXISTS (SELECT 1 FROM "Tutores" WHERE "curp_tutor" = v_curp_formateada) THEN
            RAISE EXCEPTION 'Error: La CURP % ya está registrada en otro tutor diferente.', v_curp_formateada;
        END IF;
    END IF;

    -- ============================================================
    -- 5.1 ADUANA BIOMÉTRICA DE CURP (CRUCE ESTRICTO DE IDENTIDAD)
    -- ============================================================
    -- Eliminación de tildes en memoria para evitar rechazos por acentos ortográficos
    v_paterno_limpio := TRANSLATE(UPPER(TRIM(p_apellido_paterno)), 'ÁÉÍÓÚÜ', 'AEIOUU');
    v_materno_limpio := TRANSLATE(UPPER(TRIM(p_apellido_materno)), 'ÁÉÍÓÚÜ', 'AEIOUU');

    -- A) Extracción y Validación de la Fecha de Nacimiento
    v_curp_fecha_str := SUBSTRING(v_curp_formateada FROM 5 FOR 6);
    v_curp_anio := SUBSTRING(v_curp_fecha_str FROM 1 FOR 2)::INT;
    v_curp_mes := SUBSTRING(v_curp_fecha_str FROM 3 FOR 2)::INT;
    v_curp_dia := SUBSTRING(v_curp_fecha_str FROM 5 FOR 2)::INT;

    -- Lógica de cambio de siglo oficial (RENAPO) basada en el año en curso
    IF v_curp_anio <= (v_anio_actual - 2000) THEN
        v_curp_anio := 2000 + v_curp_anio;
    ELSE
        v_curp_anio := 1900 + v_curp_anio;
    END IF;

    IF v_curp_mes < 1 OR v_curp_mes > 12 THEN
        RAISE EXCEPTION 'Fraude de CURP: El mes de nacimiento [%] extraído de la CURP no es válido.', v_curp_mes;
    END IF;

    IF v_curp_dia < 1 OR v_curp_dia > 31 THEN
        RAISE EXCEPTION 'Fraude de CURP: El día de nacimiento [%] extraído de la CURP no es válido.', v_curp_dia;
    END IF;

    -- Control de fechas inexistentes en el calendario real
    BEGIN
        v_fecha_nacimiento := TO_DATE(v_curp_anio || '-' || v_curp_mes || '-' || v_curp_dia, 'YYYY-MM-DD');
    EXCEPTION WHEN OTHERS THEN
        RAISE EXCEPTION 'Fraude de CURP: La combinación de fecha [%] en la CURP no existe en el calendario real.', v_curp_fecha_str;
    END;

    -- Filtros cronológicos
    IF v_curp_anio < 1910 THEN
        RAISE EXCEPTION 'Error de coherencia cronológica: El año de nacimiento [%] es anterior a 1910.', v_curp_anio;
    END IF;

    IF v_fecha_nacimiento > CURRENT_DATE THEN
        RAISE EXCEPTION 'Error de coherencia cronológica: La fecha extraída de la CURP [%] pertenece al futuro.', TO_CHAR(v_fecha_nacimiento, 'DD-MM-YYYY');
    END IF;

    -- B) Cruce de Letras de la Identidad - Apellido Paterno
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

    -- C) Cruce de Letras de la Identidad - Apellido Materno
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
    -- 6. VALIDACIÓN ESTRUCTURAL Y TRANSVERSAL DEL CORREO
    -- ============================================================
    IF v_correo_formateado !~ v_regex_correo THEN
        RAISE EXCEPTION 'Error: El correo % no tiene una estructura válida (Ej: usuario@dominio.com).', p_correo;
    END IF;

    -- Si se detecta un cambio en el correo electrónico, aplicamos candados de unicidad
    IF v_correo_formateado <> v_tut_actual.correo THEN
        -- A) Unicidad Local en Tutores Activos
        IF EXISTS (
            SELECT 1 FROM "Tutores" WHERE "correo" = v_correo_formateado AND "estado" = TRUE AND "claveTutor" <> p_claveTutor
        ) THEN
            RAISE EXCEPTION 'Error: El correo electrónico % ya está vinculado a otra cuenta ACTIVA en Tutores.', v_correo_formateado;
        END IF;

        -- B) Unicidad Transversal en Receptores Activos 
        IF EXISTS (
            SELECT 1 FROM "Receptores"  
            WHERE "correo_destino" = v_correo_formateado 
              AND "estado" = TRUE 
              AND ("claveTutor" IS NULL OR "claveTutor" <> p_claveTutor)
        ) THEN
            RAISE EXCEPTION 'Bloqueo de Seguridad: El correo % ya está registrado en el sistema bajo otro rol activo (Usuario o TutorAspirante).', v_correo_formateado;
        END IF;
    END IF;

    -- ============================================================
    -- 7. ACTUALIZACIÓN DE LA TABLA TUTORES
    -- ============================================================
    UPDATE "Tutores" SET
        "nombre"           = UPPER(TRIM(p_nombre)),
        "apellido_paterno" = UPPER(TRIM(p_apellido_paterno)),
        "apellido_materno" = COALESCE(UPPER(TRIM(p_apellido_materno)), ''), -- Control de nulos homologado
        "curp_tutor"       = v_curp_formateada,
        "telefono"         = v_telefono_limpio,
        "correo"           = v_correo_formateado,
        "parentesco"       = INITCAP(TRIM(p_parentesco))
    WHERE "claveTutor" = p_claveTutor;

    -- ============================================================
    -- 8. SINCRONIZACIÓN AUTOMÁTICA CON LA TABLA RECEPTORES
    -- ============================================================
    IF v_correo_formateado <> v_tut_actual.correo THEN
        CALL sp_actualizar_receptor_tutor(p_claveTutor, v_correo_formateado);
    END IF;

    RAISE NOTICE 'Éxito: Datos del Tutor % actualizados correctamente de forma integral.', UPPER(p_nombre);

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Fallo en la actualización del tutor: %', SQLERRM;
END;
$$;