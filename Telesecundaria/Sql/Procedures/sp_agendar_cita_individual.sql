CREATE OR REPLACE PROCEDURE sp_agendar_cita_individual(
    p_clave_revision VARCHAR(18),
    p_fecha_cita DATE,
    p_hora_cita TIME
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_clave_tutor VARCHAR(18);
    v_anio_actual INTEGER := EXTRACT(YEAR FROM CURRENT_DATE);
    v_estado_buffer BOOLEAN;
BEGIN
    -- ==========================================================
    -- 1. EL FILTRO DE SEGURIDAD (Tu punto clave)
    -- ==========================================================
    -- Buscamos el estado del booleano directamente
    SELECT "Estado" INTO v_estado_buffer
    FROM "RevisionesAceptadas" 
    WHERE "claveRevision" = p_clave_revision;

    -- A) Si es NULL, significa que la revisión ni siquiera fue aceptada (No existe en el buffer)
    IF v_estado_buffer IS NULL THEN
        RAISE EXCEPTION 'Acceso Denegado: La revisión % no existe en el registro de aceptados.', p_clave_revision;
    END IF;

    -- B) Si es FALSE, significa que ya se le generó una cita anteriormente
    IF v_estado_buffer = FALSE THEN
        RAISE EXCEPTION 'Operación Bloqueada: La revisión % ya tiene una cita procesada.', p_clave_revision;
    END IF;

    -- ==========================================================
    -- 2. VALIDACIONES DE REGLAS DE AGENDA (Secundaria)
    -- ==========================================================
    -- Fecha: Mayor a hoy y dentro del año actual
    IF p_fecha_cita <= CURRENT_DATE THEN
        RAISE EXCEPTION 'Fecha Inválida: La cita debe ser a partir de mañana.';
    END IF;

    IF EXTRACT(YEAR FROM p_fecha_cita) > v_anio_actual THEN
        RAISE EXCEPTION 'Año Inválido: Solo se agenda dentro del ciclo escolar actual (%).', v_anio_actual;
    END IF;

    -- Hora: Horario Matutino Escolar (08:00 a 12:00)
    IF p_hora_cita < '08:00:00' OR p_hora_cita > '12:00:00' THEN
        RAISE EXCEPTION 'Horario Inválido: La secundaria solo recibe citas de 08:00 AM a 12:00 PM.';
    END IF;

    -- ==========================================================
    -- 3. EXTRACCIÓN Y REGISTRO
    -- ==========================================================
    -- Sacamos la clave del tutor desde la adjunción relacionada
    SELECT adj."claveTutorAspirante" 
    INTO v_clave_tutor
    FROM "Revisiones" rev
    INNER JOIN "Adjunciones" adj ON rev."claveAdjuncion" = adj."claveAdjuncion"
    WHERE rev."claveRevision" = p_clave_revision;

    -- Inserción final en la agenda
    INSERT INTO "CitasInscripcion" (
        fecha_cita, 
        hora_cita, 
        "claveRevision", 
        "claveTutorAspirante",
        estado_cita
    ) VALUES (
        p_fecha_cita,
        p_hora_cita,
        p_clave_revision,
        v_clave_tutor,
        'Programada'
    );

    -- 4. EL TRUCO DEL BOOLEANO: "Quemamos" el ticket de entrada
    UPDATE "RevisionesAceptadas" 
    SET "Estado" = FALSE 
    WHERE "claveRevision" = p_clave_revision;

    RAISE NOTICE 'Éxito: Cita vinculada a la revisión % agendada correctamente.', p_clave_revision;

EXCEPTION 
    WHEN unique_violation THEN
        RAISE EXCEPTION 'Error de Integridad: Ya existe una cita para esta revisión o tutor.';
END;
$$;
