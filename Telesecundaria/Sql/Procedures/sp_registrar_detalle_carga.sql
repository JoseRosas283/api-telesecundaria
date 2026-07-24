CREATE OR REPLACE PROCEDURE sp_registrar_detalle_carga(
    p_clave_carga VARCHAR(18),
    p_clave_documento VARCHAR(18)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_expediente_carga      VARCHAR(18);
    v_expediente_documento  VARCHAR(18);
    v_archivo_url_original  VARCHAR(255);
    v_existe_detalle        BOOLEAN;
    v_existe_mismo_tipo     BOOLEAN; -- Variable para el candado de categoría
    
    -- Variables de control de cuotas
    v_tipo_titular          VARCHAR(20);
    v_total_exigido         INTEGER;
    v_total_actual          INTEGER;
    
    -- Variables para control de Intendente y Tipo de Documento
    v_es_intendente         BOOLEAN;
    v_clave_tipo_doc        VARCHAR(18); -- Guardará la clave del tipo de documento
    v_nombre_tipo_doc       VARCHAR(100);
BEGIN
    -- 1. OBTENER EL EXPEDIENTE Y EL TIPO DE TITULAR
    SELECT "claveExpediente", tipo_titular 
    INTO v_expediente_carga, v_tipo_titular
    FROM "Expedientes" 
    WHERE "claveExpediente"= (SELECT "claveExpediente" FROM "CargasDocumentos" WHERE "claveCarga" = p_clave_carga);

    IF v_expediente_carga IS NULL THEN
        RAISE EXCEPTION 'Error: La carga % no existe o no tiene un expediente válido.', p_clave_carga;
    END IF;

    -- 2. RECUPERAR DATOS DEL DOCUMENTO MAESTRO Y SU TIPO
    -- Se recupera v_clave_tipo_doc para evaluar la categoría del archivo
    SELECT d."claveExpediente", d.archivo_url, d."claveTipoDocumento", td.nombre_documento
    INTO v_expediente_documento, v_archivo_url_original, v_clave_tipo_doc, v_nombre_tipo_doc
    FROM "Documentos" d
    INNER JOIN "TipoDocumentos" td ON d."claveTipoDocumento" = td."claveTipoDocumento"
    WHERE d."claveDocumento" = p_clave_documento;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Error: El documento % no existe.', p_clave_documento;
    END IF;

    -- 3. VALIDACIÓN DE CONGRUENCIA
    IF v_expediente_carga <> v_expediente_documento THEN
        RAISE EXCEPTION 'Bloqueo: El documento no pertenece al expediente dueño de esta carga.';
    END IF;

    -- 4. VALIDACIÓN DE CUOTAS SEGÚN TIPO DE TITULAR
    IF v_tipo_titular = 'Empleado' THEN
        -- Conteo base de documentos laborales activos
        SELECT COUNT(*) INTO v_total_exigido FROM "TipoDocumentos" WHERE area = 'Laboral' AND estado = TRUE;
        
        -- Verificar si el dueño del expediente es un Intendente activo hoy
        SELECT EXISTS (
            SELECT 1 
            FROM "Empleados" e
            INNER JOIN "EmpleadoRol" er ON e."claveEmpleado" = er."claveEmpleado"
            INNER JOIN "Roles" r ON er."claveRol" = r."claveRol"
            WHERE e."claveExpediente" = v_expediente_carga
              AND r.nombre_rol = 'Intendente'
              AND CURRENT_DATE >= er.fecha_inicio 
              AND (er.fecha_fin IS NULL OR CURRENT_DATE <= er.fecha_fin)
        ) INTO v_es_intendente;

        -- Reglas de excepción para puesto de Intendente
        IF v_es_intendente THEN
            -- Candado directo para Título y Cédula Profesional
            IF v_nombre_tipo_doc IN ('TÍTULO PROFESIONAL', 'CÉDULA PROFESIONAL') THEN
                RAISE EXCEPTION 'Bloqueo: El perfil de Intendente no requiere ni permite la carga de %.', v_nombre_tipo_doc;
            END IF;

            -- Descontamos ambos documentos del total exigido (-2)
            v_total_exigido := v_total_exigido - 2;
        END IF;
        
        -- Conteo de documentos actuales filtrando por área laboral y estado activo
        SELECT COUNT(*) INTO v_total_actual
        FROM "DetalleCarga" dc
        JOIN "Documentos" d ON dc."claveDocumento" = d."claveDocumento"
        JOIN "TipoDocumentos" td ON d."claveTipoDocumento" = td."claveTipoDocumento"
        WHERE dc."claveCarga" = p_clave_carga 
          AND td.area = 'Laboral'
          AND td.estado = TRUE;

    ELSIF v_tipo_titular = 'Alumno' THEN
        SELECT COUNT(*) INTO v_total_exigido 
        FROM "Requisitos" 
        WHERE etapa_proceso = 'Inscripción' AND estado_requisito = TRUE;

        SELECT COUNT(*) INTO v_total_actual FROM "DetalleCarga" WHERE "claveCarga"= p_clave_carga;
    END IF;

    -- Verificar si ya está lleno antes de insertar
    IF v_total_actual >= v_total_exigido THEN
        RAISE EXCEPTION 'Límite alcanzado: No se pueden agregar más documentos a este proceso de %.', v_tipo_titular;
    END IF;

    -- 5. VERIFICAR DUPLICADOS
    -- Candado A: Que el documento físico exacto no esté ya vinculado
    SELECT EXISTS (SELECT 1 FROM "DetalleCarga" WHERE "claveDocumento" = p_clave_documento) INTO v_existe_detalle;
    IF v_existe_detalle THEN
        RAISE EXCEPTION 'Error: El documento % ya fue vinculado anteriormente.', p_clave_documento;
    END IF;

    -- Candado B: Que no exista ya un documento de la misma categoría en esta carga específica
    SELECT EXISTS (
        SELECT 1 
        FROM "DetalleCarga" dc
        INNER JOIN "Documentos" d ON dc."claveDocumento" = d."claveDocumento"
        WHERE dc."claveCarga" = p_clave_carga 
          AND d."claveTipoDocumento" = v_clave_tipo_doc
    ) INTO v_existe_mismo_tipo;

    IF v_existe_mismo_tipo THEN
        RAISE EXCEPTION 'Bloqueo: Ya se encuentra registrado un documento de tipo "%" en esta carga.', v_nombre_tipo_doc;
    END IF;

    -- 6. INSERCIÓN FINAL
    INSERT INTO "DetalleCarga" ("claveCarga", "claveDocumento", archivo_url) 
    VALUES (p_clave_carga, p_clave_documento, v_archivo_url_original);

    -- 7. LÓGICA DE CIERRE AUTOMÁTICO (Encapsulada)
    -- Verificamos si con esta inserción (v_total_actual + 1) llegamos al total
    IF (v_total_actual + 1) = v_total_exigido THEN
        UPDATE "CargasDocumentos" 
        SET estatus_validacion = 'Completado',
            observaciones = COALESCE(observaciones, '') || ' | Carga finalizada automáticamente por el sistema.'
        WHERE "claveCarga" = p_clave_carga;
        
        RAISE NOTICE 'Búnker Informa: Carga % marcada como COMPLETADA.', p_clave_carga;
    END IF;

    RAISE NOTICE 'Éxito: Registro completado (% de %)', (v_total_actual + 1), v_total_exigido;

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION '%', SQLERRM;
END;
$$;
