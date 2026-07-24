CREATE OR REPLACE PROCEDURE sp_insertar_tipo_notificacion(
    p_nombre_proceso VARCHAR(50),
    p_descripcion TEXT,
    p_icono VARCHAR(40),
    p_color VARCHAR(9)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_existe_nombre INTEGER;
BEGIN
    -- 0. VALIDACIÓN DE DUPLICADOS (Nueva barrera preventiva)
    SELECT COUNT(*) INTO v_existe_nombre 
    FROM "TipoNotificaciones" 
    WHERE nombre_proceso = TRIM(p_nombre_proceso);

    IF v_existe_nombre > 0 THEN
        RAISE EXCEPTION 'Error: El proceso "%" ya existe en el sistema.', TRIM(p_nombre_proceso);
    END IF;

    -- 1. VALIDACIÓN DE NOMBRE (Null, Vacío y Catálogo)
    IF p_nombre_proceso IS NULL OR TRIM(p_nombre_proceso) = '' THEN
        RAISE EXCEPTION 'Error: El nombre del proceso no puede estar vacío.';
    END IF;

    IF TRIM(p_nombre_proceso) NOT IN (
        'Documentos Rechazados', 'Documentos Aceptados', 'Cierre de Adjuncion','Citas','Inscripciones', 
        'Institucionales', 'Docencia', 'Directivas', 'Administrativas'
    ) THEN
        RAISE EXCEPTION 'Bloqueo: "%" no es un proceso válido en el catálogo oficial.', p_nombre_proceso;
    END IF;

    -- 2. VALIDACIÓN DE DESCRIPCIÓN
    IF p_descripcion IS NULL OR TRIM(p_descripcion) = '' THEN
        RAISE EXCEPTION 'Error: La descripción es obligatoria para informar al usuario.';
    END IF;

    -- 3. VALIDACIÓN DE ICONO (No Null, No Vacío)
    IF p_icono IS NULL OR TRIM(p_icono) = '' THEN
        RAISE EXCEPTION 'Error: El icono es obligatorio para la visualización en el sistema.';
    END IF;

    -- 4. VALIDACIÓN DE COLOR (No Null, No Vacío)
    IF p_color IS NULL OR TRIM(p_color) = '' THEN
        RAISE EXCEPTION 'Error: El color es obligatorio (ej. #FFFFFF o RGB).';
    END IF;

    -- 5. INSERCIÓN (Si todo pasó, limpiamos espacios con TRIM)
    INSERT INTO "TipoNotificaciones" (
        nombre_proceso, 
        descripcion, 
        icono, 
        color
    ) VALUES (
        TRIM(p_nombre_proceso), 
        TRIM(p_descripcion), 
        TRIM(p_icono), 
        TRIM(p_color)
    );

    RAISE NOTICE 'Éxito: Tipo de notificación "%" creado correctamente.', p_nombre_proceso;

EXCEPTION
    WHEN unique_violation THEN
        RAISE EXCEPTION 'Error: El proceso "%" ya existe en el sistema.', p_nombre_proceso;
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Operación cancelada: %', SQLERRM;
END;
$$;