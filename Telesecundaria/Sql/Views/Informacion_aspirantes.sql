CREATE OR REPLACE VIEW "Informacion_aspirantes" AS
SELECT 
    -- 1. DETALLES DEL ASPIRANTE (Al inicio)
    a.nombre || ' ' || a.apellido_paterno || ' ' || COALESCE(a.apellido_materno, '') AS "Nombre del Aspirante",
    a.curp AS "CURP Aspirante",
    a.escuela_procedencia AS "Primaria de Origen",
    a.promedio_primaria AS "Promedio",
    a.estatus_aspirante AS "Estatus",
    CASE WHEN a.estado THEN 'Activo' ELSE 'Inactivo' END AS "Estado Aspirante",
    a."claveConvocatoria" AS "Clave Convocatoria",

    -- 2. DOMICILIO (El puente entre ambos)
    COALESCE(d.calle_numero || ', Col. ' || d.colonia || ', C.P. ' || d.codigo_postal || ', ' || d.municipio, 'DOMICILIO NO REGISTRADO') AS "Domicilio Completo",

    -- 3. DETALLES DEL TUTOR (Al final)
    COALESCE(t.nombre || ' ' || t.apellido_paterno || ' ' || COALESCE(t.apellido_materno, ''), 'SIN TUTOR ASIGNADO') AS "Nombre del Tutor",
    t.telefono AS "Teléfono Tutor",
    t.correo AS "Correo Tutor",
    t.parentesco AS "Parentesco",
    CASE WHEN t.estado THEN 'Activo' ELSE 'Inactivo' END AS "Estado Tutor"

FROM "Aspirantes" a
LEFT JOIN "TutorAspirante" t ON a."claveTutorAspirante" = t."claveTutorAspirante"
LEFT JOIN "Direcciones" d ON t."claveTutorAspirante" = d."claveTutorAspirante";