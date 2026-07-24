CREATE OR REPLACE PROCEDURE registrar_aceptacion_buffer(
    p_claveRevision VARCHAR(18)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_claveReceptor      VARCHAR(18);
    v_claveConvocatoria   VARCHAR(18);
    v_claveAspirante     VARCHAR(18);
    v_estatus_revision   VARCHAR(50);
BEGIN
    -- 1. REALIZAMOS LOS VIAJES Y OBTENEMOS EL ESTATUS
    SELECT 
        r."claveReceptor", 
        asp."claveConvocatoria",
        asp."claveAspirante",
        rev.estatus_revision
    INTO 
        v_claveReceptor, 
        v_claveConvocatoria,
        v_claveAspirante,
        v_estatus_revision
    FROM "Revisiones" rev
    JOIN "Adjunciones" adj ON rev."claveAdjuncion" = adj."claveAdjuncion"
    JOIN "Aspirantes" asp ON adj."claveAspirante" = asp."claveAspirante"
    JOIN "Receptores" r ON adj."claveTutorAspirante" = r."claveTutorAspirante"
    WHERE rev."claveRevision" = p_claveRevision;

    -- 2. VALIDACIÓN DE SEGURIDAD (El Filtro de Estatus)
    IF v_estatus_revision IS NULL THEN
        RAISE EXCEPTION 'La revisión % no existe.', p_claveRevision;
    END IF;

    IF v_estatus_revision <> 'Aceptada' THEN
        RAISE EXCEPTION 'Error: La revisión % tiene estatus "%". Solo las revisiones "Aceptada" pueden entrar al buffer de citas.', 
            p_claveRevision, v_estatus_revision;
    END IF;

    -- Validamos que los viajes secundarios hayan tenido éxito
    IF v_claveReceptor IS NULL OR v_claveConvocatoria IS NULL THEN
        RAISE EXCEPTION 'No se pudo recuperar la información de contacto o convocatoria para la revisión %', p_claveRevision;
    END IF;

    -- 3. INSERCIÓN EN EL PULMÓN (Buffer)
    INSERT INTO "RevisionesAceptadas" (
        "claveRevision",
        "claveReceptor",
        "claveConvocatoria"
    ) VALUES (
        p_claveRevision,
        v_claveReceptor,
        v_claveConvocatoria
    );

    RAISE NOTICE 'Aspirante % encapsulado exitosamente en RevisionesAceptadas (Convocatoria: %).', 
        v_claveAspirante, v_claveConvocatoria;

END;
$$;