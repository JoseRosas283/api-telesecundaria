CREATE OR REPLACE PROCEDURE sp_eliminar_tutor_aspirante(
    p_claveTutor VARCHAR(18)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_existe_tutor BOOLEAN;
    v_tiene_adjunciones BOOLEAN;
    v_tiene_aspirantes BOOLEAN;
    v_clave_receptor VARCHAR(18);
BEGIN
    -- ==========================================
    -- 1. VALIDACIÓN DE EXISTENCIA
    -- ==========================================
    SELECT EXISTS(SELECT 1 FROM "TutorAspirante" WHERE "claveTutorAspirante" = p_claveTutor) INTO v_existe_tutor;

    IF NOT v_existe_tutor THEN
        RAISE EXCEPTION 'Error: El tutor con clave % no existe.', p_claveTutor;
    END IF;

    -- ==========================================
    -- 2. CANDADO INMUTABLE: ADJUNCIONES
    -- ==========================================
    -- Si ya hay documentos, el proceso está "blindado". No se toca nada.
    SELECT EXISTS (
        SELECT 1 FROM "Adjunciones" ADJ
        INNER JOIN "Aspirantes" ASP ON ADJ."claveAspirante" = ASP."claveAspirante"
        WHERE ASP."claveTutorAspirante" = p_claveTutor
    ) INTO v_tiene_adjunciones;

    IF v_tiene_adjunciones THEN
        RAISE EXCEPTION 'Acción Prohibida: El tutor tiene expedientes en Adjunciones. El registro es inmutable para fines de auditoría.';
    END IF;

    -- ==========================================
    -- 3. VALIDACIÓN DE VÍNCULOS
    -- ==========================================
    SELECT EXISTS (
        SELECT 1 FROM "Aspirantes" WHERE "claveTutorAspirante" = p_claveTutor
    ) INTO v_tiene_aspirantes;

    SELECT "claveReceptor" INTO v_clave_receptor FROM "Receptores" 
    WHERE "claveTutorAspirante" = p_claveTutor AND "tipo_receptor" = 'TutorAspirante';

    -- ==========================================
    -- 4. LOGICA DE ELIMINACIÓN / DESACTIVACIÓN
    -- ==========================================

    IF v_tiene_aspirantes THEN
        -- ------------------------------------------
        -- ESCENARIO A: DESACTIVACIÓN EN CASCADA
        -- ------------------------------------------
        
        -- A.1) Desactivar Tutor y sus Aspirantes
        UPDATE "TutorAspirante" SET estado = FALSE WHERE "claveTutorAspirante" = p_claveTutor;
        UPDATE "Aspirantes" SET estado = FALSE WHERE "claveTutorAspirante" = p_claveTutor;
        
        -- A.2) Desactivar Receptor (Mantenemos el vínculo de comunicación histórico)
        IF v_clave_receptor IS NOT NULL THEN
            UPDATE "Receptores" SET estado = FALSE WHERE "claveReceptor" = v_clave_receptor;
        END IF;

        -- A.3) Desactivar Verificación de Dirección
        -- Marcamos la dirección como no vigente o desactivada
        UPDATE "Direcciones" 
        SET "estado_verificacion" = FALSE -- O el nombre exacto de tu columna de estado
        WHERE "claveTutorAspirante" = p_claveTutor;

        RAISE NOTICE 'Cascada Completa: Tutor, Aspirantes, Receptor y Dirección han sido desactivados.';

    ELSE
        -- ------------------------------------------
        -- ESCENARIO B: BORRADO FÍSICO TOTAL
        -- ------------------------------------------
        
        -- B.1) Intentar eliminar el receptor (el SP decide si borra o desactiva por notificaciones)
        IF v_clave_receptor IS NOT NULL THEN
            CALL sp_eliminar_receptor(v_clave_receptor);
        END IF;

        -- B.2) Borrado físico de la dirección y del tutor
        DELETE FROM "Direcciones" WHERE "claveTutorAspirante" = p_claveTutor;
        DELETE FROM "TutorAspirante" WHERE "claveTutorAspirante" = p_claveTutor;

        RAISE NOTICE 'Limpieza Exitosa: Registro sin historial eliminado físicamente del sistema.';
    END IF;

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error crítico en sp_eliminar_tutor_aspirante: %', SQLERRM;
END;
$$;
