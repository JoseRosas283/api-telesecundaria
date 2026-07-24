CREATE OR REPLACE FUNCTION fn_notificar_cierre_institucional()
RETURNS TRIGGER AS $$
DECLARE
    v_destinatario  RECORD;
    -- Usamos el NOMBRE SELLADO del catálogo para mayor seguridad y dinamismo
    v_nombre_proceso VARCHAR(50) := 'Cierre de Adjuncion'; 
BEGIN
    -- EL GUARDIÁN: Solo actúa cuando el estatus operativo cambia de Abierta a Cerrada
    IF (OLD.estatus_operativo = 'Abierta' AND NEW.estatus_operativo = 'Cerrada') THEN
        
        -- VIAJE POR LAS ENTIDADES:
        -- 1. Receptor -> 2. Usuario -> 3. Empleado -> 4. EmpleadoRol -> 5. Rol
        FOR v_destinatario IN (
            SELECT DISTINCT r."claveReceptor", ro.nombre_rol -- <--- Único cambio: añadimos ro.nombre_rol
            FROM "Receptores" r
            INNER JOIN "Usuarios" u ON r."claveUsuario" = u."claveUsuario"
            INNER JOIN "Empleados" e ON u."claveEmpleado" = e."claveEmpleado"
            INNER JOIN "EmpleadoRol" er ON e."claveEmpleado" = er."claveEmpleado"
            INNER JOIN "Roles" ro ON er."claveRol" = ro."claveRol"
            WHERE r.tipo_receptor = 'Usuario' 
              AND r.estado = TRUE               -- El receptor debe estar habilitado
              AND e.estatus_laboral = 'Activo'  -- El empleado debe estar laborando
              
              -- FILTRO DE VIGENCIA: ¿El rol es válido hoy?
              AND er.fecha_inicio <= CURRENT_DATE 
              AND (er.fecha_fin IS NULL OR er.fecha_fin >= CURRENT_DATE)
              
              -- FILTRO DE ROLES PERMITIDOS: Solo Directivos y Administrativos
              AND ro.nombre_rol IN ('Directivo', 'Administrativo')
        ) LOOP

            -- DISPARAR AL EMISOR CENTRAL
            -- Se ejecutará una vez por cada receptor individual que cumpla los criterios
            CALL sp_emitir_notificacion(
                v_destinatario."claveReceptor",
                'Nuevo Expediente para Revisión',
                'El aspirante con folio ' || NEW."claveAspirante" || ' ha cerrado su adjunción.',
                2::SMALLINT, -- Prioridad Media
                v_nombre_proceso, -- Mandamos el nombre para que el SP busque el ID
                jsonb_build_object(
                    'folio_adjuncion', NEW."claveAdjuncion",
                    'fecha_evento', CURRENT_TIMESTAMP
                )
            );

            -- EL CAMBIO CLAVE: Mensaje dinámico según el rol real encontrado
            RAISE NOTICE 'Notificación registrada para el sistema (Usuario %).', v_destinatario.nombre_rol;

        END LOOP;
        
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS tg_notificar_cierre_adjuncion ON "Adjunciones";

CREATE TRIGGER tg_notificar_cierre_adjuncion
AFTER UPDATE OF estatus_operativo ON "Adjunciones"
FOR EACH ROW
EXECUTE FUNCTION fn_notificar_cierre_institucional();