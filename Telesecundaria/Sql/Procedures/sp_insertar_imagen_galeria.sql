CREATE OR REPLACE PROCEDURE sp_insertar_imagen_galeria(
    p_nombre_archivo VARCHAR(100),
    p_ruta_url TEXT,
    p_tipo_recurso VARCHAR(25)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_existe_nombre INTEGER;
    v_existe_url INTEGER;
BEGIN
    -- 1. VALIDACIÓN DE CAMPOS NULOS O VACÍOS
    IF p_nombre_archivo IS NULL OR TRIM(p_nombre_archivo) = '' OR 
       p_ruta_url IS NULL OR TRIM(p_ruta_url) = '' OR 
       p_tipo_recurso IS NULL OR TRIM(p_tipo_recurso) = '' THEN
        RAISE EXCEPTION 'Error: Todos los campos (nombre, ruta y tipo) son obligatorios y no pueden estar vacíos.';
    END IF;

    -- 2. VALIDACIÓN DEL CHECK (Catálogo de tipos de recurso)
    IF TRIM(p_tipo_recurso) NOT IN ('Eventos Culturales', 'Noticia', 'Aviso', 'Convocatorias','Galería', 'otros') THEN
        RAISE EXCEPTION 'Bloqueo: El tipo de recurso "%" no es válido. Use: Eventos Culturales, Noticia, Aviso, Convocatorias u otros.', p_tipo_recurso;
    END IF;

    -- 3. VALIDACIÓN DE UNICIDAD (No repetir nombre de archivo)
    SELECT COUNT(*) INTO v_existe_nombre 
    FROM "GaleriaImagenes" 
    WHERE UPPER(TRIM(nombre_archivo)) = UPPER(TRIM(p_nombre_archivo));

    IF v_existe_nombre > 0 THEN
        RAISE EXCEPTION 'Error: El nombre de archivo "%" ya existe en la galería.', p_nombre_archivo;
    END IF;

    -- 4. VALIDACIÓN DE UNICIDAD (No repetir ruta URL)
    SELECT COUNT(*) INTO v_existe_url 
    FROM "GaleriaImagenes" 
    WHERE TRIM(ruta_url) = TRIM(p_ruta_url);

    IF v_existe_url > 0 THEN
        RAISE EXCEPTION 'Error: La ruta URL "%" ya está registrada para otra imagen.', p_ruta_url;
    END IF;

    -- 5. INSERCIÓN (La clave y la fecha son automáticas por el DEFAULT de la tabla)
    INSERT INTO "GaleriaImagenes" (
        nombre_archivo,
        ruta_url,
        tipo_recurso
    ) VALUES (
        TRIM(p_nombre_archivo),
        TRIM(p_ruta_url),
        TRIM(p_tipo_recurso)
    );

    RAISE NOTICE 'Éxito: La imagen "%" se ha registrado correctamente en la categoría %.', 
                 p_nombre_archivo, p_tipo_recurso;

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Operación fallida: %', SQLERRM;
END;
$$;