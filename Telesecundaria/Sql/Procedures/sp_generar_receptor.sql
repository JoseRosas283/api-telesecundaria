CREATE OR REPLACE PROCEDURE sp_generar_receptor(
    p_tipo_receptor VARCHAR(80),
    p_clave_referencia VARCHAR(18),
    p_correo VARCHAR(100)
)
LANGUAGE plpgsql
AS $$
DECLARE
    -- Variables temporales para asegurar la exclusividad (NULOS)
    v_tutor_asp VARCHAR(18) := NULL;
    v_tutor     VARCHAR(18) := NULL;
    v_usuario   VARCHAR(18) := NULL;
    
    -- Variable para verificar existencia
    v_existe_en_entidad BOOLEAN := FALSE;
BEGIN
    -- ============================================================
    -- 1. VALIDACIÓN DE TIPO Y EXISTENCIA EN ENTIDAD MADRE
    -- ============================================================
    -- Aquí validamos que la clave realmente pertenezca a la tabla que dice el tipo
    CASE p_tipo_receptor
        WHEN 'TutorAspirante' THEN
            SELECT EXISTS (SELECT 1 FROM "TutorAspirante" WHERE "claveTutorAspirante" = p_clave_referencia) INTO v_existe_en_entidad;
            IF NOT v_existe_en_entidad THEN
                RAISE EXCEPTION 'Validación fallida: La clave "%" no existe en la tabla de TutoresAspirantes.', p_clave_referencia;
            END IF;
            v_tutor_asp := p_clave_referencia;

        WHEN 'Tutor' THEN
            SELECT EXISTS (SELECT 1 FROM "Tutores" WHERE "claveTutor" = p_clave_referencia) INTO v_existe_en_entidad;
            IF NOT v_existe_en_entidad THEN
                RAISE EXCEPTION 'Validación fallida: La clave "%" no existe en la tabla de Tutores.', p_clave_referencia;
            END IF;
            v_tutor := p_clave_referencia;

        WHEN 'Usuario' THEN
            SELECT EXISTS (SELECT 1 FROM "Usuarios" WHERE "claveUsuario" = p_clave_referencia) INTO v_existe_en_entidad;
            IF NOT v_existe_en_entidad THEN
                RAISE EXCEPTION 'Validación fallida: La clave "%" no existe en la tabla de Usuarios.', p_clave_referencia;
            END IF;
            v_usuario := p_clave_referencia;

        ELSE
            RAISE EXCEPTION 'Error: El tipo de receptor "%" no es reconocido por el sistema.', p_tipo_receptor;
    END CASE;

    -- ============================================================
    -- 2. INSERCIÓN ATÓMICA
    -- ============================================================
    -- Si llegamos aquí, es porque la clave existe y el tipo es correcto
    INSERT INTO "Receptores" (
        tipo_receptor,
        "claveTutorAspirante",
        "claveTutor",
        "claveUsuario",
        correo_destino,
        estado
    ) 
    VALUES (
        p_tipo_receptor,
        v_tutor_asp,
        v_tutor,
        v_usuario,
        p_correo,
        TRUE      
    );

    RAISE NOTICE 'Receptor creado exitosamente: Tipo %, Referencia % confirmada.', 
                  p_tipo_receptor, p_clave_referencia;

EXCEPTION
    WHEN OTHERS THEN
        -- Capturamos cualquier error (incluyendo los RAISE EXCEPTION de arriba)
        RAISE EXCEPTION 'Error en sp_generar_receptor: %', SQLERRM;
END;
$$;
