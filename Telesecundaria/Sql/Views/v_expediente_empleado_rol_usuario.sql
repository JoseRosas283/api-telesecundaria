CREATE OR REPLACE VIEW "v_expediente_empleado_rol_usuario" AS
SELECT 
    -- 1. EL EXPEDIENTE (Identidad)
    ex."claveExpediente" AS "ID_Expediente",
    ex.apellido_paterno || ' ' || COALESCE(ex.apellido_materno, '') || ' ' || ex.nombre AS "Nombre_Titular",
    ex.curp AS "CURP",

    -- 2. EL EMPLEADO (Laboral)
    em."claveEmpleado" AS "ID_Empleado",
    em.fecha_contratacion AS "Fecha_Ingreso",
    em.estatus_laboral AS "Situacion_Laboral",

    -- 3. EL ROL ACTIVO (Puesto)
    r.nombre_rol AS "Puesto_Vigente",
    r.descripcion AS "Funciones",

    -- 4. EL USUARIO (Acceso al Sistema)
    -- Si no hay registro en la tabla Usuarios, mostrará 'No tiene'
    COALESCE(us.nombre_usuario, 'No tiene') AS "Cuenta_Usuario",
    COALESCE(us.correo_institucional, 'Sin correo') AS "Correo_Institucional",
    CASE 
        WHEN us.estado = TRUE THEN 'Acceso Activo'
        WHEN us.estado = FALSE THEN 'Acceso Suspendido'
        ELSE 'Sin Cuenta'
    END AS "Estatus_Sistema"

FROM "Expedientes" ex
JOIN "Empleados" em ON ex."claveExpediente" = em."claveExpediente"
JOIN "EmpleadoRol" er ON em."claveEmpleado" = er."claveEmpleado"
JOIN "Roles" r ON er."claveRol" = r."claveRol"
-- Usamos LEFT JOIN para no ocultar empleados que no tengan usuario creado
LEFT JOIN "Usuarios" us ON em."claveEmpleado" = us."claveEmpleado"

WHERE 
    em.estatus_laboral = 'Activo'
    AND (er.fecha_fin IS NULL OR er.fecha_fin > CURRENT_DATE)

ORDER BY ex.apellido_paterno ASC;