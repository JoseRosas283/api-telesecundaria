CREATE OR REPLACE PROCEDURE sp_insertar_tipo_documento(
    p_nombre_documento VARCHAR(50),
    p_area VARCHAR(20),
    p_descripcion TEXT
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_nombre_limpio VARCHAR(50);
    v_area_limpia   VARCHAR(20); -- Nueva variable para asegurar el hábitat
    v_desc_limpia   TEXT;
BEGIN
    -- ==========================================
    -- 1. LIMPIEZA Y PREPARACIÓN DE DATOS
    -- ==========================================
    v_nombre_limpio := UPPER(TRIM(p_nombre_documento));
    v_desc_limpia   := TRIM(p_descripcion);
    v_area_limpia   := TRIM(p_area); -- Aseguramos que el área no traiga basura/espacios

    -- ==========================================
    -- 2. VALIDACIONES ESTRICTAS DE NULIDAD Y VACÍOS
    -- ==========================================
    
    -- Validar Nombre
    IF v_nombre_limpio IS NULL OR v_nombre_limpio = '' THEN
        RAISE EXCEPTION 'Error: El nombre del documento es obligatorio y no puede estar vacío.';
    END IF;

    -- Validar Área
    IF v_area_limpia IS NULL OR v_area_limpia = '' THEN
        RAISE EXCEPTION 'Error: El área (Nativa) es obligatoria.';
    END IF;

    -- Validar Descripción
    IF v_desc_limpia IS NULL OR v_desc_limpia = '' THEN
        RAISE EXCEPTION 'Error: La descripción es obligatoria. Debe explicar para qué sirve el documento.';
    END IF;

    -- ==========================================
    -- 3. VALIDACIÓN DE IDENTIDAD UNIQUE (GLOBAL)
    -- ==========================================
    -- Comparamos contra el dato ya guardado en la tabla (que debe estar normalizado)
    IF EXISTS (
        SELECT 1 FROM "TipoDocumentos" 
        WHERE nombre_documento = v_nombre_limpio
    ) THEN
        RAISE EXCEPTION 'Error: El documento "%" ya existe. No se puede duplicar un documento nativo.', 
                        p_nombre_documento;
    END IF;

    -- ==========================================
    -- 4. VALIDACIÓN DE DOMINIO (ÁREAS PERMITIDAS)
    -- ==========================================
    -- Se verifica contra la lista oficial (Debe coincidir con el CHECK de la tabla)
    IF v_area_limpia NOT IN ('Preinscripción', 'Inscripción', 'Becas', 'Egreso', 'Laboral', 'Institucional') THEN
        RAISE EXCEPTION 'Error: El área "%" no es válida. Verifique el catálogo de áreas permitidas.', v_area_limpia;
    END IF;

    -- ==========================================
    -- 5. INSERCIÓN FINAL
    -- ==========================================
    -- Se añade de forma explícita la columna 'estado' con valor TRUE por defecto al nacer
    INSERT INTO "TipoDocumentos" (
        nombre_documento,
        area,
        descripcion,
        estado
    ) VALUES (
        v_nombre_limpio,
        v_area_limpia,
        v_desc_limpia,
        TRUE
    );

    RAISE NOTICE 'Éxito: Documento "%" registrado correctamente como nativo de %.', 
                 v_nombre_limpio, v_area_limpia;
END;
$$;
