CREATE OR REPLACE FUNCTION entrega_completada()
RETURNS TRIGGER AS $$
DECLARE
    -- Variables para los datos del Alumno (Aspirante)
    v_nombre_asp VARCHAR(50);
    v_paterno_asp VARCHAR(50);
    v_materno_asp VARCHAR(50);
    v_curp_asp VARCHAR(18);
    v_claveTutorAspirante VARCHAR(18);
    
    -- Variables para identidades definitivas
    v_clave_expediente_nueva VARCHAR(18);
    v_claveAlumno_nueva VARCHAR(18);
    v_claveTutor_definitiva VARCHAR(18);
    
    -- VARIABLE AGREGADA: Para capturar el estado del tutor definitivo si ya existe
    v_estado_tutor_definitivo BOOLEAN;
    
    -- Variable tipo registro para leer todos los datos del Tutor de Origen
    v_reg_tutor RECORD;
    
    -- Variable tipo registro para iterar los documentos de la adjunción original
    v_reg_doc RECORD;

    -- NUEVA VARIABLE: Para validar si el tutor aún tiene citas pendientes de otros hijos
    v_citas_pendientes INTEGER;
BEGIN
    -- ============================================================
    -- FILTRO DE SEGURIDAD: SÓLO ACTÚA SI PASA A 'Completada'
    -- ============================================================
    IF NEW."estado_final" = 'Completada' AND (OLD."estado_final" IS NULL OR OLD."estado_final" <> 'Completada') THEN
        
        -- ============================================================
        -- 1. VIAJE RELACIONAL: Extraer datos del Alumno y su link al Tutor
        -- ============================================================
        SELECT "nombre", "apellido_paterno", "apellido_materno", "curp", "claveTutorAspirante"
        INTO v_nombre_asp, v_paterno_asp, v_materno_asp, v_curp_asp, v_claveTutorAspirante
        FROM "Aspirantes"
        WHERE "claveAspirante" = NEW."claveAspirante";

        -- Control de pánico por si la consistencia interna falla
        IF v_curp_asp IS NULL THEN
            RAISE EXCEPTION 'Error catastrófico: No se encontraron los datos del Aspirante para la entrega %.', NEW."claveEntrega";
        END IF;

        -- ============================================================
        -- 2. VIAJE RELACIONAL: Extraer los datos completos del Tutor Origen
        -- ============================================================
        SELECT * INTO v_reg_tutor
        FROM "TutorAspirante"
        WHERE "claveTutorAspirante" = v_claveTutorAspirante;

        -- Control de seguridad para el tutor
        IF v_reg_tutor.curp_tutor IS NULL THEN
            RAISE EXCEPTION 'Error de Humanidad/Consistencia: El aspirante % no tiene un tutor asociado en TutorAspirante.', NEW."claveAspirante";
        END IF;

        -- ============================================================
        -- [CAMBIO DE ORDEN]: DESACTIVACIÓN PREVIA DEL RECEPTOR TEMPORAL
        -- ============================================================
        -- Se ejecuta en este punto exacto para apagar el estado activo del
        -- receptor histórico y permitir que 'sp_insertar_tutor' valide sin bloqueos.
        UPDATE "Receptores"
        SET "estado" = FALSE  
        WHERE "claveTutorAspirante" = v_claveTutorAspirante AND "tipo_receptor" = 'TutorAspirante';

        -- ============================================================
        -- 3. PROCESAMIENTO ENCAPSULADO DEL TUTOR CON VALIDACIÓN DE ESTADO
        -- ============================================================
        -- Verificamos si la CURP ya existe en el sistema definitivo y extraemos su estado
        SELECT "claveTutor", "estado" 
        INTO v_claveTutor_definitiva, v_estado_tutor_definitivo
        FROM "Tutores" 
        WHERE UPPER(TRIM("curp_tutor")) = UPPER(TRIM(v_reg_tutor."curp_tutor"));

        -- SÓLO SI NO EXISTE se intenta registrar
        IF v_claveTutor_definitiva IS NULL THEN
            BEGIN
                -- Invocamos tu SP especializado con todos los parámetros requeridos
                CALL sp_insertar_tutor(
                    v_reg_tutor."nombre",
                    v_reg_tutor."apellido_paterno",
                    v_reg_tutor."apellido_materno",
                    v_reg_tutor."curp_tutor",
                    v_reg_tutor."telefono",
                    v_reg_tutor."correo",
                    v_reg_tutor."parentesco",
                    v_reg_tutor."estado"
                );
                
                -- CAPTURA DE CLAVE TUTOR DEFINITIVA RECIÉN CREADA
                SELECT "claveTutor" INTO v_claveTutor_definitiva 
                FROM "Tutores" 
                WHERE UPPER(TRIM("curp_tutor")) = UPPER(TRIM(v_reg_tutor."curp_tutor"));

                RAISE NOTICE 'Trigger Alertas: Tutor % registrado como oficial y receptor generado.', v_reg_tutor."curp_tutor";

            EXCEPTION 
                WHEN OTHERS THEN
                    RAISE NOTICE 'Aviso Trigger: Error inesperado al insertar tutor nuevo. Motivo: %', SQLERRM;
            END;
        ELSE
            -- MODIFICACIÓN ADICIONADA: Control si el tutor definitivo está desactivado
            IF v_estado_tutor_definitivo = FALSE THEN
                v_claveTutor_definitiva := NULL; -- Se anula para evitar el INSERT del bloque 6
                RAISE NOTICE 'Aviso Trigger: El tutor con CURP % existe pero está DESACTIVADO. No se le asignará el alumno.', v_reg_tutor."curp_tutor";
            ELSE
                -- SI EXISTE Y ESTÁ ACTIVO simplemente lo ignora y avisa en consola
                RAISE NOTICE 'Aviso Trigger: El tutor con CURP % ya existía en el sistema definitivo. Saltando inserción de tutor y receptor.', v_reg_tutor."curp_tutor";
            END IF;
        END IF;

        -- ============================================================
        -- 4. PROCESAMIENTO DEL EXPEDIENTE Y ALUMNO
        -- ============================================================
        -- Creamos el expediente inicial del alumno (Internamente crea al Alumno vía SP)
        CALL sp_insertar_expediente(
            v_nombre_asp,
            v_paterno_asp,
            v_materno_asp,
            v_curp_asp,
            'Alumno',
            NEW."claveEntrega"
        );

        -- Capturamos la clave del expediente recién creado
        SELECT "claveExpediente" INTO v_clave_expediente_nueva
        FROM "Expedientes"
        WHERE "claveEntrega" = NEW."claveEntrega";

        -- Control de seguridad por si el expediente no se guardó correctamente
        IF v_clave_expediente_nueva IS NULL THEN
            RAISE EXCEPTION 'Error interno: No se pudo recuperar el expediente para la entrega % al intentar registrar al alumno.', NEW."claveEntrega";
        END IF;

        -- CAPTURA DE CLAVE ALUMNO DEFINITIVA
        SELECT "claveAlumno" INTO v_claveAlumno_nueva 
        FROM "Alumnos" 
        WHERE "claveExpediente" = v_clave_expediente_nueva;

        -- ============================================================
        -- NUEVO PASO: MIGRACIÓN DE DOCUMENTOS VÍA SP
        -- ============================================================
        FOR v_reg_doc IN (
            SELECT dao."ruta_pdf_original", td."nombre_documento"
            FROM "Entregas" e
            INNER JOIN "AdjuncionesOriginales" ao ON e."claveEntrega" = ao."claveEntrega"
            INNER JOIN "DetalleAdjuncionOriginal" dao ON ao."claveAdjOriginal" = dao."claveAdjOriginal"
            INNER JOIN "DocumentosAspirante" da ON dao."claveDocAspirante" = da."claveDocAspirante"
            INNER JOIN "TipoDocumentos" td ON da."claveTipoDocumento" = td."claveTipoDocumento"
            WHERE e."claveEntrega" = NEW."claveEntrega"
        ) LOOP
            BEGIN
                CALL sp_insertar_documento(
                    v_reg_doc."ruta_pdf_original",
                    v_clave_expediente_nueva,
                    v_reg_doc."nombre_documento"
                );
            EXCEPTION
                WHEN OTHERS THEN
                    RAISE NOTICE 'Aviso Trigger: No se pudo indexar el archivo % en el expediente definitivo. Motivo: %', 
                        v_reg_doc."nombre_documento", SQLERRM;
            END;
        END LOOP;

        RAISE NOTICE 'Trigger Documentos: Iteración masiva completada para el Expediente %.', v_clave_expediente_nueva;

        -- ============================================================
        -- 5. DEPURACIÓN AUTOMÁTICA (CIERRE DE CICLO ASPIRANTE)
        -- ============================================================
        -- A) Desactivamos al Aspirante SIEMPRE (Ya es Alumno)
        UPDATE "Aspirantes"
        SET "estado" = FALSE
        WHERE "claveAspirante" = NEW."claveAspirante";

        -- [AJUSTE]: La desactivación del receptor se adelantó al bloque previo al procesamiento del tutor.

        -- C) Verificamos si el Tutor tiene más citas de inscripción programadas (otros hijos)
        SELECT COUNT(*) INTO v_citas_pendientes
        FROM "CitasInscripcion"
        WHERE "claveTutorAspirante" = v_claveTutorAspirante
          AND "estado_cita" = 'Programada';

        -- D) Desactivación condicional del Tutor en la tabla TutorAspirante
        IF v_citas_pendientes = 0 THEN
            UPDATE "TutorAspirante"
            SET "estado" = FALSE
            WHERE "claveTutorAspirante" = v_claveTutorAspirante;
            
            RAISE NOTICE 'Depuración Completa: Aspirante, Receptor y Tutor desactivados por fin de trámites.';
        ELSE
            RAISE NOTICE 'Depuración Parcial: Aspirante y Receptor desactivados. Tutor permanece ACTIVO por % cita(s) pendiente(s).', v_citas_pendientes;
        END IF;

        -- ============================================================
        -- 6. ASIGNACIÓN LEGAL (VÍNCULO FINAL TUTOR-ALUMNO)
        -- ============================================================
        -- Se realiza al final, una vez que el aspirante ha sido "apagado" 
        -- y el alumno tiene sus documentos listos.
        IF v_claveAlumno_nueva IS NOT NULL AND v_claveTutor_definitiva IS NOT NULL THEN
            CALL sp_asignar_tutor_alumno(v_claveAlumno_nueva, v_claveTutor_definitiva);
            
            RAISE NOTICE 'Vínculo: Alumno % asignado legalmente al Tutor %.', v_claveAlumno_nueva, v_claveTutor_definitiva;
        END IF;

        RAISE NOTICE 'Trigger Consecuencias: Ciclo completo finalizado. Registro definitivo, Alumno, Tutor y Documentos processed para la entrega %.', NEW."claveEntrega";

    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;


CREATE OR REPLACE TRIGGER trg_entrega_completada
AFTER UPDATE OF "estado_final" ON "Entregas" 
FOR EACH ROW
EXECUTE FUNCTION entrega_completada();