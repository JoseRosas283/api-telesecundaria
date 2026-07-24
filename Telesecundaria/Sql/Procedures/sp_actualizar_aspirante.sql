CREATE OR REPLACE PROCEDURE sp_actualizar_aspirante(
    p_claveAspirante VARCHAR(18),       -- 1. Primero la PK para identificar la fila
    p_nombre VARCHAR(50),               -- 2. Datos personales en el orden del INSERT
    p_apellido_paterno VARCHAR(50),
    p_apellido_materno VARCHAR(50),
    p_curp VARCHAR(18),                 -- 3. Nueva CURP a validar
    p_escuela_procedencia VARCHAR(150), -- 4. Datos escolares al final
    p_promedio_primaria DECIMAL(3,1)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_claveTutor_vinculado VARCHAR(18);
    v_estado_activo BOOLEAN;
    v_token_valido BOOLEAN;
    v_existe_adjuncion BOOLEAN;
    
    -- Variable tipo RECORD para capturar la tupla actual completa del aspirante
    v_asp_actual RECORD;
    
    -- Variables de validación para CURP
    v_curp_formateada VARCHAR(18);
    v_regex_curp TEXT := '^[A-Z]{4}[0-9]{6}[HM][A-Z]{5}[0-9A-Z][0-9]$';

    -- VARIABLES AGREGADAS PARA LA ADUANA DE EDAD DEL ASPIRANTE
    v_anio_actual INT := EXTRACT(YEAR FROM CURRENT_DATE);
    v_asp_anio INT;
    v_asp_edad INT;

    -- VARIABLES AGREGADAS PARA EL CRUCE ALFABÉTICO DE LA CURP
    v_letra_paterno CHAR(1);
    v_vocal_paterno CHAR(1);
    v_letra_materno CHAR(1);
    v_letra_nombre CHAR(1);
BEGIN
    -- =========================================================================
    -- PASO REQUERIDO: IDENTIFICACIÓN INICIAL DEL ASPIRANTE
    -- =========================================================================
    SELECT * INTO v_asp_actual
    FROM "Aspirantes" 
    WHERE "claveAspirante" = p_claveAspirante;

    IF v_asp_actual.claveAspirante IS NULL THEN
        RAISE EXCEPTION 'Error: El aspirante con la clave % no existe.', p_claveAspirante;
    END IF;

    v_claveTutor_vinculado := v_asp_actual.claveTutorAspirante;
    v_estado_activo        := v_asp_actual.estado;

    -- =========================================================================
    -- 1. CONTROL DE ACCESO PRIMARIO: VALIDACIÓN DE SESIÓN (TOKEN DE TUTOR)
    -- =========================================================================
    SELECT EXISTS(
        SELECT 1 FROM "TokenConvocatorias" 
        WHERE "claveTutorAspirante" = v_claveTutor_vinculado 
          AND "estado_sesion" = TRUE 
          AND "fecha_expiracion" > CURRENT_TIMESTAMP
    ) INTO v_token_valido;

    IF NOT v_token_valido THEN
        RAISE EXCEPTION 'Seguridad: Sesión inválida o expirada para el tutor responsable.';
    END IF;

    -- ==========================================
    -- 2. CONTROL DE ACCESO AND SEGURIDAD OPERATIVA
    -- ==========================================

    -- A) Candado de Desactivación (Limbo)
    IF NOT v_estado_activo THEN
        RAISE EXCEPTION 'Bloqueo: El aspirante está desactivado. No se permiten cambios.';
    END IF;

    -- B) Candado Maestro: Adjunciones
    SELECT EXISTS(
        SELECT 1 FROM "Adjunciones" 
        WHERE "claveAspirante" = p_claveAspirante
    ) INTO v_existe_adjuncion;

    IF v_existe_adjuncion THEN
        RAISE EXCEPTION 'Bloqueo: El aspirante ya cuenta con documentos adjuntos. No es posible modificar la CURP ni la firma.';
    END IF;

    -- =========================================================================
    -- 2.5 FILTRO DE CAMBIOS REALES (EVALUADO SÓLO SI EL TUTOR ESTÁ AUTENTICADO)
    -- =========================================================================
    v_curp_formateada := UPPER(TRIM(p_curp));

    IF ROW(v_asp_actual.curp, v_asp_actual.nombre, v_asp_actual.apellido_paterno, 
           v_asp_actual.apellido_materno, v_asp_actual.promedio_primaria, v_asp_actual.escuela_procedencia)
       IS NOT DISTINCT FROM
       ROW(v_curp_formateada, UPPER(TRIM(p_nombre)), UPPER(TRIM(p_apellido_paterno)), 
           NULLIF(UPPER(TRIM(p_apellido_materno)), ''), p_promedio_primaria, TRIM(p_escuela_procedencia))
    THEN
        RAISE NOTICE 'Éxito: Operación completada. Los datos enviados para el aspirante % son idénticos a los actuales; no se realizaron cambios en la base de datos.', p_claveAspirante;
        RETURN; 
    END IF;

    -- ==========================================
    -- 3. VALIDACIONES LÓGICAS AND DE IDENTIDAD (CURP)
    -- ==========================================
    
    -- A) Validación Estricta de CURP
    IF v_curp_formateada IS NULL OR LENGTH(v_curp_formateada) != 18 THEN
        RAISE EXCEPTION 'Error: La CURP debe tener exactamente 18 caracteres.';
    END IF;

    IF v_curp_formateada !~ v_regex_curp THEN
        RAISE EXCEPTION 'Error: La CURP % no tiene un formato oficial válido.', v_curp_formateada;
    END IF;

    -- Validar Unicidad: Que no pertenezca a OTRO aspirante
    IF EXISTS (
        SELECT 1 FROM "Aspirantes" 
        WHERE curp = v_curp_formateada 
          AND "claveAspirante" != p_claveAspirante
    ) THEN
        RAISE EXCEPTION 'Error: La CURP % ya está registrada con otro aspirante.', v_curp_formateada;
    END IF;

    -- B) Validaciones de Campos de Texto
    IF p_nombre IS NULL OR TRIM(p_nombre) = '' THEN
        RAISE EXCEPTION 'Error: El nombre es obligatorio.';
    END IF;

    IF p_apellido_paterno IS NULL OR TRIM(p_apellido_paterno) = '' THEN
        RAISE EXCEPTION 'Error: El apellido paterno es obligatorio.';
    END IF;

    IF p_escuela_procedencia IS NULL OR TRIM(p_escuela_procedencia) = '' THEN
        RAISE EXCEPTION 'Error: La escuela de procedencia es obligatoria.';
    END IF;

    -- C) Validación de Promedio
    IF p_promedio_primaria IS NULL OR p_promedio_primaria <= 0 OR p_promedio_primaria > 10.0 THEN
        RAISE EXCEPTION 'Error: El promedio (%) no es válido.', p_promedio_primaria;
    END IF;

    -- ============================================================
    -- CANDADO ADICIONAL 1: ADUANA CRONOLÓGICA (11 A 15 AÑOS)
    -- ============================================================
    v_asp_anio := SUBSTRING(v_curp_formateada FROM 5 FOR 2)::INT;
    
    -- Determinar el siglo de forma dinámica comparando con los últimos dos dígitos del año actual
    IF v_asp_anio <= (v_anio_actual % 100) THEN
        v_asp_anio := 2000 + v_asp_anio;
    ELSE
        v_asp_anio := 1900 + v_asp_anio;
    END IF;

    v_asp_edad := v_anio_actual - v_asp_anio;

    IF v_asp_edad < 11 THEN
        RAISE EXCEPTION 'Rechazo por edad: La CURP proporcionada calcula % anos. El aspirante es demasiado joven para secundaria.', v_asp_edad;
    END IF;

    IF v_asp_edad > 15 THEN
        RAISE EXCEPTION 'Rechazo por edad: La CURP proporcionada calcula % anos. Supera el limite de tolerancia de 3 anos (Maximo 15).', v_asp_edad;
    END IF;

    -- ============================================================
    -- CANDADO ADICIONAL 2: ADUANA BIOGRÁFICA (NOMBRES VS NUEVA CURP) - AJUSTADO PARA TILDES
    -- ============================================================
    -- Posición 1: Inicial del Apellido Paterno
    v_letra_paterno := SUBSTRING(TRANSLATE(UPPER(TRIM(p_apellido_paterno)), 'ÁÉÍÓÚÜ', 'AEIOUU') FROM 1 FOR 1);
    IF SUBSTRING(v_curp_formateada FROM 1 FOR 1) <> v_letra_paterno THEN
        RAISE EXCEPTION 'Incoherencia de identidad: La primera letra de la CURP no coincide con la inicial del Apellido Paterno.';
    END IF;

    -- Posición 2: Primera vocal interna del Apellido Paterno
    SELECT SUBSTRING(TRANSLATE(SUBSTRING(UPPER(TRIM(p_apellido_paterno)) FROM 2), 'ÁÉÍÓÚÜ', 'AEIOUU') FROM '[AEIOU]') INTO v_vocal_paterno;
    IF v_vocal_paterno IS NOT NULL AND SUBSTRING(v_curp_formateada FROM 2 FOR 1) <> v_vocal_paterno THEN
        RAISE EXCEPTION 'Incoherencia de identidad: La segunda posición de la CURP debe ser la primera vocal interna del Apellido Paterno.';
    END IF;

    -- Posición 3: Inicial del Apellido Materno o 'X' si no tiene
    IF p_apellido_materno IS NOT NULL AND TRIM(p_apellido_materno) <> '' THEN
        v_letra_materno := SUBSTRING(TRANSLATE(UPPER(TRIM(p_apellido_materno)), 'ÁÉÍÓÚÜ', 'AEIOUU') FROM 1 FOR 1);
        IF SUBSTRING(v_curp_formateada FROM 3 FOR 1) <> v_letra_materno THEN
            RAISE EXCEPTION 'Incoherencia de identidad: La tercera letra de la CURP no coincide con la inicial del Apellido Materno.';
        END IF;
    ELSE
        IF SUBSTRING(v_curp_formateada FROM 3 FOR 1) <> 'X' THEN
            RAISE EXCEPTION 'Incoherencia de identidad: Al no especificar Apellido Materno, la tercera posición de la CURP debe ser una letra X.';
        END IF;
    END IF;

    -- Posición 4: Inicial del Primer Nombre
    v_letra_nombre := SUBSTRING(TRANSLATE(UPPER(TRIM(p_nombre)), 'ÁÉÍÓÚÜ', 'AEIOUU') FROM 1 FOR 1);
    IF SUBSTRING(v_curp_formateada FROM 4 FOR 1) <> v_letra_nombre THEN
        RAISE EXCEPTION 'Incoherencia de identidad: La cuarta letra de la CURP no coincide con la inicial del Nombre.';
    END IF;

    -- =========================================================================
    -- VALIDACIÓN DE DUPLICIDAD INSTITUCIONAL EN EXPEDIENTES
    -- =========================================================================
    IF EXISTS (
        SELECT 1 FROM "Expedientes" 
        WHERE curp = v_curp_formateada 
          AND "tipo_titular" = 'Alumno'
    ) THEN
        RAISE EXCEPTION 'Rechazo por Duplicidad Institucional: El titular de la CURP % ya se encuentra registrado como un Alumno vigente en la institución.', v_curp_formateada;
    END IF;

    -- ==========================================
    -- 4. PROCESAMIENTO FINAL
    -- ==========================================
    UPDATE "Aspirantes" SET
        curp = v_curp_formateada,
        nombre = UPPER(TRIM(p_nombre)),
        apellido_paterno = UPPER(TRIM(p_apellido_paterno)),
        apellido_materno = CASE 
                            WHEN p_apellido_materno IS NULL OR TRIM(p_apellido_materno) = '' THEN NULL 
                            ELSE UPPER(TRIM(p_apellido_materno)) 
                           END,
        promedio_primaria = p_promedio_primaria,
        escuela_procedencia = TRIM(p_escuela_procedencia)
    WHERE "claveAspirante" = p_claveAspirante;

    RAISE NOTICE 'Éxito: Datos y CURP del aspirante actualizados correctamente.';

END;
$$;