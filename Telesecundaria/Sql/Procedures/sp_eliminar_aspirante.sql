CREATE OR REPLACE PROCEDURE sp_eliminar_aspirante(
    p_claveAspirante VARCHAR(18)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_claveTutor_vinculado VARCHAR(18);
    v_token_valido BOOLEAN;
    v_existe_adjuncion BOOLEAN;
BEGIN
    -- ==========================================
    -- 1. LOCALIZACIÓN Y PROPIEDAD
    -- ==========================================
    SELECT "claveTutorAspirante" INTO v_claveTutor_vinculado
    FROM "Aspirantes" WHERE "claveAspirante" = p_claveAspirante;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Error: El aspirante con la clave % no existe.', p_claveAspirante;
    END IF;

    -- ==========================================
    -- 2. VALIDACIÓN DE SESIÓN (SEGURIDAD)
    -- ==========================================
    SELECT EXISTS(
        SELECT 1 FROM "TokenConvocatorias" 
        WHERE "claveTutorAspirante" = v_claveTutor_vinculado 
          AND "estado_sesion" = TRUE 
          AND "fecha_expiracion" > CURRENT_TIMESTAMP
    ) INTO v_token_valido;

    IF NOT v_token_valido THEN
        RAISE EXCEPTION 'Seguridad: Sesión inválida. Solo el tutor responsable puede gestionar esta eliminación.';
    END IF;

    -- ==========================================
    -- 3. EL ÚNICO CANDADO NECESARIO: ADJUNCIONES
    -- ==========================================
    -- Si ya entró al flujo de Adjunciones, el sistema automático
    -- se encargará de su estado. El usuario ya no tiene permiso de borrarlo.
    SELECT EXISTS(SELECT 1 FROM "Adjunciones" WHERE "claveAspirante" = p_claveAspirante) 
    INTO v_existe_adjuncion;

    IF v_existe_adjuncion THEN
        RAISE EXCEPTION 'Acción Prohibida: Este aspirante ya cuenta con documentos en el sistema. El proceso debe seguir su curso automático y no puede ser eliminado ni desactivado manualmente.';
    END IF;

    -- ==========================================
    -- 4. BORRADO FÍSICO (LIMPIEZA DE ERRORES)
    -- ==========================================
    -- Si llegó aquí es porque el registro no tiene rastro en Adjunciones.
    -- Lo borramos físicamente porque es paja o un error de dedo.
    DELETE FROM "Aspirantes" WHERE "claveAspirante" = p_claveAspirante;
    
    RAISE NOTICE 'Éxito: Registro de aspirante (sin historial) eliminado permanentemente.';

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Fallo en sp_eliminar_aspirante: %', SQLERRM;
END;
$$;
