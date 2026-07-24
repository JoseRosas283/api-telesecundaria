CREATE OR REPLACE PROCEDURE sp_asignar_tutor_alumno(
    p_claveAlumno    VARCHAR(18),
    p_claveTutor     VARCHAR(18)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_claveExpediente VARCHAR(18);
BEGIN
    -- ============================================================
    -- 1. VALIDACIÓN DE EXISTENCIA (ALUMNO Y TUTOR)
    -- ============================================================
    SELECT "claveExpediente" INTO v_claveExpediente 
    FROM "Alumnos" WHERE "claveAlumno" = p_claveAlumno;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Error: El alumno con clave "%" no existe.', p_claveAlumno;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM "Tutores" WHERE "claveTutor" = p_claveTutor) THEN
        RAISE EXCEPTION 'Error: El tutor con clave "%" no existe.', p_claveTutor;
    END IF;

    -- ============================================================
    -- 2. VALIDACIÓN DE EXPEDIENTE (DOCUMENTOS)
    -- ============================================================
    -- El Alumno debe tener documentos para poder asignarle su tutor
    IF NOT EXISTS (SELECT 1 FROM "Documentos" WHERE "claveExpediente" = v_claveExpediente) THEN
        RAISE EXCEPTION 'Bloqueo: El alumno % no tiene documentos cargados. No se puede asignar tutor.', p_claveAlumno;
    END IF;

    -- ============================================================
    -- 3. VALIDACIÓN DE EXCLUSIVIDAD
    -- ============================================================
    -- Un alumno solo puede tener UN tutor activo (fecha_baja es NULL)
    IF EXISTS (
        SELECT 1 FROM "TutoresAlumnos" 
        WHERE "claveAlumno" = p_claveAlumno 
          AND fecha_baja IS NULL
    ) THEN
        RAISE EXCEPTION 'Bloqueo: El alumno ya tiene un tutor activo asignado.';
    END IF;

    -- ============================================================
    -- 4. INSERCIÓN CON VALORES POR DEFECTO MANUALES
    -- ============================================================
    INSERT INTO "TutoresAlumnos" (
        "claveAlumno", 
        "claveTutor",
        fecha_baja  -- Lo insertamos explícitamente como NULL
        -- fecha_inicio: Se omite para que use su DEFAULT CURRENT_TIMESTAMP
    ) VALUES (
        p_claveAlumno, 
        p_claveTutor,
        NULL        -- Nace activo
    );

    RAISE NOTICE 'Éxito: Tutor % vinculado al alumno % (Relación Activa).', p_claveTutor, p_claveAlumno;

END;
$$;