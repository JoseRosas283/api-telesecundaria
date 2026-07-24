CREATE OR REPLACE PROCEDURE validar_e_insertar_ruta_rechazada(
    p_claveAdjuncion VARCHAR(18),
    p_claveRevision VARCHAR(18),
    p_claveDocAspirante VARCHAR(18)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_claveAdjuncion_real VARCHAR(18);
    v_estatus_doc_revision VARCHAR(50);
    v_estatus_doc_adjuncion VARCHAR(20); -- ??? Corregido a VARCHAR(20) igual que en tu tabla DetalleAdjuncion
    v_ruta_actual VARCHAR(255);
BEGIN
    -- 1. VALIDACIÓN: Que la Adjunción exista en la cabecera
    IF NOT EXISTS (SELECT 1 FROM "Adjunciones" WHERE "claveAdjuncion" = p_claveAdjuncion) THEN
        RAISE EXCEPTION 'Error de Validación: La Adjunción con clave % no existe.', p_claveAdjuncion;
    END IF;

    -- 2. VALIDACIÓN: Que la Revisión exista en la cabecera
    IF NOT EXISTS (SELECT 1 FROM "Revisiones" WHERE "claveRevision" = p_claveRevision) THEN
        RAISE EXCEPTION 'Error de Validación: La Revisión con clave % no existe.', p_claveRevision;
    END IF;

    -- 3. VALIDACIÓN: Que el Documento exista en el catálogo global y tenga ruta
    SELECT "ruta_archivo" INTO v_ruta_actual
    FROM "DocumentosAspirante" 
    WHERE "claveDocAspirante" = p_claveDocAspirante;

    IF v_ruta_actual IS NULL THEN
        RAISE EXCEPTION 'Error de Validación: El Documento con clave % no existe o no tiene una ruta de archivo asignada.', p_claveDocAspirante;
    END IF;

    -- 4. VALIDACIÓN: Que la Adjunción encapsulada en la Revisión sea la misma que se está procesando
    SELECT "claveAdjuncion" INTO v_claveAdjuncion_real
    FROM "Revisiones"
    WHERE "claveRevision" = p_claveRevision;

    IF v_claveAdjuncion_real <> p_claveAdjuncion THEN
        RAISE EXCEPTION 'Error de Integridad: La revisión % pertenece a la adjunción %, no a la adjunción % enviada.', 
            p_claveRevision, v_claveAdjuncion_real, p_claveAdjuncion;
    END IF;

    -- 5. VALIDACIÓN: Que el documento exista en DetalleRevision y extraemos su 'estatus_doc'
    SELECT "estatus_doc" INTO v_estatus_doc_revision
    FROM "DetalleRevision"
    WHERE "claveRevision" = p_claveRevision 
      AND "claveDocAspirante" = p_claveDocAspirante;

    -- Si el documento no participó en esta revisión en específico
    IF v_estatus_doc_revision IS NULL THEN
        RAISE EXCEPTION 'Error de Consistencia: El documento % no se encuentra registrado en el DETALLE de la revisión %.', 
            p_claveDocAspirante, p_claveRevision;
    END IF;

    -- 6. VALIDACIÓN: Que el documento exista en DetalleAdjuncion y extraemos su 'estatus_documento'
    SELECT "estatus_documento" INTO v_estatus_doc_adjuncion
    FROM "DetalleAdjuncion"
    WHERE "claveAdjuncion" = p_claveAdjuncion 
      AND "claveDocAspirante" = p_claveDocAspirante;

    -- Si el documento no existe en el detalle de la adjunción
    IF v_estatus_doc_adjuncion IS NULL THEN
        RAISE EXCEPTION 'Error de Consistencia: El documento % está en la revisión %, pero NO EXISTE en el detalle de la adjunción %.', 
            p_claveDocAspirante, p_claveRevision, p_claveAdjuncion;
    END IF;

    -- =================================================================
    -- 7. REGLAS DE NEGOCIO (Validaciones de Estado cruzadas)
    -- =================================================================
    
    -- ??? AJUSTE: Validar que el documento no tenga ya un rechazo en ESTA misma Adjunción
    IF EXISTS (
        SELECT 1 
        FROM "RutasRechazadas" 
        WHERE "claveAdjuncion" = p_claveAdjuncion 
          AND "claveDocAspirante" = p_claveDocAspirante
    ) THEN
        RAISE EXCEPTION 'Error de Unicidad: El documento % ya tiene una ruta rechazada guardada para la adjunción %. No se permiten duplicados en el mismo proceso.',
            p_claveDocAspirante, p_claveAdjuncion;
    END IF;

    -- A. Si el documento ya está Aceptado en la adjunción operativa, no permitimos alterarlo
    IF v_estatus_doc_adjuncion = 'Aceptado' THEN
        RAISE EXCEPTION 'Error de Blindaje: No se puede registrar un rechazo para el documento % porque su estado actual en la adjunción ya es "Aceptado".',
            p_claveDocAspirante;
    END IF;

    -- B. Si el estado en la revisión no es 'Rechazado' (con Z, emparejado con tu CHECK)
    IF v_estatus_doc_revision <> 'Rechazado' THEN 
        RAISE EXCEPTION 'Error de Regla de Negocio: No se puede respaldar el documento %. Su estado en la revisión es "%", solo se permiten documentos "Rechazado".', 
            p_claveDocAspirante, v_estatus_doc_revision;
    END IF;

    -- =================================================================
    --  SI PASÓ TODAS LAS ADUANAS, SE INSERTA SEGURO
    -- =================================================================
    INSERT INTO "RutasRechazadas" (
        "claveAdjuncion",
        "claveDocAspirante",
        "claveRevision",
        "ruta_archivo_rechazado"
    ) VALUES (
        p_claveAdjuncion,
        p_claveDocAspirante,
        p_claveRevision,
        v_ruta_actual
    );

    RAISE NOTICE 'Inserción Exitosa: La ruta del documento % fue congelada en la revisión %.', 
        p_claveDocAspirante, p_claveRevision;

END;
$$;