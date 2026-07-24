CREATE OR REPLACE PROCEDURE sp_insertar_modulo(
    p_nombre_modulo VARCHAR(50),
    p_descripcion TEXT,
    p_url_modulo VARCHAR(100),
    p_claveModuloPadre VARCHAR(18) DEFAULT NULL
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_existe_nombre BOOLEAN;
    v_url_en_uso_por_otro BOOLEAN;
    v_padre_existe BOOLEAN;
    v_url_padre VARCHAR(100);
BEGIN
    -- 1. VALIDACIÓN: Nombre único
    SELECT EXISTS (SELECT 1 FROM "Modulos" WHERE nombre_modulo = p_nombre_modulo) INTO v_existe_nombre;
    IF v_existe_nombre THEN
        RAISE EXCEPTION 'Error: El nombre del módulo "%" ya está registrado.', p_nombre_modulo;
    END IF;

    -- 2. VALIDACIÓN: URL inteligente
    IF p_url_modulo IS NOT NULL THEN
        -- Obtenemos la URL del padre (si tiene uno)
        IF p_claveModuloPadre IS NOT NULL THEN
            SELECT url_modulo INTO v_url_padre FROM "Modulos" WHERE "claveModulo" = p_claveModuloPadre;
        END IF;

        -- Buscamos si la URL ya existe en otro módulo que NO sea su padre directo
        SELECT EXISTS (
            SELECT 1 FROM "Modulos" 
            WHERE url_modulo = p_url_modulo 
            AND (p_claveModuloPadre IS NULL OR "claveModulo" <> p_claveModuloPadre)
        ) INTO v_url_en_uso_por_otro;

        IF v_url_en_uso_por_otro THEN
            RAISE EXCEPTION 'Error: La URL "%" ya está en uso por otro módulo que no es el padre.', p_url_modulo;
        END IF;
    END IF;

    -- 3. VALIDACIÓN: Existencia del Padre
    IF p_claveModuloPadre IS NOT NULL THEN
        SELECT EXISTS (SELECT 1 FROM "Modulos" WHERE "claveModulo" = p_claveModuloPadre) INTO v_padre_existe;
        IF NOT v_padre_existe THEN
            RAISE EXCEPTION 'Error: El módulo padre con clave "%" no existe.', p_claveModuloPadre;
        END IF;
    END IF;

    -- 4. INSERCIÓN
    INSERT INTO "Modulos" (
        nombre_modulo, 
        descripcion, 
        url_modulo, 
        "claveModuloPadre"
    ) VALUES (
        p_nombre_modulo, 
        p_descripcion, 
        p_url_modulo, 
        p_claveModuloPadre
    );

    RAISE NOTICE 'Éxito: Módulo "%" insertado correctamente.', p_nombre_modulo;

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Fallo en la inserción: %', SQLERRM;
END;
$$;