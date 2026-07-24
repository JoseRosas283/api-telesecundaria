CREATE OR REPLACE PROCEDURE sp_insertar_aspirante(
    p_nombre VARCHAR(50),
    p_apellido_paterno VARCHAR(50),
    p_apellido_materno VARCHAR(50),
    p_curp VARCHAR(18),
    p_escuela_procedencia VARCHAR(150),
    p_promedio_primaria DECIMAL(3,1),
    p_discapacidad_txt VARCHAR(10),
    p_nombre_enfermedad VARCHAR(100),
    p_hermano_txt VARCHAR(10),
    p_curp_hermano VARCHAR(18),
    p_claveTutorAspirante VARCHAR(18)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_clave_conv_detectada VARCHAR(18);
    v_tutor_existe BOOLEAN;
    v_tiene_discapacidad BOOLEAN;
    v_hermano_plantel BOOLEAN;
    v_enfermedad_final VARCHAR(100);
    v_curp_hermano_final VARCHAR(18);
    v_curp_formateada VARCHAR(18);
    v_regex_curp TEXT := '^[A-Z]{4}[0-9]{6}[HM][A-Z]{5}[0-9A-Z][0-9]$';
    
    v_apellido_p_exp VARCHAR(80);
    v_apellido_m_exp VARCHAR(80);
    v_token_activo BOOLEAN;

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
    -- 1. LIMPIEZA Y FORMATEO INICIAL
    v_curp_formateada := UPPER(TRIM(p_curp));

    -- 2. VALIDACIONES DE CAMPOS OBLIGATORIOS (INDIVIDUALES)
    IF p_nombre IS NULL OR TRIM(p_nombre) = '' THEN
        RAISE EXCEPTION 'El nombre del aspirante es obligatorio.';
    END IF;

    IF p_apellido_paterno IS NULL OR TRIM(p_apellido_paterno) = '' THEN
        RAISE EXCEPTION 'El apellido paterno es obligatorio.';
    END IF;

    IF p_curp IS NULL OR TRIM(p_curp) = '' THEN
        RAISE EXCEPTION 'La CURP es obligatoria para el registro.';
    END IF;

    IF p_escuela_procedencia IS NULL OR TRIM(p_escuela_procedencia) = '' THEN
        RAISE EXCEPTION 'Debe especificar la escuela de procedencia.';
    END IF;

    -- 3. VALIDACIÓN DE DATOS NUMÉRICOS
    IF p_promedio_primaria IS NULL THEN
        RAISE EXCEPTION 'El promedio de primaria no puede ser nulo.';
    END IF;

    IF p_promedio_primaria <= 0 OR p_promedio_primaria > 10 THEN
        RAISE EXCEPTION 'El promedio (%) no es válido. Debe estar entre 0.1 y 10.0.', p_promedio_primaria;
    END IF;

    -- ============================================================
    --  NUEVA UBICACIÓN: 4. VALIDACIÓN DE CURP (FORMATO REGEX)
    -- ============================================================
    IF v_curp_formateada !~ v_regex_curp THEN
        RAISE EXCEPTION 'La CURP % no tiene un formato válido (Estructura oficial).', v_curp_formateada;
    END IF;

    -- ============================================================
    --  NUEVO ORDEN: VALIDACIÓN DE EXISTENCIA DEL TUTOR BASE
    -- ============================================================
    SELECT EXISTS(SELECT 1 FROM "TutorAspirante" WHERE "claveTutorAspirante" = p_claveTutorAspirante) 
    INTO v_tutor_existe;

    IF NOT v_tutor_existe THEN
        RAISE EXCEPTION 'El tutor con clave % no existe. Registre al tutor primero.', p_claveTutorAspirante;
    END IF;

    -- ============================================================
    -- VALIDACIÓN DE TOKEN ACTIVO (AHORA ABAJO DE LA EXISTENCIA DEL TUTOR)
    -- ============================================================
    SELECT EXISTS(
        SELECT 1 FROM "TokenConvocatorias" 
        WHERE "claveTutorAspirante" = p_claveTutorAspirante 
        AND "estado_sesion" = TRUE 
        AND "fecha_expiracion" > CURRENT_TIMESTAMP
    ) INTO v_token_activo;

    IF NOT v_token_activo THEN
        RAISE EXCEPTION 'Acceso denegado: El tutor no cuenta con un token de sesión activo o vigente.';
    END IF;

    -- ============================================================
    -- CANDADO 1: ADUANA CRONOLÓGICA
    -- ============================================================
    v_asp_anio := SUBSTRING(v_curp_formateada FROM 5 FOR 2)::INT;
    
    -- Determinar el siglo de forma dinámica comparando con los últimos dos dígitos del año actual
    IF v_asp_anio <= (v_anio_actual % 100) THEN
        v_asp_anio := 2000 + v_asp_anio;
    ELSE
        v_asp_anio := 1900 + v_asp_anio;
    END IF;

    v_asp_edad := v_anio_actual - v_asp_anio;

    -- Validaciones de rango de edad reglamentario para secundaria
    IF v_asp_edad < 11 THEN
        RAISE EXCEPTION 'Rechazo por edad: El aspirante tiene % anos. Es demasiado joven para ingresar a secundaria.', v_asp_edad;
    END IF;

    IF v_asp_edad > 15 THEN
        RAISE EXCEPTION 'Rechazo por edad: El aspirante tiene % anos. Supera el limite de tolerancia de 3 anos (Maximo 15).', v_asp_edad;
    END IF;

    -- ============================================================
    -- CANDADO 2: ADUANA BIOGRÁFICA (NOMBRE Y APELLIDOS VS CURP) - AJUSTADO PARA TILDES
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
            RAISE EXCEPTION 'Incoherencia de identidad: Al no tener Apellido Materno, la tercera posición de la CURP debe ser una letra X.';
        END IF;
    END IF;

    -- Posición 4: Inicial del Primer Nombre
    v_letra_nombre := SUBSTRING(TRANSLATE(UPPER(TRIM(p_nombre)), 'ÁÉÍÓÚÜ', 'AEIOUU') FROM 1 FOR 1);
    IF SUBSTRING(v_curp_formateada FROM 4 FOR 1) <> v_letra_nombre THEN
        RAISE EXCEPTION 'Incoherencia de identidad: La cuarta letra de la CURP no coincide con la inicial del Nombre.';
    END IF;

    -- ============================================================
    -- VALIDACIÓN DE EXISTENCIA EN BD (SÓLO SI PASÓ LOS CANDADOS ANTERIORES)
    -- ============================================================
    IF EXISTS (SELECT 1 FROM "Aspirantes" WHERE "curp" = v_curp_formateada) THEN
        RAISE EXCEPTION 'Ya existe un registro con la CURP %.', v_curp_formateada;
    END IF;

    -- Si no existe en la actual, validamos que no sea ya nuestro Alumno inscrito
    IF EXISTS (
        SELECT 1 FROM "Expedientes" 
        WHERE "curp" = v_curp_formateada 
          AND "tipo_titular" = 'Alumno'
    ) THEN
        RAISE EXCEPTION 'Rechazo por Duplicidad Institucional: El titular de la CURP % ya se encuentra registrado como un Alumno vigente en la institución.', v_curp_formateada;
    END IF;

    -- 5. AUTODETECCIÓN DE CONVOCATORIA ACTIVA
    SELECT "claveConvocatoria" INTO v_clave_conv_detectada 
    FROM "Convocatorias" 
    WHERE "activacion" = TRUE;

    IF v_clave_conv_detectada IS NULL THEN
        RAISE EXCEPTION 'No se admiten registros: No existe ninguna convocatoria abierta en este momento.';
    END IF;

    -- ==========================================
    -- 7. LÓGICA DE SALUD (CON BLOQUEO POR INCOHERENCIA)
    -- ==========================================
    IF p_discapacidad_txt = 'No tiene' AND (p_nombre_enfermedad IS NOT NULL AND TRIM(p_nombre_enfermedad) <> '') THEN
        RAISE EXCEPTION 'Incoherencia de datos: No puede proporcionar información de enfermedad si marcó que "No tiene" discapacidad.';
    END IF;

    IF p_discapacidad_txt = 'Si tiene' THEN
        v_tiene_discapacidad := TRUE;
        IF p_nombre_enfermedad IS NULL OR TRIM(p_nombre_enfermedad) = '' THEN
            RAISE EXCEPTION 'Campo obligatorio: Especifique la enfermedad o discapacidad.';
        END IF;
        v_enfermedad_final := p_nombre_enfermedad;
    ELSE
        v_tiene_discapacidad := FALSE;
        v_enfermedad_final := NULL;
    END IF;

    -- ==========================================
    -- 8. LÓGICA DE HERMANO (VALIDACIÓN ESTRICTA DE AMBOS APELLIDOS)
    -- ==========================================
    IF p_hermano_txt = 'No tiene' AND (p_curp_hermano IS NOT NULL AND TRIM(p_curp_hermano) <> '') THEN
        RAISE EXCEPTION 'Incoherencia de datos: El campo CURP Hermano debe estar vacío si seleccionó "No tiene".';
    END IF;

    IF p_hermano_txt = 'Si tiene' THEN
        v_curp_hermano_final := UPPER(TRIM(p_curp_hermano));
        
        IF v_curp_hermano_final IS NULL OR v_curp_hermano_final = '' THEN
            RAISE EXCEPTION 'Debe proporcionar la CURP del hermano si este estudia en el plantel.';
        END IF;

        IF v_curp_hermano_final !~ v_regex_curp THEN
            RAISE EXCEPTION 'La CURP del hermano no tiene un formato válido.';
        END IF;

        -- CONTINÚA EL FLUJO: Vuelve a checar la existencia y apellidos obligatoriamente
        SELECT "apellido_paterno", "apellido_materno" 
        INTO v_apellido_p_exp, v_apellido_m_exp
        FROM "Expedientes" 
        WHERE "curp" = v_curp_hermano_final 
        AND "tipo_titular" = 'Alumno';

        IF v_apellido_p_exp IS NULL THEN
            RAISE EXCEPTION 'Error: La CURP % no existe en nuestros Expedientes como alumno vigente.', v_curp_hermano_final;
        END IF;

        -- Validation de coincidencia total de apellidos (Paterno AND Materno)
        IF NOT (
            UPPER(TRIM(p_apellido_paterno)) = UPPER(TRIM(v_apellido_p_exp)) 
            AND 
            COALESCE(UPPER(TRIM(p_apellido_materno)), '') = COALESCE(UPPER(TRIM(v_apellido_m_exp)), '')
        ) THEN
            RAISE EXCEPTION 'Acceso denegado: Los apellidos no coinciden exactamente con los del alumno en el expediente %. Verifique el parentesco.', v_curp_hermano_final;
        END IF;

        v_hermano_plantel := TRUE;
    ELSE
        v_hermano_plantel := FALSE;
        v_curp_hermano_final := NULL;
    END IF;

    -- 9. INSERCIÓN FINAL
    INSERT INTO "Aspirantes" (
        "nombre", "apellido_paterno", "apellido_materno", "curp",
        "escuela_procedencia", "promedio_primaria", "tiene_discapacidad",
        "nombre_enfermedad", "Hermano_Plantel", "curp_hermano",
        "estatus_aspirante", "claveConvocatoria", "claveTutorAspirante"
    ) VALUES (
        p_nombre, p_apellido_paterno, p_apellido_materno, v_curp_formateada,
        p_escuela_procedencia, p_promedio_primaria, v_tiene_discapacidad,
        v_enfermedad_final, v_hermano_plantel, v_curp_hermano_final,
        'En proceso', v_clave_conv_detectada, p_claveTutorAspirante
    );

    RAISE NOTICE 'Registro exitoso. Coherencia de campos, rango de edad y parentesco confirmados.';
END;
$$;