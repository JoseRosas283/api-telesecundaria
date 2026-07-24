CREATE OR REPLACE PROCEDURE sp_gestionar_permisos_por_nombre(
    p_nombre_rol VARCHAR,
    p_nombre_modulo VARCHAR,
    p_puede_ver VARCHAR,      -- 'Puede' o 'No puede'
    p_puede_crear VARCHAR,    -- NULL si es Padre
    p_puede_editar VARCHAR,   -- NULL si es Padre
    p_puede_eliminar VARCHAR, -- NULL si es Padre
    OUT p_exito BOOLEAN,
    OUT p_mensaje VARCHAR
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_clave_rol VARCHAR(20);
    v_clave_modulo VARCHAR(20);
    v_clave_padre VARCHAR(18);
    v_nombre_padre VARCHAR(50);
    v_es_padre BOOLEAN;
    v_ver BOOLEAN;
    v_crear BOOLEAN;
    v_editar BOOLEAN;
    v_eliminar BOOLEAN;
    v_padre_autorizado BOOLEAN;
BEGIN
    -- 1. BÚSQUEDA DE CLAVES Y TIPO DE MÓDULO
    SELECT "claveRol" INTO v_clave_rol FROM "Roles" WHERE nombre_rol = p_nombre_rol;
    
    SELECT "claveModulo", "claveModuloPadre", ("claveModuloPadre" IS NULL) 
    INTO v_clave_modulo, v_clave_padre, v_es_padre 
    FROM "Modulos" WHERE nombre_modulo = p_nombre_modulo;

    -- 2. VALIDACIÓN DE EXISTENCIA BÁSICA
    IF v_clave_rol IS NULL OR v_clave_modulo IS NULL THEN
        p_exito := FALSE;
        p_mensaje := 'Error: Rol o Módulo no encontrados.';
        RETURN;
    END IF;

    -- 3. TRADUCCIÓN DEL PERMISO MAESTRO
    v_ver := (LOWER(TRIM(p_puede_ver)) = 'puede');

    -- 4. VALIDACIÓN DE JERARQUÍA Y COHERENCIA
    IF v_es_padre THEN
        -- REGLA PARA PADRES
        IF p_puede_crear IS NOT NULL OR p_puede_editar IS NOT NULL OR p_puede_eliminar IS NOT NULL THEN
            p_exito := FALSE;
            p_mensaje := 'Error: Para módulos Padre, los campos Crear, Editar y Eliminar deben ser NULL.';
            RETURN;
        END IF;
        v_crear := FALSE; v_editar := FALSE; v_eliminar := FALSE;
    
    ELSE
        -- REGLA PARA HIJOS
        -- A. Obligar valores en hijos (Evitar NULLs)
        IF p_puede_crear IS NULL OR p_puede_editar IS NULL OR p_puede_eliminar IS NULL THEN
            p_exito := FALSE;
            p_mensaje := 'Error: Para módulos Hijo, los campos Crear, Editar y Eliminar NO pueden ser NULL.';
            RETURN;
        END IF;

        -- B. Verificar relación y permiso del Padre
        SELECT puede_ver INTO v_padre_autorizado FROM "Permisos" 
        WHERE "claveRol" = v_clave_rol AND "claveModulo" = v_clave_padre;

        IF v_padre_autorizado IS NULL THEN
            SELECT nombre_modulo INTO v_nombre_padre FROM "Modulos" WHERE "claveModulo" = v_clave_padre;
            p_exito := FALSE;
            p_mensaje := 'Error: El Rol no está relacionado al módulo Padre "' || v_nombre_padre || '".';
            RETURN;
        ELSIF v_padre_autorizado = FALSE THEN
            SELECT nombre_modulo INTO v_nombre_padre FROM "Modulos" WHERE "claveModulo" = v_clave_padre;
            p_exito := FALSE;
            p_mensaje := 'Error: El módulo Padre "' || v_nombre_padre || '" tiene el acceso denegado. No se pueden gestionar hijos.';
            RETURN;
        END IF;

        -- C. VALIDACIÓN DE COHERENCIA (DETENER EN SECO)
        IF v_ver = FALSE THEN
            -- Si intenta poner "Puede" en cualquier acción pero "No puede" en ver, RECHAZAMOS.
            IF (LOWER(TRIM(p_puede_crear)) = 'puede' OR 
                LOWER(TRIM(p_puede_editar)) = 'puede' OR 
                LOWER(TRIM(p_puede_eliminar)) = 'puede') THEN
                p_exito := FALSE;
                p_mensaje := 'Error de Coherencia: No se pueden asignar acciones (Crear/Editar/Eliminar) si el acceso visual está desactivado.';
                RETURN;
            END IF;
            -- Si llega aquí es porque v_ver es false y todas las acciones son 'no puede'
            v_crear := FALSE; v_editar := FALSE; v_eliminar := FALSE;
        ELSE
            -- Si ve = true, tomamos los valores del usuario
            v_crear    := (LOWER(TRIM(p_puede_crear)) = 'puede');
            v_editar   := (LOWER(TRIM(p_puede_editar)) = 'puede');
            v_eliminar := (LOWER(TRIM(p_puede_eliminar)) = 'puede');
        END IF;
    END IF;

    -- 5. UPSERT (Solo se ejecuta si pasó todas las validaciones anteriores)
    INSERT INTO "Permisos" ("claveRol", "claveModulo", puede_ver, puede_crear, puede_editar, puede_eliminar) 
    VALUES (v_clave_rol, v_clave_modulo, v_ver, v_crear, v_editar, v_eliminar)
    ON CONFLICT ("claveRol", "claveModulo") 
    DO UPDATE SET
        puede_ver = EXCLUDED.puede_ver, puede_crear = EXCLUDED.puede_crear,
        puede_editar = EXCLUDED.puede_editar, puede_eliminar = EXCLUDED.puede_eliminar,
        fecha_asignacion = CURRENT_TIMESTAMP;

    p_exito := TRUE;
    p_mensaje := 'Configuración guardada exitosamente.';

EXCEPTION WHEN OTHERS THEN
    p_exito := FALSE; p_mensaje := 'Error técnico: ' || SQLERRM;
END;
$$;