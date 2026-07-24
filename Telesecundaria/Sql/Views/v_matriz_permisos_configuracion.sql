CREATE OR REPLACE VIEW "v_matriz_permisos_configuracion" AS
SELECT 
    -- 1. ESTRUCTURA DEL MENÚ
    COALESCE(padre.nombre_modulo, '---') AS "Sección Principal",
    m.nombre_modulo AS "Módulo/Botón",
    
    -- 2. IDENTIDAD DEL ROL
    r.nombre_rol AS "Perfil",

    -- 3. MATRIZ DE ACCIONES (Traducción de Booleanos)
    CASE WHEN p.puede_ver THEN 'SÍ' ELSE 'NO' END AS "Visualizar",
    CASE WHEN p.puede_crear THEN 'SÍ' ELSE 'NO' END AS "Crear/Insertar",
    CASE WHEN p.puede_editar THEN 'SÍ' ELSE 'NO' END AS "Modificar",
    CASE WHEN p.puede_eliminar THEN 'SÍ' ELSE 'NO' END AS "Borrar",

    -- 4. DETALLES TÉCNICOS
    m.url_modulo AS "Ruta_URL",
    r.descripcion AS "Descripción del Rol"

FROM "Permisos" p
JOIN "Roles" r ON p."claveRol" = r."claveRol"
JOIN "Modulos" m ON p."claveModulo" = m."claveModulo"
LEFT JOIN "Modulos" padre ON m."claveModuloPadre" = padre."claveModulo"
ORDER BY r.nombre_rol, "Sección Principal", m.nombre_modulo;