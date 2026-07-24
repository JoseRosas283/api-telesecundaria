CREATE OR REPLACE FUNCTION fn_mantenimiento_sesiones_tutor()
RETURNS void AS $$
BEGIN
    
    UPDATE "TokenConvocatorias"
    SET 
        "estado_sesion" = FALSE,
        "fecha_expiracion" = CURRENT_TIMESTAMP -- Seteamos la expiración al cierre real
    WHERE "estado_sesion" = TRUE 
      AND "fecha_expiracion" <= CURRENT_TIMESTAMP;

    
END;
$$ LANGUAGE plpgsql;