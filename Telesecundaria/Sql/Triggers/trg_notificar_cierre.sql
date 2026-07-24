CREATE OR REPLACE FUNCTION fn_revision_notificar_cambio()
RETURNS TRIGGER AS $$
DECLARE
    v_receptor_info RECORD;
    v_titulo         VARCHAR(80);
    v_mensaje        TEXT;
    v_prioridad      SMALLINT;
    v_nombre_proceso VARCHAR(50); -- Usamos el nombre del CHECK (Nuestro Enum)
    v_docs_json      JSONB;
    
    -- ??? VARIABLES DEL CICLO PARA RECHAZO (Declaradas globalmente para evitar pérdida de ámbito)
    r_doc_error      RECORD;
    v_lista_errores  TEXT;
BEGIN
    -- EL GUARDIÁN: Solo cuando el administrativo cierra la revisión (Abierta -> Cerrada)
    IF (OLD."estado_operativo" = 'Abierta' AND NEW."estado_operativo" = 'Cerrada') THEN
        
        -- 1. BUSCAR AL RECEPTOR, EL NOMBRE DEL TUTOR Y EL NOMBRE DEL HIJO (ASPIRANTE)
        SELECT 
            r."claveReceptor",
            t."nombre" || ' ' || t."apellido_paterno" AS nombre_tutor,   -- Nombre del Padre
            asp."nombre" || ' ' || asp."apellido_paterno" AS nombre_hijo, -- Nombre del Hijo
            a."claveAspirante",      
            asp."claveConvocatoria"  
        INTO v_receptor_info
        FROM "Adjunciones" a
        INNER JOIN "Aspirantes" asp ON a."claveAspirante" = asp."claveAspirante" 
        INNER JOIN "Receptores" r ON r."claveTutorAspirante" = a."claveTutorAspirante"
        INNER JOIN "TutorAspirante" t ON a."claveTutorAspirante" = t."claveTutorAspirante"
        WHERE a."claveAdjuncion" = NEW."claveAdjuncion";

        -- 3. DEFINIR TEXTOS, ENUM Y LÓGICA DE NEGOCIO (Cupos y Estatus)
        IF NEW."estatus_revision" = 'Aceptada' THEN
            
            -- [NO SE MODIFICÓ NADA]: 2. ARMAR EL LISTADO DE DOCUMENTOS (JSON) - ORIGINAL PARA ACEPTACIÓN
            SELECT jsonb_agg(jsonb_build_object(
                'archivo', "claveDocAspirante", 
                'resultado', "estatus_documento", 
                'comentario', COALESCE("motivo_rechazo", 'Correcto')
            )) INTO v_docs_json
            FROM "DetalleAdjuncion"
            WHERE "claveAdjuncion" = NEW."claveAdjuncion";

            -- [NO SE MODIFICÓ NADA]: PRIMERO ASEGURAMOS EL ESTATUS "ACEPTADO"
            UPDATE "Aspirantes" 
            SET "estatus_aspirante" = 'Aceptado'
            WHERE "claveAspirante" = v_receptor_info."claveAspirante";

            -- [NO SE MODIFICÓ NADA]: REGISTRAMOS EN EL BUFFER DE AGENDACIÓN
            CALL registrar_aceptacion_buffer(NEW."claveRevision");

            v_titulo         := '¡Revisión Exitosa!';
            v_mensaje        := 'Estimado tutor ' || v_receptor_info.nombre_tutor || 
                                ', le informamos que la documentación de su hijo ' || v_receptor_info.nombre_hijo || 
                                ' ha sido Aceptada. Favor de esperar su cita para entrega de documentos.';
            v_prioridad      := 2::SMALLINT;
            v_nombre_proceso := 'Documentos Aceptados';

            -- [NO SE MODIFICÓ NADA]: DISPARAMOS NOTIFICACIÓN ANTES DE TOCAR EL CUPO
            CALL sp_emitir_notificacion(
                p_clave_receptor => v_receptor_info."claveReceptor",
                p_titulo         => v_titulo,
                p_mensaje        => v_mensaje,
                p_prioridad      => v_prioridad,
                p_nombre_proceso => v_nombre_proceso,
                p_datos_json     => jsonb_build_object(
                                        'folio_revision', NEW."claveRevision",
                                        'observacion_general', NEW."observacion_general",
                                        'detalle_documentos', v_docs_json
                                     )
            );

            -- [NO SE MODIFICÓ NADA]: ACTUALIZAR EL CUPO DISPONIBLE AL FINAL
            UPDATE "Convocatorias" 
            SET "cupo_disponible" = "cupo_disponible" - 1
            WHERE "claveConvocatoria" = v_receptor_info."claveConvocatoria"
              AND "cupo_disponible" > 0;

            -- [NO SE MODIFICÓ NADA]: Si no se afectó ninguna fila, es porque no hay cupo
            IF NOT FOUND THEN 
                RAISE EXCEPTION 'No se puede aceptar la revisión: Cupo agotado en la convocatoria.';
            END IF;
        
        ELSIF NEW."estatus_revision" = 'Rechazada' THEN
            
            -- =========================================================================
            --  CAMBIO REALIZADO: LEER LA VERDAD DIRECTAMENTE DE DetalleRevision (Revisión 7)
            -- =========================================================================
            SELECT jsonb_agg(jsonb_build_object(
                'archivo', dr."claveDocAspirante", 
                'tipo', td."nombre_documento", 
                'resultado', dr."estatus_doc", 
                'comentario', COALESCE(dr."motivo_rechazo", 'Correcto')
            )) INTO v_docs_json
            FROM "DetalleRevision" dr
            INNER JOIN "DocumentosAspirante" doc ON dr."claveDocAspirante" = doc."claveDocAspirante"
            INNER JOIN "TipoDocumentos" td ON doc."claveTipoDocumento" = td."claveTipoDocumento"
            WHERE dr."claveRevision" = NEW."claveRevision"; -- Filtro por la revisión en proceso

            -- ACTUALIZAR ESTATUS DEL ASPIRANTE A RECHAZADO
            UPDATE "Aspirantes" 
            SET "estatus_aspirante" = 'Rechazado'
            WHERE "claveAspirante" = v_receptor_info."claveAspirante";

            -- Inicialización limpia de la cadena acumuladora
            v_lista_errores := '';

            -- =========================================================================
            --  CAMBIO REALIZADO: SE ELIMINÓ EL DECLARE/BEGIN ANIDADO INTERNO
            -- =========================================================================
            FOR r_doc_error IN 
                SELECT * FROM jsonb_to_recordset(v_docs_json) 
                AS x(archivo VARCHAR(18), tipo TEXT, resultado VARCHAR(50), comentario TEXT)
                WHERE resultado = 'Rechazado'
            LOOP
                -- Usamos el tipo amigable en vez del código de barra del archivo
                v_lista_errores := v_lista_errores || E'\n• ' || r_doc_error.tipo || ': ' || r_doc_error.comentario;
            END LOOP;

            v_titulo         := 'Atención: Documentos%Rechazados';
            v_titulo         := 'Atención: Documentos Rechazados';
            v_prioridad      := 3::SMALLINT;
            v_nombre_proceso := 'Documentos%Rechazados';
            v_nombre_proceso := 'Documentos Rechazados';

            -- MENSAJE PERSONALIZADO DETALLADO (Fuera de bloques anidados, 100% persistente)
            v_mensaje        := 'Estimado tutor ' || v_receptor_info.nombre_tutor || 
                                ', se le informa que la documentación de su hijo ' || v_receptor_info.nombre_hijo || 
                                ' ha sido rechazada por los siguientes motivos:' || E'\n' || 
                                v_lista_errores || E'\n\nPor favor, corrija estos archivos e intente nuevamente.';

            -- DISPARAR NOTIFICACIÓN DE RECHAZO
            CALL sp_emitir_notificacion(
                p_clave_receptor => v_receptor_info."claveReceptor",
                p_titulo         => v_titulo,
                p_mensaje        => v_mensaje,
                p_prioridad      => v_prioridad,
                p_nombre_proceso => v_nombre_proceso,
                p_datos_json     => jsonb_build_object(
                                        'folio_revision', NEW."claveRevision",
                                        'observacion_general', NEW."observacion_general",
                                        'detalle_documentos', v_docs_json
                                     )
            );
        END IF;

        -- 5. EXPULSIÓN DE LA FILA VIRTUAL
        DELETE FROM "FilaVirtual" 
        WHERE "claveAspirante" = v_receptor_info."claveAspirante";

    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_notificar_cierre ON "Revisiones";

CREATE TRIGGER trg_notificar_cierre
AFTER UPDATE OF estado_operativo ON "Revisiones" -- Solo vigila este campo
FOR EACH ROW
EXECUTE FUNCTION fn_revision_notificar_cambio();