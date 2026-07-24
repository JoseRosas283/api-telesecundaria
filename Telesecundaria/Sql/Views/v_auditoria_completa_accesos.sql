CREATE OR REPLACE VIEW "v_auditoria_completa_accesos" AS
SELECT 
    -- 1. TODO LO DE LA SECCIÓN DE LOGUEO (Datos completos)
    l."claveLogueo" AS "ID_Sesion",
    l.fecha_acceso AS "Inicio_Sesion",
    l.fecha_cierre AS "Fin_Sesion",
    l.estatus_intento AS "Estatus",
    l.direccion_ip AS "IP_Origen",
    l.agente_usuario AS "Navegador_Dispositivo",
    
    -- Calculamos si la sesión sigue abierta o cuánto duró
    CASE 
        WHEN l.fecha_cierre IS NULL AND l.estatus_intento = 'Exitoso' THEN 'Activa'
        WHEN l.fecha_cierre IS NOT NULL THEN 'Finalizada'
        ELSE 'Fallida'
    END AS "Estado_Sesion",

    -- 2. IDENTIDAD DIGITAL (Usuario)
    u."claveUsuario" AS "ID_Usuario",
    u.nombre_usuario AS "Cuenta",

    -- 3. EL ROL (Viaje a través de Empleado)
    r.nombre_rol AS "Rol_en_Turno",
    r.descripcion AS "Perfil_Acceso"

FROM "Logueos" l
-- Joins para llegar al Rol
JOIN "Usuarios" u ON l."claveUsuario" = u."claveUsuario"
JOIN "Empleados" e ON u."claveEmpleado" = e."claveEmpleado"
JOIN "EmpleadoRol" er ON e."claveEmpleado" = er."claveEmpleado"
JOIN "Roles" r ON er."claveRol" = r."claveRol"

WHERE 
    -- Mantenemos la integridad del rol respecto a la fecha en que ocurrió el logueo
    (l.fecha_acceso >= er.fecha_inicio)
    AND (er.fecha_fin IS NULL OR l.fecha_acceso <= er.fecha_fin)

ORDER BY l.fecha_acceso DESC;