CREATE OR REPLACE PROCEDURE sp_insertar_rol(
    p_nombre_rol VARCHAR(20),
    p_descripcion VARCHAR(100)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_existe_rol BOOLEAN;
    v_nombre_limpio VARCHAR(20);
BEGIN
    -- ==========================================
    -- 1. LIMPIEZA Y NORMALIZACIÓN
    -- ==========================================
    v_nombre_limpio := INITCAP(TRIM(p_nombre_rol));

    -- ==========================================
    -- 2. VALIDACIONES DE NULIDAD Y VACÍO
    -- ==========================================
    IF v_nombre_limpio IS NULL OR v_nombre_limpio = '' THEN
        RAISE EXCEPTION 'Error: El nombre del rol no puede estar vacío.';
    END IF;

    IF p_descripcion IS NULL OR TRIM(p_descripcion) = '' THEN
        RAISE EXCEPTION 'Error: La descripción es obligatoria.';
    END IF;

    -- ==========================================
    -- 3. VALIDACIÓN MANUAL DE OPCIONES (El Check interno)
    -- ==========================================
    IF v_nombre_limpio NOT IN ('Directivo', 'Administrativo', 'Docente', 'Intendente') THEN
        RAISE EXCEPTION 'Bloqueo: El nombre "%" no pertenece a las opciones permitidas por el sistema.', v_nombre_limpio;
    END IF;

    -- ==========================================
    -- 4. VALIDACIÓN DE DUPLICIDAD
    -- ==========================================
    SELECT EXISTS (SELECT 1 FROM "Roles" WHERE nombre_rol = v_nombre_limpio)
    INTO v_existe_rol;

    IF v_existe_rol THEN
        RAISE EXCEPTION 'Bloqueo: El rol "%" ya existe en el catálogo.', v_nombre_limpio;
    END IF;

    -- ==========================================
    -- 5. INSERCIÓN FINAL
    -- ==========================================
    INSERT INTO "Roles" (
        nombre_rol,
        descripcion
    ) VALUES (
        v_nombre_limpio,
        TRIM(p_descripcion)
    );

    RAISE NOTICE 'Éxito: Rol "%" registrado correctamente.', v_nombre_limpio;

EXCEPTION
    -- Capturamos cualquier otro error de integridad por si acaso
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error en procedimiento: %', SQLERRM;
END;
$$;