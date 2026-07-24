CREATE OR REPLACE PROCEDURE sp_asignar_rol_empleado(
    p_claveEmpleado VARCHAR(18),
    p_nombreRol VARCHAR(20)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_claveRol VARCHAR(18);
    v_tiene_rol_activo BOOLEAN;
BEGIN
    -- 1. IDENTIFICACIÓN DEL ROL POR NOMBRE
    SELECT "claveRol" INTO v_claveRol 
    FROM "Roles" 
    WHERE nombre_rol = p_nombreRol;

    IF v_claveRol IS NULL THEN
        RAISE EXCEPTION 'Error: El rol "%" no existe en el catálogo.', p_nombreRol;
    END IF;

    -- 2. VALIDACIÓN DE EMPLEADO EXISTENTE
    IF NOT EXISTS (SELECT 1 FROM "Empleados" WHERE "claveEmpleado" = p_claveEmpleado) THEN
        RAISE EXCEPTION 'Error: El empleado con clave % no existe.', p_claveEmpleado;
    END IF;

    -- 3. REGLA DE ORO: VALIDAR ROL ACTIVO
    -- Un rol está activo si fecha_fin ES NULL
    SELECT EXISTS (
        SELECT 1 FROM "EmpleadoRol" 
        WHERE "claveEmpleado" = p_claveEmpleado 
        AND fecha_fin IS NULL
    ) INTO v_tiene_rol_activo;

    IF v_tiene_rol_activo THEN
        RAISE EXCEPTION 'Bloqueo: El empleado % ya tiene un rol activo. Debe finalizar el actual antes de asignar uno nuevo.', p_claveEmpleado;
    END IF;

    -- 4. INSERCIÓN
    -- fecha_inicio se toma por DEFAULT CURRENT_DATE de la tabla
    -- fecha_fin se queda NULL por defecto
    INSERT INTO "EmpleadoRol" (
        "claveEmpleado",
        "claveRol"
    ) VALUES (
        p_claveEmpleado,
        v_claveRol
    );

    RAISE NOTICE 'Éxito: Se ha asignado el rol "%" al empleado %.', p_nombreRol, p_claveEmpleado;

END;
$$;
