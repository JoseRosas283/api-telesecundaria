CREATE OR REPLACE PROCEDURE sp_eliminar_tutor(
    p_claveTutor VARCHAR(18)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_nombre_tutor VARCHAR(50);
    v_existe_receptor BOOLEAN;
    v_tiene_alumno_activo BOOLEAN;
BEGIN
    -- ============================================================
    -- 1. VALIDACIÓN DE EXISTENCIA FÍSICA
    -- ============================================================
    SELECT "nombre" INTO v_nombre_tutor
    FROM "Tutores"
    WHERE "claveTutor" = p_claveTutor;

    -- Si la consulta no arrojó resultados, el tutor no existe
    IF NOT FOUND THEN
        RAISE EXCEPTION 'Error de Eliminación: El tutor con clave % no existe en la base de datos.', p_claveTutor;
    END IF;

    -- ============================================================
    -- 1.5 CANDADO DE INTEGRIDAD: VÍNCULO ACTIVO CON ALUMNO
    -- ============================================================
    -- Un tutor tiene un alumno activo si existe en TutoresAlumnos y fecha_baja ES NULL
    SELECT EXISTS (
        SELECT 1 
        FROM "TutoresAlumnos" 
        WHERE "claveTutor" = p_claveTutor 
          AND "fecha_baja" IS NULL
    ) INTO v_tiene_alumno_activo;

    IF v_tiene_alumno_activo THEN
        RAISE EXCEPTION 'Bloqueo de Integridad: El tutor % no puede desactivarse porque actualmente tiene una relación ACTIVA con un alumno en la tabla TutoresAlumnos.', p_claveTutor;
    END IF;

    -- ============================================================
    -- 2. DESACTIVACIÓN LÓGICA DEL TUTOR
    -- ============================================================
    UPDATE "Tutores" SET 
        "estado" = FALSE 
    WHERE "claveTutor" = p_claveTutor;

    -- ============================================================
    -- 3. DESACTIVACIÓN LÓGICA DE SU RECEPTOR VINCULADO
    -- ============================================================
    -- Verificamos si cuenta con un receptor activo en el sistema para apagarlo
    SELECT EXISTS (
        SELECT 1 FROM "Receptores" WHERE "claveTutor" = p_claveTutor
    ) INTO v_existe_receptor;

    IF v_existe_receptor THEN
        UPDATE "Receptores" SET 
            "estado" = FALSE 
        WHERE "claveTutor" = p_claveTutor;
    END IF;

    RAISE NOTICE 'Éxito: El tutor % y su receptor asociado han sido desactivados del sistema correctamente.', v_nombre_tutor;

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Fallo en la operación de eliminación/desactivación: %', SQLERRM;
END;
$$;