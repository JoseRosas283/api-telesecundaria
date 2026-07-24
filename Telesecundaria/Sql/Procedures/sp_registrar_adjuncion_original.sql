CREATE OR REPLACE PROCEDURE sp_registrar_adjuncion_original(
    p_claveEntrega VARCHAR(18),
    p_claveUsuario VARCHAR(18)
)
AS $$
DECLARE
    v_estado_entrega VARCHAR(20);
    v_usuario_entrega VARCHAR(18); -- Variable agregada para la validación
    v_es_docente BOOLEAN;
BEGIN
    -- 1. SEGURIDAD: Validar que el usuario operativo NO sea un Docente activo
    SELECT EXISTS (
        SELECT 1 
        FROM "Usuarios" u
        INNER JOIN "Empleados" e ON u."claveEmpleado" = e."claveEmpleado"
        INNER JOIN "EmpleadoRol" er ON e."claveEmpleado" = er."claveEmpleado"
        INNER JOIN "Roles" r ON er."claveRol" = r."claveRol"
        WHERE u."claveUsuario" = p_claveUsuario
          AND r."nombre_rol" = 'Docente'
          AND er."fecha_fin" IS NULL
    ) INTO v_es_docente;

    IF v_es_docente THEN
        RAISE EXCEPTION 'Acceso denegado: El personal con rol de Docente no está autorizado para cargar expedientes originales.';
    END IF;

    -- 2. INTEGRIDAD: Validar que la entrega exista y esté activa (Pendiente)
    SELECT "estado_final", "claveUsuario" INTO v_estado_entrega, v_usuario_entrega -- Se agrega claveUsuario al SELECT
    FROM "Entregas"
    WHERE "claveEntrega" = p_claveEntrega;

    IF v_estado_entrega IS NULL THEN
        RAISE EXCEPTION 'Error: La entrega con clave % no existe.', p_claveEntrega;
    END IF;

    -- VALIDACIÓN OPERATIVA: El usuario que adjunta debe ser el mismo que registró la entrega
    IF v_usuario_entrega <> p_claveUsuario THEN
        RAISE EXCEPTION 'Acceso denegado: El usuario % no está autorizado. Esta entrega pertenece al usuario %.', p_claveUsuario, v_usuario_entrega;
    END IF;

    -- Si la entrega ya se cerró como Completada o Incompleta, ya no se permiten cambios
    IF v_estado_entrega <> 'pendiente' THEN
        RAISE EXCEPTION 'Error: No se pueden adjuntar archivos porque la entrega ya se encuentra en estado "%".', v_estado_entrega;
    END IF;

    -- 3. RESTRICCIÓN FIXED (1:1): Asegurar que no exista ya un registro para esta entrega
    -- Aunque la tabla tiene el UNIQUE, el SP frena el proceso de forma limpia antes de tronar
    IF EXISTS (SELECT 1 FROM "AdjuncionesOriginales" WHERE "claveEntrega" = p_claveEntrega) THEN
        RAISE EXCEPTION 'Error de duplicidad: Esta entrega ya cuenta con un expediente digital original registrado.';
    END IF;

    -- 4. INSERCIÓN DIRECTA
    INSERT INTO "AdjuncionesOriginales" (
        "claveEntrega", 
        "claveUsuario"
    )
    VALUES (
        p_claveEntrega, 
        p_claveUsuario
    );

    RAISE NOTICE 'Éxito: Expediente digital original registrado correctamente para la entrega %.', p_claveEntrega;

END;
$$ LANGUAGE plpgsql;