CREATE OR REPLACE FUNCTION fn_tr_notificar_cita_detallada()
RETURNS TRIGGER AS $$
DECLARE
    v_datos_aspirante RECORD;
    v_lista_requisitos TEXT;
    v_mensaje_cuerpo TEXT;
    v_titulo_notif VARCHAR(80);
BEGIN
    -- 1. VIAJE POR LAS TABLAS: Incluimos el JOIN con Receptores para obtener la clave RECP-
    SELECT 
        asp.nombre || ' ' || asp.apellido_paterno AS nombre_alumno,
        tut.nombre || ' ' || tut.apellido_paterno AS nombre_tutor,
        rev."claveAdjuncion",
        rec."claveReceptor" -- <--- Obtenemos el ID real del receptor (ej. RECP-000000000005)
    INTO 
        v_datos_aspirante
    FROM "Revisiones" rev
    INNER JOIN "Adjunciones" adj ON rev."claveAdjuncion" = adj."claveAdjuncion"
    INNER JOIN "Aspirantes" asp ON adj."claveAspirante" = asp."claveAspirante"
    INNER JOIN "TutorAspirante" tut ON asp."claveTutorAspirante" = tut."claveTutorAspirante"
    INNER JOIN "Receptores" rec ON tut."claveTutorAspirante" = rec."claveTutorAspirante" -- Puente con Receptores
    WHERE rev."claveRevision" = NEW."claveRevision";

    -- 2. CAMBIO AÑADIDO: Obtener lista de requisitos de la etapa 'Inscripción'
    SELECT string_agg('• ' || td.nombre_documento || ' (' || r.formato_exigido || ')', chr(10)) 
    INTO v_lista_requisitos
    FROM "Requisitos" r
    INNER JOIN "TipoDocumentos" td ON r."claveTipoDocumento" = td."claveTipoDocumento"
    WHERE r.etapa_proceso = 'Inscripción' 
      AND r.estado_requisito = TRUE;

    -- 3. PERSONALIZACIÓN DEL MENSAJE (Incluyendo los requisitos dinámicos)
    v_titulo_notif := 'Cita de Inscripción Confirmada - ' || v_datos_aspirante.nombre_alumno;
    
    v_mensaje_cuerpo := 'Estimado(a) ' || v_datos_aspirante.nombre_tutor || ': ' || chr(10) ||
                        'Se le informa que la cita de inscripción para el alumno ' || 
                        v_datos_aspirante.nombre_alumno || ' (Folio: ' || v_datos_aspirante."claveAdjuncion" || ') ' ||
                        'ha sido programada exitosamente.' || chr(10) || chr(10) ||
                        'FECHA: ' || TO_CHAR(NEW.fecha_cita, 'DD/MM/YYYY') || chr(10) ||
                        'HORA: ' || TO_CHAR(NEW.hora_cita, 'HH24:MI') || ' hrs.' || chr(10) || chr(10) ||
                        'REQUISITOS OBLIGATORIOS (Original y Escaneados en USB):' || chr(10) ||
                        COALESCE(v_lista_requisitos, 'Consulte la lista en el portal oficial.') || chr(10) || chr(10) ||
                        'Por favor, acuda puntualmente con la documentación mencionada.';

    -- 4. LLAMADA A TU PROCEDIMIENTO sp_emitir_notificacion
    -- Cambiamos NEW.claveTutorAspirante por la clave del Receptor real
    CALL sp_emitir_notificacion(
        v_datos_aspirante."claveReceptor", -- Enviamos el RECP-... que sí existe en Receptores
        v_titulo_notif, 
        v_mensaje_cuerpo, 
        3::SMALLINT, 
        'Citas', 
        jsonb_build_object(
            'alumno', v_datos_aspirante.nombre_alumno,
            'folio', v_datos_aspirante."claveAdjuncion",
            'fecha', NEW.fecha_cita
        )
    );

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;
-- 4. DISPARADOR (Trigger)
CREATE TRIGGER tr_notificar_cita_completa
AFTER INSERT ON "CitasInscripcion"
FOR EACH ROW
EXECUTE FUNCTION fn_tr_notificar_cita_detallada();