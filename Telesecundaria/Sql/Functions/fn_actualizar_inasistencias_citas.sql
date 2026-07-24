CREATE OR REPLACE FUNCTION fn_actualizar_inasistencias_citas()
RETURNS VOID AS $$
BEGIN
    -- Actualizamos las citas que se quedaron en el "limbo"
    UPDATE "CitasInscripcion"
    SET "estado_cita" = 'No Asistió',
        "observaciones" = COALESCE("observaciones", '') || ' [Sistema: Marcado como inasistencia automática por fecha vencida].'
    WHERE "fecha_cita" < CURRENT_DATE 
      AND "estado_cita" = 'Programada';

    RAISE NOTICE 'Proceso completado: Se actualizaron las citas vencidas a "No Asistió".';
END;
$$ LANGUAGE plpgsql;