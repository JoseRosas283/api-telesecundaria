CREATE OR REPLACE PROCEDURE sp_dar_baja_rol_empleado(
    p_claveEmpleado VARCHAR(18)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_nombre_rol_eliminado VARCHAR(20);
BEGIN
    -- ==========================================================
    -- 1. VALIDACIÓN DE EMPLEADO EXISTENTE
    -- ==========================================================
    IF NOT EXISTS (SELECT 1 FROM "Empleados" WHERE "claveEmpleado" = p_claveEmpleado) THEN
        RAISE EXCEPTION 'Error: El empleado con clave % no existe.', p_claveEmpleado;
    END IF;

    -- ==========================================================
    -- 2. REGLA DE NEGOCIO: VERIFICAR SI TIENE UN ROL ACTIVO PARA CERRAR
    -- ==========================================================
    -- Buscamos el nombre del rol que actualmente está activo (fecha_fin IS NULL)
    SELECT r.nombre_rol 
    INTO v_nombre_rol_eliminado
    FROM "EmpleadoRol" er
    JOIN "Roles" r ON er."claveRol" = r."claveRol"
    WHERE er."claveEmpleado" = p_claveEmpleado 
      AND er.fecha_fin IS NULL;

    -- Si la consulta no guardó nada en la variable, significa que no hay un rol activo
    IF v_nombre_rol_eliminado IS NULL THEN
        RAISE EXCEPTION 'Bloqueo: El empleado % no cuenta con ningún rol activo en este momento. No hay nada que dar de baja.', p_claveEmpleado;
    END IF;

    -- ==========================================================
    -- 3. PROCESAMIENTO FINAL: SELLAR FECHA DE FIN
    -- ==========================================================
    -- Actualizamos el registro activo asignándole la fecha del día de hoy a fecha_fin
    UPDATE "EmpleadoRol" SET
        fecha_fin = CURRENT_DATE
    WHERE "claveEmpleado" = p_claveEmpleado 
      AND fecha_fin IS NULL;

    RAISE NOTICE 'Éxito: Se ha dado de baja el rol "%" para el empleado % correctamente.', 
        v_nombre_rol_eliminado, p_claveEmpleado;

END;
$$;
