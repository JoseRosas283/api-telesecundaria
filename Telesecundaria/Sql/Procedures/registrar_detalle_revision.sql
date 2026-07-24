CREATE OR REPLACE PROCEDURE registrar_detalle_revision(
    p_claveRevision VARCHAR(18),
    p_claveDocAspirante VARCHAR(18),
    p_estatus_doc VARCHAR(50),
    p_motivo_rechazo TEXT
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_estatus_revision_global VARCHAR(50);
    v_estatus_documento_previo VARCHAR(50);
    v_adj_revision VARCHAR(18);
    v_adj_documento VARCHAR(18);
BEGIN
    -- 1. VALIDACIÓN DE PERTENENCIA A LA ADJUNCIÓN (La Cápsula)
    -- Primero obtenemos la adjunción "madre" de la revisión actual
    SELECT "claveAdjuncion" INTO v_adj_revision FROM "Revisiones" WHERE "claveRevision" = p_claveRevision;
    
    -- AJUSTE CLAVE: Buscamos el documento PERO filtrando por la adjunción de la revisión.
    -- Esto evita que el sistema se confunda con registros de adjunciones rechazadas anteriormente.
    SELECT "claveAdjuncion" INTO v_adj_documento 
    FROM "DetalleAdjuncion" 
    WHERE "claveDocAspirante" = p_claveDocAspirante 
      AND "claveAdjuncion" = v_adj_revision -- <--- ESTA ES LA LLAVE QUE QUITA LA CONFUSIÓN
    LIMIT 1;

    -- Si no lo encuentra bajo esta adjunción específica, lanzamos el error
    IF v_adj_documento IS NULL THEN
        RAISE EXCEPTION 'El documento % no pertenece a la Adjunción % vinculada a esta revisión.', p_claveDocAspirante, v_adj_revision;
    END IF;

    -- 2. BUSCAR ANTECEDENTES (Cruzamos Detalle con su Cabecera)
    SELECT r.estatus_revision, d.estatus_doc 
    INTO v_estatus_revision_global, v_estatus_documento_previo
    FROM "DetalleRevision" d
    JOIN "Revisiones" r ON d."claveRevision" = r."claveRevision"
    WHERE d."claveDocAspirante" = p_claveDocAspirante
    ORDER BY r.fecha_revision DESC 
    LIMIT 1;

    -- 3. APLICACIÓN DE TUS REGLAS DE NEGOCIO
    IF v_estatus_revision_global IN ('Pendiente', 'Aceptada') THEN
        RAISE EXCEPTION 'No se puede insertar: El documento ya está vinculado a una revisión %.', v_estatus_revision_global;
    
    ELSIF v_estatus_revision_global = 'Rechazada' THEN
        IF v_estatus_documento_previo = 'Aceptado' THEN
            RAISE EXCEPTION 'No se puede insertar: Este documento ya fue Aceptado individualmente en una revisión previa rechazada.';
        END IF;
    END IF;

    -- 4. VALIDACIÓN DE MOTIVO OBLIGATORIO
    IF p_estatus_doc = 'Rechazado' AND (p_motivo_rechazo IS NULL OR TRIM(p_motivo_rechazo) = '') THEN
        RAISE EXCEPTION 'Debe especificar el motivo de rechazo para el documento %.', p_claveDocAspirante;
    END IF;

    -- 5. INSERCIÓN
    INSERT INTO "DetalleRevision" (
        "claveRevision", "claveDocAspirante", estatus_doc, motivo_rechazo
    ) VALUES (
        p_claveRevision, p_claveDocAspirante, p_estatus_doc, 
        CASE WHEN p_estatus_doc = 'Rechazado' THEN TRIM(p_motivo_rechazo) ELSE NULL END
    );

END;
$$;