CREATE OR REPLACE PROCEDURE sp_actualizar_expediente(
    p_claveExpediente VARCHAR(18),
    p_nuevo_nombre VARCHAR(80),
    p_nuevo_paterno VARCHAR(80),
    p_nuevo_materno VARCHAR(80),
    p_nueva_curp VARCHAR(18)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_curp_actual VARCHAR(18);
    v_tiene_documentos BOOLEAN;
    
    -- Variable tipo RECORD para capturar la tupla actual completa del expediente
    v_exp_actual RECORD;

    -- NUEVAS VARIABLES PARA ADUANA BIOMÉTRICA DE CURP EN ACTUALIZACIÓN
    v_curp_formateada VARCHAR(18);
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
    -- ==========================================================
    -- 1. PRIMERO: VERIFICAR EXISTENCIA
    -- ==========================================================
    SELECT *, 
           EXISTS (SELECT 1 FROM "Documentos" WHERE "claveExpediente" = p_claveExpediente)
    INTO v_exp_actual
    FROM "Expedientes"
    WHERE "claveExpediente" = p_claveExpediente;

    -- Si el SELECT no arrojó resultados, la clave no existe
    IF v_exp_actual."claveExpediente" IS NULL THEN
        RAISE EXCEPTION 'Error: El expediente % no existe en la base de datos.', p_claveExpediente;
    END IF;

    -- Mapeo de tus variables de control originales
    v_curp_actual      := v_exp_actual.curp;
    v_tiene_documentos := v_exp_actual.exists;

    -- =========================================================================
    -- 1.5 FILTRO DE CAMBIOS REALES (ESTRATEGIA UNIFICADA - SIN TIPO)
    -- =========================================================================
    -- Eliminamos tipo_titular de la ecuación espejo
    IF ROW(v_exp_actual.nombre, v_exp_actual.apellido_paterno, v_exp_actual.apellido_materno, 
           v_exp_actual.curp)
       IS NOT DISTINCT FROM
       ROW(UPPER(TRIM(p_nuevo_nombre)), UPPER(TRIM(p_nuevo_paterno)), UPPER(TRIM(p_nuevo_materno)), 
           UPPER(TRIM(p_nueva_curp)))
    THEN
        RAISE NOTICE 'Éxito: Operación completada. Los datos enviados para el expediente % son idénticos a los actuales; no se realizaron cambios en la base de datos.', p_claveExpediente;
        RETURN; 
    END IF;

    -- ==========================================================
    -- 2. SEGUNDO: VERIFICAR BLOQUEO POR DOCUMENTOS
    -- ==========================================================
    IF v_tiene_documentos THEN
        RAISE EXCEPTION 'Bloqueo: El expediente % ya cuenta con documentos cargados y no puede ser modificado.', p_claveExpediente;
    END IF;

    -- ==========================================================
    -- 3. TERCERO: VALIDAR CAMPOS OBLIGATORIOS
    -- ==========================================================
    IF p_nuevo_nombre IS NULL OR TRIM(p_nuevo_nombre) = '' THEN
        RAISE EXCEPTION 'Error: El Nombre es requerido para la actualización.';
    END IF;

    IF p_nuevo_paterno IS NULL OR TRIM(p_nuevo_paterno) = '' THEN
        RAISE EXCEPTION 'Error: El Apellido Paterno es requerido para la actualización.';
    END IF;

    -- ==========================================================
    -- 4. CUARTO: VALIDAR CURP (Formato y Disponibilidad)
    -- ==========================================================
    v_curp_formateada := UPPER(TRIM(p_nueva_curp));

    IF v_curp_formateada !~ '^[A-Z]{4}[0-9]{6}[HM][A-Z]{5}[0-9A-Z]{2}$' THEN
        RAISE EXCEPTION 'Error: La CURP % no cumple con el formato legal.', v_curp_formateada;
    END IF;

    -- Si cambió la CURP, verificar que no la tenga otro expediente
    IF v_curp_formateada <> v_curp_actual THEN
        IF EXISTS (SELECT 1 FROM "Expedientes" WHERE curp = v_curp_formateada) THEN
            RAISE EXCEPTION 'Error: La CURP % ya está registrada en otro expediente diferente.', v_curp_formateada;
        END IF;
    END IF;

    -- ============================================================
    -- 4.5 NUEVO: ADUANA BIOMÉTRICA DE CURP Y EDAD PARA ACTUALIZACIONES
    -- ============================================================
    -- Limpieza de acentos en memoria para el cruce biográfico
    v_paterno_limpio := TRANSLATE(UPPER(TRIM(p_nuevo_paterno)), 'ÁÉÍÓÚÜ', 'AEIOUU');
    v_materno_limpio := TRANSLATE(UPPER(TRIM(p_nuevo_materno)), 'ÁÉÍÓÚÜ', 'AEIOUU');

    -- A) Extracción e interpretación cronológica de la nueva CURP
    v_curp_fecha_str := SUBSTRING(v_curp_formateada FROM 5 FOR 6);
    v_curp_anio := SUBSTRING(v_curp_fecha_str FROM 1 FOR 2)::INT;
    v_curp_mes := SUBSTRING(v_curp_fecha_str FROM 3 FOR 2)::INT;
    v_curp_dia := SUBSTRING(v_curp_fecha_str FROM 5 FOR 2)::INT;

    -- Lógica de Siglo RENAPO
    IF v_curp_anio <= (v_anio_actual - 2000) THEN
        v_curp_anio := 2000 + v_curp_anio;
    ELSE
        v_curp_anio := 1900 + v_curp_anio;
    END IF;

    -- Validaciones preventivas de rango del calendario
    IF v_curp_mes < 1 OR v_curp_mes > 12 THEN
        RAISE EXCEPTION 'Fraude de CURP: El mes de nacimiento [%] en la nueva CURP no es válido.', v_curp_mes;
    END IF;

    IF v_curp_dia < 1 OR v_curp_dia > 31 THEN
        RAISE EXCEPTION 'Fraude de CURP: El día de nacimiento [%] en la nueva CURP no es válido.', v_curp_dia;
    END IF;

    -- Verificación de existencia real en el calendario
    BEGIN
        v_fecha_nacimiento := TO_DATE(v_curp_anio || '-' || v_curp_mes || '-' || v_curp_dia, 'YYYY-MM-DD');
    EXCEPTION WHEN OTHERS THEN
        RAISE EXCEPTION 'Fraude de CURP: La fecha [%] en la nueva CURP no existe en el calendario real.', v_curp_fecha_str;
    END;

    -- Candados cronológicos base
    IF v_curp_anio < 1910 THEN
        RAISE EXCEPTION 'Error de coherencia cronológica: El año de nacimiento [%] es anterior a 1910.', v_curp_anio;
    END IF;

    IF v_fecha_nacimiento > CURRENT_DATE THEN
        RAISE EXCEPTION 'Error de coherencia cronológica: La fecha de la nueva CURP [%] pertenece al futuro.', TO_CHAR(v_fecha_nacimiento, 'DD-MM-YYYY');
    END IF;

    -- B) Cálculo matemático de edad y aplicación de políticas institucionales
    v_edad_titular := EXTRACT(YEAR FROM AGE(CURRENT_DATE, v_fecha_nacimiento));

    -- Evaluamos según el tipo original guardado en la base de datos (v_exp_actual.tipo_titular)
    IF v_exp_actual.tipo_titular = 'Empleado' THEN
        IF v_edad_titular < 18 THEN
            RAISE EXCEPTION 'Bloqueo Laboral: La nueva CURP da una edad de % años. No se permiten Empleados menores de 18 años.', v_edad_titular;
        ELSIF v_edad_titular > 75 THEN
            RAISE EXCEPTION 'Bloqueo Laboral: La nueva CURP da una edad de % años. Supera el límite de contratación de 75 años.', v_edad_titular;
        END IF;
    ELSIF v_exp_actual.tipo_titular = 'Alumno' THEN
        IF v_edad_titular < 12 OR v_edad_titular > 18 THEN
            RAISE EXCEPTION 'Bloqueo Escolar: La nueva CURP da una edad de % años. El rango permitido para Alumnos es de 12 a 18 años.', v_edad_titular;
        END IF;
    END IF;

    -- C) Cruce biográfico: Primer Apellido
    v_letra_paterno := SUBSTRING(v_paterno_limpio FROM 1 FOR 1);
    v_vocal_paterno := SUBSTRING(v_paterno_limpio FROM 2 FOR 1);
    IF v_vocal_paterno !~ '[AEIOU]' THEN
        SELECT (regexp_matches(SUBSTRING(v_paterno_limpio FROM 2), '[AEIOU]'))[1] INTO v_vocal_paterno;
    END IF;

    IF SUBSTRING(v_curp_formateada FROM 1 FOR 1) <> v_letra_paterno THEN
        RAISE EXCEPTION 'Incoherencia biográfica: La primera letra de la CURP [%] no coincide con el nuevo Apellido Paterno [%].', 
            SUBSTRING(v_curp_formateada FROM 1 FOR 1), v_letra_paterno;
    END IF;

    IF v_vocal_paterno IS NOT NULL AND SUBSTRING(v_curp_formateada FROM 2 FOR 1) <> v_vocal_paterno THEN
        RAISE EXCEPTION 'Incoherencia biográfica: La segunda letra de la CURP [%] debe ser la primera vocal del nuevo Apellido Paterno [%].', 
            SUBSTRING(v_curp_formateada FROM 2 FOR 1), v_vocal_paterno;
    END IF;

    -- D) Cruce biográfico: Segundo Apellido
    IF p_nuevo_materno IS NOT NULL AND TRIM(p_nuevo_materno) <> '' THEN
        v_letra_materno := SUBSTRING(v_materno_limpio FROM 1 FOR 1);
        IF SUBSTRING(v_curp_formateada FROM 3 FOR 1) <> v_letra_materno THEN
            RAISE EXCEPTION 'Incoherencia biográfica: La tercera letra de la CURP [%] no coincide con el nuevo Apellido Materno [%].', 
                SUBSTRING(v_curp_formateada FROM 3 FOR 1), v_letra_materno;
        END IF;
    ELSE
        IF SUBSTRING(v_curp_formateada FROM 3 FOR 1) <> 'X' THEN
            RAISE EXCEPTION 'Incoherencia biográfica: Al omitir el Apellido Materno, la tercera posición de la CURP debe ser "X" y se recibió "%".', 
                SUBSTRING(v_curp_formateada FROM 3 FOR 1);
        END IF;
    END IF;

    -- ==========================================================
    -- 5. FINALMENTE: EJECUTAR ACTUALIZACIÓN DEL EXPEDIENTE
    -- ==========================================================
    -- Nota: Al remover p_nuevo_tipo, la columna tipo_titular queda intacta en la base de datos
    UPDATE "Expedientes" SET
        nombre = UPPER(TRIM(p_nuevo_nombre)),
        apellido_paterno = UPPER(TRIM(p_nuevo_paterno)),
        apellido_materno = COALESCE(UPPER(TRIM(p_nuevo_materno)), ''), -- Control de vacíos homologado
        curp = v_curp_formateada
    WHERE "claveExpediente" = p_claveExpediente;

    RAISE NOTICE 'Éxito: Expediente % actualizado correctamente.', p_claveExpediente;

END;
$$;