CREATE OR REPLACE PROCEDURE sp_insertar_tutor_con_direccion(
    -- Datos personales del Tutor
    p_nombre VARCHAR(50),
    p_apellido_paterno VARCHAR(50),
    p_apellido_materno VARCHAR(50),
    p_curp_tutor VARCHAR(18),
    p_telefono VARCHAR(15),
    p_correo VARCHAR(100),
    p_parentesco VARCHAR(50),
    p_contrasena VARCHAR(255),
    -- Datos de la Dirección
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
    v_clave_tutor_generada VARCHAR(18);
    v_tipo_receptor_actual VARCHAR(80); -- Variable necesaria para interceptar el tipo exacto
    
    -- Variable de control para la salida de identidad inteligente (Comprobación 3)
    v_coincide_propietario BOOLEAN := FALSE;
    
    -- NUEVAS VARIABLES PARA ADUANA BIOMÉTRICA DE CURP
    v_curp_fecha_str TEXT;
    v_curp_anio INT;
    v_curp_mes INT;
    v_curp_dia INT;
    v_fecha_nacimiento DATE;
    v_anio_actual INT := EXTRACT(YEAR FROM CURRENT_DATE);
    
    v_letra_paterno CHAR(1);
    v_vocal_paterno CHAR(1);
    v_letra_materno CHAR(1);
    
    -- Expresiones Regulares
    v_regex_curp TEXT := '^[A-Z]{4}[0-9]{6}[HM][A-Z]{5}[0-9A-Z][0-9]$';
    v_regex_correo TEXT := '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$';
    v_regex_cp TEXT := '^[0-9]{5}$';
    v_regex_tel TEXT := '^[0-9]+$';
BEGIN
    -- 0. VALIDACIÓN DE DISPONIBILIDAD DEL SISTEMA
    IF NOT EXISTS (
        SELECT 1 FROM "Convocatorias" WHERE estado = 'Publicada' AND activacion = TRUE
    ) THEN 
        RAISE EXCEPTION 'El registro de tutores no está disponible actualmente. No hay convocatorias activas.';
    END IF;

    -- 1. LIMPIEZA Y FORMATEO
    v_curp_formateada := UPPER(TRIM(p_curp_tutor));
    v_correo_formateado := LOWER(TRIM(p_correo));
    v_telefono_limpio := TRIM(p_telefono);

    -- 2. VALIDACIONES DE INTEGRIDAD
    IF p_nombre IS NULL OR TRIM(p_nombre) = '' THEN RAISE EXCEPTION 'El nombre del tutor es obligatorio.'; END IF;
    IF p_apellido_paterno IS NULL OR TRIM(p_apellido_paterno) = '' THEN RAISE EXCEPTION 'El apellido paternal es obligatorio.'; END IF;
    IF v_telefono_limpio IS NULL OR v_telefono_limpio = '' THEN RAISE EXCEPTION 'El teléfono de contacto es obligatorio.'; END IF;
    IF p_contrasena IS NULL OR TRIM(p_contrasena) = '' THEN RAISE EXCEPTION 'La contraseña es obligatoria.'; END IF;

    -- Validación Estricta de Dirección
    IF p_calle_numero IS NULL OR TRIM(p_calle_numero) = '' OR p_colonia IS NULL OR TRIM(p_colonia) = '' OR 
       p_codigo_postal IS NULL OR TRIM(p_codigo_postal) = '' OR p_municipio IS NULL OR TRIM(p_municipio) = '' THEN 
        RAISE EXCEPTION 'Todos los campos de la dirección son obligatorios.'; 
    END IF;

    -- 3. VALIDACIONES DE FORMATO
    IF v_telefono_limpio !~ v_regex_tel THEN RAISE EXCEPTION 'El teléfono debe contener únicamente números.'; END IF;
    IF LENGTH(v_telefono_limpio) < 10 OR LENGTH(v_telefono_limpio) > 15 THEN RAISE EXCEPTION 'El teléfono debe tener entre 10 y 15 dígitos.'; END IF;
    IF v_curp_formateada !~ v_regex_curp THEN RAISE EXCEPTION 'La CURP % no tiene un formato oficial válido.', v_curp_formateada; END IF;
    IF v_correo_formateado !~ v_regex_correo THEN RAISE EXCEPTION 'El correo % no tiene una estructura válida.', v_correo_formateado; END IF;
    IF p_codigo_postal !~ v_regex_cp THEN RAISE EXCEPTION 'El código postal debe ser de exactamente 5 dígitos.'; END IF;

    -- ============================================================
    -- 3.1 ADUANA BIOMÉTRICA DE CURP (CRUCE ESTRICTO DE IDENTIDAD)
    -- ============================================================
    
    -- A) EXTRACCIÓN Y VALIDACIÓN DE FECHA DE NACIMIENTO
    v_curp_fecha_str := SUBSTRING(v_curp_formateada FROM 5 FOR 6);
    v_curp_anio := SUBSTRING(v_curp_fecha_str FROM 1 FOR 2)::INT;
    v_curp_mes := SUBSTRING(v_curp_fecha_str FROM 3 FOR 2)::INT;
    v_curp_dia := SUBSTRING(v_curp_fecha_str FROM 5 FOR 2)::INT;

    -- Lógica de siglo (RENAPO): Si el año es menor o igual al año actual abreviado (26), es 2000+. Si no, es 1900+.
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

    -- Intentar conversión real para detectar fechas imposibles (ej. 31 de abril)
    BEGIN
        v_fecha_nacimiento := TO_DATE(v_curp_anio || '-' || v_curp_mes || '-' || v_curp_dia, 'YYYY-MM-DD');
    EXCEPTION WHEN OTHERS THEN
        RAISE EXCEPTION 'Fraude de CURP: La combinación de fecha [%] en la CURP no existe en el calendario real.', v_curp_fecha_str;
    END;

    -- Candados cronológicos solicitados
    IF v_curp_anio < 1910 THEN
        RAISE EXCEPTION 'Error de coherencia cronológica: El año de nacimiento [%] es anterior a 1910.', v_curp_anio;
    END IF;

    IF v_fecha_nacimiento > CURRENT_DATE THEN
        RAISE EXCEPTION 'Error de coherencia cronológica: La fecha extraída de la CURP [%] pertenece al futuro.', TO_CHAR(v_fecha_nacimiento, 'DD-MM-YYYY');
    END IF;

    -- B) CRUCE DE LETRAS CONTRA EL APELLIDO PATERNO
    v_letra_paterno := SUBSTRING(TRANSLATE(UPPER(TRIM(p_apellido_paterno)), 'ÁÉÍÓÚÜ', 'AEIOUU') FROM 1 FOR 1);
    
    -- Busca la primera vocal interna saltándose la primera letra
    SELECT SUBSTRING(TRANSLATE(SUBSTRING(UPPER(TRIM(p_apellido_paterno)) FROM 2), 'ÁÉÍÓÚÜ', 'AEIOUU') FROM '[AEIOU]') INTO v_vocal_paterno;

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
        v_letra_materno := SUBSTRING(TRANSLATE(UPPER(TRIM(p_apellido_materno)), 'ÁÉÍÓÚÜ', 'AEIOUU') FROM 1 FOR 1);
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

    -- ============================================================
    -- 4. CONTROL DE UNICIDAD E IDENTIDAD INTELIGENTE (ACTUALIZADO)
    -- ============================================================
    -- ADUANA A: Validación Local en Convocatoria Actual (Misma tabla)
    IF EXISTS (
        SELECT 1 
        FROM "TutorAspirante" 
        WHERE curp_tutor = v_curp_formateada 
          AND estado = TRUE
    ) THEN
        RAISE EXCEPTION 'Ya existe un tutor activo registrado en esta convocatoria con la CURP %.', v_curp_formateada;
    END IF;

    -- Solo rebota el correo si la otra cuenta local sigue ACTIVA en la convocatoria
    IF EXISTS (
        SELECT 1 
        FROM "TutorAspirante" 
        WHERE correo = v_correo_formateado 
          AND estado = TRUE
    ) THEN
        RAISE EXCEPTION 'El correo electrónico % ya está vinculado a otra cuenta ACTIVA en TutorAspirante.', v_correo_formateado;
    END IF;

    -- ADUANA B: Blindaje Transversal e Interinstitucional en Receptores Activos
    IF EXISTS (
        SELECT 1 
        FROM "Receptores" 
        WHERE LOWER("correo_destino") = v_correo_formateado 
          AND estado = TRUE
    ) THEN
        
        -- Extraemos directamente el tipo de receptor activo
        SELECT "tipo_receptor" INTO v_tipo_receptor_actual
        FROM "Receptores" 
        WHERE LOWER("correo_destino") = v_correo_formateado 
          AND estado = TRUE
        LIMIT 1;

        -- Bloqueo determinista basado en el tipo de rol institucional detectado
        IF v_tipo_receptor_actual IN ('Usuario', 'TutorAspirante') THEN
            RAISE EXCEPTION 'Bloqueo de Seguridad: El correo % ya está registrado en el sistema bajo un rol institucional [%] activo no autorizado para tutores.', 
                v_correo_formateado, v_tipo_receptor_actual;
        
        ELSIF v_tipo_receptor_actual = 'Tutor' THEN
            v_coincide_propietario := FALSE;

            -- Si es estrictamente un 'Tutor' definitivo activo, validamos consistencia biográfica y de CURP al 100%
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

            -- Si no hubo coincidencia exacta de datos de identidad, se bloquea el proceso
            IF v_coincide_propietario IS NOT TRUE THEN
                RAISE EXCEPTION 'Bloqueo de Seguridad: El correo % ya está registrado en el sistema, no coinciden sus datos con las credenciales de identidad oficiales.', v_correo_formateado;
            END IF;
            
            RAISE NOTICE 'Validación de identidad exitosa: Se reconoce al Tutor Definitivo activo con el parentesco y CURP correctos. Reutilización transaccional del correo autorizada.';
        END IF;

    -- ADUANA C: Validación de CURP existente en catálogo definitivo con un correo nuevo
    ELSIF EXISTS (
        SELECT 1 FROM "Tutores" WHERE curp_tutor = v_curp_formateada
    ) THEN
        v_coincide_propietario := FALSE;

        -- Confirmamos que el dueño legítimo de esa CURP histórica sea quien llena el formulario
        SELECT TRUE INTO v_coincide_propietario
        FROM "Tutores"
        WHERE curp_tutor = v_curp_formateada
          AND UPPER(TRIM(nombre)) = UPPER(TRIM(p_nombre))
          AND UPPER(TRIM(apellido_paterno)) = UPPER(TRIM(p_apellido_paterno))
          AND UPPER(TRIM(parentesco)) = UPPER(TRIM(p_parentesco));

        IF v_coincide_propietario IS NOT TRUE THEN
            RAISE EXCEPTION 'Bloqueo de Seguridad: La CURP % ya está registrada en el catálogo definitivo de la institución bajo otra identidad.', v_curp_formateada;
        END IF;

        RAISE NOTICE 'Validación de identidad exitosa: Se reconoce la CURP del Tutor Definitivo con una nueva cuenta de correo de contacto.';
    END IF;

    -- ============================================================
    -- BLOQUES DE INSERCIÓN ORIGINALES (SIN CAMBIOS)
    -- ============================================================
    -- 5. INSERCIÓN DEL TUTOR
    INSERT INTO "TutorAspirante" (
        nombre, apellido_paterno, apellido_materno, curp_tutor, telefono, correo, parentesco, contrasena, estado
    ) VALUES (
        p_nombre, p_apellido_paterno, p_apellido_materno, v_curp_formateada, v_telefono_limpio, v_correo_formateado, p_parentesco, p_contrasena, TRUE
    ) RETURNING "claveTutorAspirante" INTO v_clave_tutor_generada;

    -- 6. INSERCIÓN DE LA DIRECCIÓN
    INSERT INTO "Direcciones" (
        calle_numero, colonia, codigo_postal, municipio, "estado_verificacion", "claveTutorAspirante"
    ) VALUES (
        TRIM(p_calle_numero), TRIM(p_colonia), p_codigo_postal, TRIM(p_municipio), TRUE, v_clave_tutor_generada
    );

    -- 7. ENCAPSULAMIENTO DEL RECEPTOR
    CALL sp_generar_receptor(
        'TutorAspirante', 
        v_clave_tutor_generada, 
        v_correo_formateado
    );

    RAISE NOTICE 'Proceso completo: Tutor registrado, dirección vinculada y receptor habilitado.';

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error en el registro integral: %', SQLERRM;
END;
$$;