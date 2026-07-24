CREATE OR REPLACE VIEW "v_empleados_roles_basica" AS
SELECT 
    -- Datos del Empleado
    e."claveEmpleado",
    e.tipo_contrato,
    e.estatus_laboral,
    e.telefono,
    
    -- Datos del Rol obtenido mediante la relación
    r.nombre_rol,
    
    -- Datos de la asignación
    er.fecha_inicio,
    er.fecha_fin
FROM "Empleados" e
JOIN "EmpleadoRol" er ON e."claveEmpleado" = er."claveEmpleado"
JOIN "Roles" r ON er."claveRol" = r."claveRol";