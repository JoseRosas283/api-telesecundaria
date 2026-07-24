CREATE OR REPLACE PROCEDURE sp_actualizar_tutor_Aspirante(
    p_claveTutor VARCHAR(18),
    p_nombre VARCHAR(50),
    p_apellido_paterno VARCHAR(50),
    p_apellido_materno VARCHAR(50),
    p_curp_tutor VARCHAR(18),
    p_telefono VARCHAR(15),
    p_correo VARCHAR(100),
    p_parentesco VARCHAR(50),
    p_contrasena VARCHAR(255),
    p_calle_numero VARCHAR(100),
    p_colonia VARCHAR(50),
    p_codigo_postal VARCHAR(5),
    p_municipio VARCHAR(50)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_curp_formateada VARCHAR(18);
    v_correo_formateado VARCHAR(100);
    v_telefono_limpio VARCHAR(15);
    v_existe_tutor BOOLEAN;
    v_estado_activo BOOLEAN; 
    v_existe_adjuncion BOOLEAN;
    v_clave_receptor VARCHAR(18);
    v_correo_actual VARCHAR(100);
    v_tipo_receptor_actual VARCHAR(80);

    v_coincide_propietario BOOLEAN := FALSE;

    v_curp_fecha_str TEXT;
    v_curp_anio INT;
    v_curp_mes INT;
    v_curp_dia INT;
    v_fecha_nacimiento DATE;
    v_anio_actual INT := EXTRACT(YEAR FROM CURRENT_DATE);
    
    v_letra_paterno CHAR(1);
    v_vocal_paterno CHAR(1);
    v_letra_materno CHAR(1);

    v_tutor_actual RECORD;
    v_direccion_actual RECORD;

    v_regex_curp TEXT := '^[A-Z]{4}[0-9]{6}[HM][A-Z]{5}[0-9A-Z][0-9]$';
    v_regex_correo TEXT := '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$';
    v_regex_cp TEXT := '^[0-9]{5}$';
BEGIN
    -- ==========================================
    -- 1. VERIFICACIÓN DE EXISTENCIA Y ESTADO
    -- ==========================================
    SELECT * INTO v_tutor_actual
    FROM "TutorAspirante" 
    WHERE "claveTutorAspirante" = p_claveTutor;

    IF v_tutor_actual.claveTutorAspirante IS NULL THEN
        RAISE EXCEPTION 'Error: El tutor con la clave especificada no existe.';
    END IF;

    v_estado_activo := v_tutor_actual.estado;
    v_correo_actual := v_tutor_actual.correo;

    IF v_estado_activo = FALSE THEN
        RAISE EXCEPTION 'Bloqueo: El tutor se encuentra desactivado y no puede recibir actualizaciones.';
    END IF;

    SELECT * INTO v_direccion_actual
    FROM "Direcciones"
    WHERE "claveTutorAspirante" = p_claveTutor;

    -- ==========================================
    -- 1.5 LIMPIEZA PREVIA
    -- ==========================================
    v_curp_formateada := UPPER(TRIM(p_curp_tutor));
    v_correo_formateado := LOWER(TRIM(p_correo));
    v_telefono_limpio := REGEXP_REPLACE(p_telefono, '[^0-9]', '', 'g');

    -- ==========================================
    -- 1.8 FILTRO INTELIGENTE DE CONTENIDO IDÉNTICO
    -- ==========================================
    IF ROW(v_tutor_actual.nombre, v_tutor_actual.apellido_paterno, v_tutor_actual.apellido_materno, 
           v_tutor_actual.curp_tutor, v_tutor_actual.telefono, v_tutor_actual.correo, 
           v_tutor_actual.parentesco, v_tutor_actual.contrasena) 
       IS NOT DISTINCT FROM 
       ROW(UPPER(TRIM(p_nombre)), UPPER(TRIM(p_apellido_paterno)), NULLIF(UPPER(TRIM(p_apellido_materno)), ''), 
           v_curp_formateada, v_telefono_limpio, v_correo_formateado, 
           UPPER(TRIM(p_parentesco)), p_contrasena)
       AND
       ROW(v_direccion_actual.calle_numero, v_direccion_actual.colonia, v_direccion_actual.codigo_postal, v_direccion_actual.municipio)
       IS NOT DISTINCT FROM
       ROW(UPPER(TRIM(p_calle_numero)), UPPER(TRIM(p_colonia)), p_codigo_postal, UPPER(TRIM(p_municipio)))
    THEN
        RAISE NOTICE 'Exito: Operacion completada sin cambios por datos idénticos.';
        RETURN; 
    END IF;

    -- ==========================================
    -- 2. CANDADO DE ADJUNCIONES
    -- ==========================================
    SELECT EXISTS (
        SELECT 1 FROM "Adjunciones" ADJ
        INNER JOIN "Aspirantes" ASP ON ADJ."claveAspirante" = ASP."claveAspirante"
        WHERE ASP."claveTutorAspirante" = p_claveTutor
    ) INTO v_existe_adjuncion;

    IF v_existe_adjuncion THEN
        RAISE EXCEPTION 'Bloqueo: El tutor ya tiene procesos en Adjunciones. No se permiten cambios.';
    END IF;

    -- ==========================================
    -- 3. VALIDACIONES DE OBLIGATORIEDAD
    -- ==========================================
    IF p_nombre IS NULL OR TRIM(p_nombre) = '' THEN RAISE EXCEPTION 'El nombre es obligatorio.'; END IF;
    IF p_apellido_paterno IS NULL OR TRIM(p_apellido_paterno) = '' THEN RAISE EXCEPTION 'El apellido paterno es obligatorio.'; END IF;
    IF p_contrasena IS NULL OR TRIM(p_contrasena) = '' THEN RAISE EXCEPTION 'La contraseña es obligatoria.'; END IF;

    IF p_calle_numero IS NULL OR TRIM(p_calle_numero) = '' THEN RAISE EXCEPTION 'La calle y número son obligatorios.'; END IF;
    IF p_colonia IS NULL OR TRIM(p_colonia) = '' THEN RAISE EXCEPTION 'La colonia es obligatoria.'; END IF;
    IF p_municipio IS NULL OR TRIM(p_municipio) = '' THEN RAISE EXCEPTION 'El municipio es obligatorio.'; END IF;
    IF p_codigo_postal IS NULL OR TRIM(p_codigo_postal) = '' THEN RAISE EXCEPTION 'El código postal es obligatorio.'; END IF;

    -- ==========================================
    -- 4. VALIDACIONES DE REGLAS DE FORMATO BÁSICO
    -- ==========================================
    IF v_curp_formateada !~ v_regex_curp THEN RAISE EXCEPTION 'CURP invalida en su estructura de caracteres.'; END IF;
    IF v_correo_formateado !~ v_regex_correo THEN RAISE EXCEPTION 'Correo electronico con formato invalido.'; END IF;
    IF p_codigo_postal !~ v_regex_cp THEN RAISE EXCEPTION 'CP debe ser de 5 dígitos.'; END IF;

    -- ============================================================
    -- 4.2 ADUANA BIOMÉTRICA DE CURP
    -- ============================================================
    v_curp_fecha_str := SUBSTRING(v_curp_formateada FROM 5 FOR 6);
    v_curp_anio := SUBSTRING(v_curp_fecha_str FROM 1 FOR 2)::INT;
    v_curp_mes := SUBSTRING(v_curp_fecha_str FROM 3 FOR 2)::INT;
    v_curp_dia := SUBSTRING(v_curp_fecha_str FROM 5 FOR 2)::INT;

    IF v_curp_anio <= (v_anio_actual - 2000) THEN
        v_curp_anio := 2000 + v_curp_anio;
    ELSE
        v_curp_anio := 1900 + v_curp_anio;
    END IF;

    IF v_curp_mes < 1 OR v_curp_mes > 12 THEN 
        RAISE EXCEPTION 'Fraude de CURP en Edición: El mes extraído no es válido.'; 
    END IF;
    
    IF v_curp_dia < 1 OR v_curp_dia > 31 THEN 
        RAISE EXCEPTION 'Fraude de CURP en Edición: El día extraído no es válido.'; 
    END IF;

    BEGIN
        v_fecha_nacimiento := TO_DATE(v_curp_anio || '-' || v_curp_mes || '-' || v_curp_dia, 'YYYY-MM-DD');
    EXCEPTION WHEN OTHERS THEN
        RAISE EXCEPTION 'Fraude de CURP en Edición: La combinación de fecha no existe en el calendario real.';
    END;

    IF v_curp_anio < 1910 THEN 
        RAISE EXCEPTION 'Error de coherencia cronológica en edición: El año de nacimiento es anterior a 1910.'; 
    END IF;
    
    IF v_fecha_nacimiento > CURRENT_DATE THEN 
        RAISE EXCEPTION 'Error de coherencia cronológica en edición: La CURP refiere una fecha en el futuro.'; 
    END IF;

    -- B) Cruce Alfabético contra Apellido Paterno (Parchado para Tildes)
    v_letra_paterno := SUBSTRING(TRANSLATE(UPPER(TRIM(p_apellido_paterno)), 'ÁÉÍÓÚÜ', 'AEIOUU') FROM 1 FOR 1);
    SELECT SUBSTRING(TRANSLATE(SUBSTRING(UPPER(TRIM(p_apellido_paterno)) FROM 2), 'ÁÉÍÓÚÜ', 'AEIOUU') FROM '[AEIOU]') INTO v_vocal_paterno;

    IF SUBSTRING(v_curp_formateada FROM 1 FOR 1) <> v_letra_paterno THEN
        RAISE EXCEPTION 'Incoherencia biográfica en Edición: La primera letra de la CURP no coincide con la inicial del Apellido Paterno.';
    END IF;

    IF v_vocal_paterno IS NOT NULL AND SUBSTRING(v_curp_formateada FROM 2 FOR 1) <> v_vocal_paterno THEN
        RAISE EXCEPTION 'Incoherencia biográfica en Edición: La segunda letra de la CURP debe ser la primera vocal interna del Apellido Paterno.';
    END IF;

    -- C) Cruce Alfabético contra Apellido Materno (Parchado para Tildes)
    IF p_apellido_materno IS NOT NULL AND TRIM(p_apellido_materno) <> '' THEN
        v_letra_materno := SUBSTRING(TRANSLATE(UPPER(TRIM(p_apellido_materno)), 'ÁÉÍÓÚÜ', 'AEIOUU') FROM 1 FOR 1);
        IF SUBSTRING(v_curp_formateada FROM 3 FOR 1) <> v_letra_materno THEN
            RAISE EXCEPTION 'Incoherencia biográfica en Edición: La tercera letra de la CURP no coincide con la inicial del Apellido Materno.';
        END IF;
    ELSE
        IF SUBSTRING(v_curp_formateada FROM 3 FOR 1) <> 'X' THEN
            RAISE EXCEPTION 'Incoherencia biográfica en Edición: Al no contar con Apellido Materno, la tercera posición de la CURP debe ser una letra X.';
        END IF;
    END IF;

    -- ==========================================
    -- 4.5 VALIDACIÓN DE UNICIDAD E IDENTIDAD
    -- ==========================================
    
    -- ADUANA A
    IF EXISTS (
        SELECT 1 FROM "TutorAspirante" 
        WHERE curp_tutor = v_curp_formateada 
          AND estado = TRUE 
          AND "claveTutorAspirante" <> p_claveTutor
    ) THEN
        RAISE EXCEPTION 'Error: La CURP ingresada ya está siendo utilizada por otro tutor ACTIVO en esta convocatoria.';
    END IF;

    -- ADUANA B
    IF v_correo_formateado <> v_correo_actual THEN
        
        IF EXISTS (
            SELECT 1 FROM "TutorAspirante" 
            WHERE correo = v_correo_formateado 
              AND estado = TRUE 
              AND "claveTutorAspirante" <> p_claveTutor
        ) THEN
            RAISE EXCEPTION 'Error: El correo ingresado ya está vinculado a otro tutor activo.';
        END IF;

        IF EXISTS (
            SELECT 1 FROM "Receptores" 
            WHERE LOWER("correo_destino") = v_correo_formateado 
              AND estado = TRUE
              AND ("claveTutorAspirante" IS NULL OR "claveTutorAspirante" <> p_claveTutor)
        ) THEN
            
            SELECT "tipo_receptor" INTO v_tipo_receptor_actual
            FROM "Receptores" 
            WHERE LOWER("correo_destino") = v_correo_formateado 
              AND estado = TRUE
            LIMIT 1;

            IF v_tipo_receptor_actual IN ('Usuario', 'TutorAspirante') THEN
                RAISE EXCEPTION 'Bloqueo de Seguridad: El correo ya está registrado en el sistema bajo un rol institucional activo no autorizado para tutores.';
            
            ELSIF v_tipo_receptor_actual = 'Tutor' THEN
                v_coincide_propietario := FALSE;

                SELECT TRUE INTO v_coincide_propietario
                FROM "Receptores" r
                JOIN "Tutores" t ON r."claveTutor" = t."claveTutor" 
                WHERE LOWER(r."correo_destino") = v_correo_formateado
                  AND r.estado = TRUE
                  AND r."tipo_receptor" = 'Tutor'
                  AND t.curp_tutor = v_curp_formateada 
                  AND UPPER(TRIM(t.nombre)) = UPPER(TRIM(p_nombre))
                  AND UPPER(TRIM(t.apellido_paterno)) = UPPER(TRIM(p_apellido_paterno))
                  AND UPPER(TRIM(t.parentesco)) = UPPER(TRIM(p_parentesco))
                  AND COALESCE(UPPER(TRIM(t.apellido_materno)), '') = COALESCE(UPPER(TRIM(p_apellido_materno)), '');

                IF v_coincide_propietario IS NOT TRUE THEN
                    RAISE EXCEPTION 'Bloqueo de Seguridad: El correo ya está asignado a un Tutor activo, y las credenciales no corresponden al propietario.';
                END IF;
                
                RAISE NOTICE 'Validación exitosa en actualización: Se reconoce al Tutor Definitivo.';
            END IF;
        END IF;

    -- ADUANA C
    ELSIF EXISTS (
        SELECT 1 FROM "Tutores" WHERE curp_tutor = v_curp_formateada
    ) THEN
        v_coincide_propietario := FALSE;

        SELECT TRUE INTO v_coincide_propietario
        FROM "Tutores"
        WHERE curp_tutor = v_curp_formateada
          AND UPPER(TRIM(nombre)) = UPPER(TRIM(p_nombre))
          AND UPPER(TRIM(apellido_paterno)) = UPPER(TRIM(p_apellido_paterno))
          AND UPPER(TRIM(parentesco)) = UPPER(TRIM(p_parentesco));

        IF v_coincide_propietario IS NOT TRUE THEN
            RAISE EXCEPTION 'Bloqueo de Seguridad: Los cambios de identidad no corresponden con el registro maestro de la CURP institucional.';
        END IF;
    END IF;

    -- ==========================================
    -- 5. ACTUALIZACIÓN DE TABLAS
    -- ==========================================
    UPDATE "TutorAspirante" SET
        nombre = UPPER(TRIM(p_nombre)),
        apellido_paterno = UPPER(TRIM(p_apellido_paterno)),
        apellido_materno = CASE WHEN p_apellido_materno IS NULL OR TRIM(p_apellido_materno) = '' THEN NULL ELSE UPPER(TRIM(p_apellido_materno)) END,
        curp_tutor = v_curp_formateada,
        telefono = v_telefono_limpio,
        correo = v_correo_formateado,
        parentesco = UPPER(TRIM(p_parentesco)),
        contrasena = p_contrasena
    WHERE "claveTutorAspirante" = p_claveTutor;

    UPDATE "Direcciones" SET
        calle_numero = UPPER(TRIM(p_calle_numero)),
        colonia = UPPER(TRIM(p_colonia)),
        codigo_postal = p_codigo_postal,
        municipio = UPPER(TRIM(p_municipio))
    WHERE "claveTutorAspirante" = p_claveTutor;

    SELECT "claveReceptor" INTO v_clave_receptor FROM "Receptores" 
    WHERE "claveTutorAspirante" = p_claveTutor AND "tipo_receptor" = 'TutorAspirante';

    IF v_clave_receptor IS NOT NULL THEN
        CALL sp_actualizar_receptor(v_clave_receptor, v_correo_formateado);
    END IF;

    RAISE NOTICE 'Operacion completada con exito.';

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Fallo en la actualizacion integral debido al siguiente error interno: %', SQLERRM;
END;
$$;
