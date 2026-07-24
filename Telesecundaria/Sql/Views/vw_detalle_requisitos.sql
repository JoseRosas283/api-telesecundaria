CREATE VIEW "vw_detalle_requisitos" AS
SELECT 
    r."claveRequisito",
    td.nombre_documento AS documento,
    td.area AS area_responsable,
    r.etapa_proceso AS etapa_del_tramite,
    r.formato_exigido,
    CASE 
        WHEN r.estado_requisito = TRUE AND td.estado = TRUE THEN 'Activo'
        ELSE 'Inactivo/Bloqueado'
    END AS estatus_global
FROM "Requisitos" r
INNER JOIN "TipoDocumentos" td ON r."claveTipoDocumento" = td."claveTipoDocumento";