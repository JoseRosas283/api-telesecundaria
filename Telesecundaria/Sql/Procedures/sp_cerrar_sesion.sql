CREATE OR REPLACE PROCEDURE sp_cerrar_sesion(
    p_clave_logueo VARCHAR(18),
    OUT p_exito BOOLEAN,
    OUT p_mensaje VARCHAR
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_fecha_cierre_actual TIMESTAMP;
BEGIN
    -- 1. Buscamos si ya existe una fecha de cierre para esta clave
    SELECT fecha_cierre INTO v_fecha_cierre_actual
    FROM "Logueos"
    WHERE "claveLogueo" = p_clave_logueo;

    -- 2. VALIDACIÓN: ¿Existe el registro?
    IF NOT FOUND THEN
        p_exito := FALSE;
        p_mensaje := 'Error: La clave de logueo no existe.';
        RETURN;
    END IF;

    -- 3. VALIDACIÓN CRÍTICA: ¿Ya está cerrada? (Si fecha_cierre NO es NULL)
    IF v_fecha_cierre_actual IS NOT NULL THEN
        p_exito := FALSE;
        p_mensaje := 'Aviso: Esta sesión ya fue cerrada previamente el ' || 
                     to_char(v_fecha_cierre_actual, 'DD/MM/YYYY HH24:MI:SS');
        RETURN;
    END IF;

    -- 4. Ejecutamos el cierre si la fecha era NULL
    UPDATE "Logueos"
    SET 
        fecha_cierre = CURRENT_TIMESTAMP,
        estatus_intento = 'Sesión Finalizada'
    WHERE "claveLogueo" = p_clave_logueo;

    p_exito := TRUE;
    p_mensaje := 'Sesión cerrada correctamente.';

EXCEPTION
    WHEN OTHERS THEN
        p_exito := FALSE;
        p_mensaje := 'Error interno: ' || SQLERRM;
END;
$$;
