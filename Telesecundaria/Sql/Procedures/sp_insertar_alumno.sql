CREATE OR REPLACE PROCEDURE sp_insertar_alumno(
    p_clave_expediente VARCHAR(18)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_matricula_generada VARCHAR(10);
    v_existe_expediente BOOLEAN;
    v_tipo_titular VARCHAR(20); -- Variable para validar el tipo
BEGIN
    -- ============================================================
    -- 1. VALIDACIÓN DE OBLIGATORIEDAD
    -- ============================================================
    IF p_clave_expediente IS NULL OR TRIM(p_clave_expediente) = '' THEN
        RAISE EXCEPTION 'Error: La clave de expediente es obligatoria para dar de alta a un alumno.';
    END IF;

    -- ============================================================
    -- 2. VALIDACIÓN DE INTEGRIDAD Y TIPO DE TITULAR
    -- ============================================================
    -- Obtenemos el tipo directamente para validar existencia y rol en un solo paso
    SELECT "tipo_titular" INTO v_tipo_titular 
    FROM "Expedientes" 
    WHERE "claveExpediente" = p_clave_expediente;

    IF v_tipo_titular IS NULL THEN
        RAISE EXCEPTION 'Error de integridad: El expediente con clave % no existe.', p_clave_expediente;
    END IF;

    IF v_tipo_titular <> 'Alumno' THEN
    RAISE EXCEPTION 'Error de negocio: El expediente % es de tipo "%" y no cumple con el perfil requerido para ser dado de alta como Alumno.', 
    p_clave_expediente, v_tipo_titular;
      END IF;

    -- Validamos que este expediente no haya sido usado ya por otro alumno
    IF EXISTS (SELECT 1 FROM "Alumnos" WHERE "claveExpediente" = p_clave_expediente) THEN
        RAISE EXCEPTION 'Error: El expediente % ya se encuentra asignado a un alumno oficial.', p_clave_expediente;
    END IF;

    -- ============================================================
    -- 3. GENERACIÓN DE MATRÍCULA DE 10 CARACTERES (Año 2026 + 6 Dígitos)
    -- ============================================================
    v_matricula_generada := (TO_CHAR(CURRENT_DATE, 'YYYY') || LPAD(FLOOR(RANDOM() * 999999)::TEXT, 6, '0'));

    IF LENGTH(v_matricula_generada) <> 10 THEN
        RAISE EXCEPTION 'Error interno: Falla al calcular los 10 caracteres de la matrícula (%).', v_matricula_generada;
    END IF;

    -- ============================================================
    -- 4. INSERCIÓN OFICIAL (Sin grado ni grupo, versión limpia)
    -- ============================================================
    INSERT INTO "Alumnos" (
        "matricula",
        "estado",
        "claveExpediente"
    ) VALUES (
        v_matricula_generada,
        'Activo',
        p_clave_expediente
    );

    RAISE NOTICE 'Éxito: Alumno oficial creado con Matrícula: %, Estado: Activo.', 
        v_matricula_generada;

EXCEPTION
    WHEN unique_violation THEN
        RAISE EXCEPTION 'Error de duplicidad: La matrícula % ya existe en el sistema. Intente de nuevo.', v_matricula_generada;
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error crítico en sp_insertar_alumno: %', SQLERRM;
END;
$$;