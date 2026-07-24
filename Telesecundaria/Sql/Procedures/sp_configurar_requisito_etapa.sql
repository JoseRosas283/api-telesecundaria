CREATE OR REPLACE PROCEDURE sp_configurar_requisito_etapa(
    p_etapa_proceso VARCHAR(20),
    p_nombre_documento VARCHAR(50)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_claveTipoDocumento VARCHAR(18);
    v_area_nativa VARCHAR(20);
    v_existe_ya INTEGER;
    v_convocatoria_activa BOOLEAN;
    v_citas_programadas INTEGER;
BEGIN
    -- ==========================================================
    -- NUEVA LÓGICA DE JERARQUÍA DE BLOQUEOS SEGÚN ESTADO
    -- ==========================================================
    
    -- Verificamos si existe alguna convocatoria publicada y activa
    SELECT EXISTS (
        SELECT 1 FROM "Convocatorias" 
        WHERE estado = 'Publicada' AND activacion = TRUE
    ) INTO v_convocatoria_activa;

    -- Verificamos si hay citas de inscripción pendientes (correos enviados)
    SELECT COUNT(*) INTO v_citas_programadas
    FROM "CitasInscripcion"
    WHERE "estado_cita" = 'Programada';

    -- REGLA 1: Preinscripción NO se toca si la convocatoria está abierta
    IF p_etapa_proceso = 'Preinscripción' AND v_convocatoria_activa THEN
        RAISE EXCEPTION 'Bloqueo: No se pueden agregar requisitos de Preinscripción mientras la convocatoria esté activa.';
    END IF;

    -- REGLA 2: Inscripción NO se toca si hay citas programadas (ya se notificó al padre)
    IF p_etapa_proceso = 'Inscripción' AND v_citas_programadas > 0 THEN
        RAISE EXCEPTION 'Bloqueo: Hay % citas programadas. El catálogo de Inscripción no se puede modificar para evitar discrepancias con los correos enviados.', v_citas_programadas;
    END IF;

    -- Nota: Becas no entra en estas validaciones, permitiendo cambios libres.

    -- 1. VALIDACIÓN DE ENTRADA (No nulos/vacíos)
    IF p_etapa_proceso IS NULL OR TRIM(p_etapa_proceso) = '' OR 
       p_nombre_documento IS NULL OR TRIM(p_nombre_documento) = '' THEN
        RAISE EXCEPTION 'Error: La etapa y el nombre del documento son obligatorios.';
    END IF;

    -- NUEVA VALIDACIÓN: Respetar el CHECK de la tabla
    IF TRIM(p_etapa_proceso) NOT IN ('Preinscripción', 'Inscripción', 'Becas') THEN
        RAISE EXCEPTION 'Bloqueo: La etapa "%" no es válida. Solo se permite: Preinscripción, Inscripción o Becas.', p_etapa_proceso;
    END IF;

    -- 2. BUSCAR EL DOCUMENTO POR NOMBRE (Normalizado)
    SELECT "claveTipoDocumento", area INTO v_claveTipoDocumento, v_area_nativa
    FROM "TipoDocumentos"
    WHERE UPPER(TRIM(nombre_Documento)) = UPPER(TRIM(p_nombre_documento));

    IF v_claveTipoDocumento IS NULL THEN
        RAISE EXCEPTION 'Error: El documento "%" no existe en el catálogo general.', p_nombre_documento;
    END IF;

    -- --- VALIDACIÓN DE DUPLICADOS EN LA MISMA ETAPA ---
    SELECT COUNT(*) INTO v_existe_ya
    FROM "Requisitos"
    WHERE etapa_proceso = TRIM(p_etapa_proceso) 
      AND "claveTipoDocumento" = v_claveTipoDocumento;

    IF v_existe_ya > 0 THEN
        RAISE EXCEPTION 'Error: El documento "%" ya está registrado como requisito para la etapa %. No se puede repetir en la misma etapa.', 
                        p_nombre_documento, p_etapa_proceso;
    END IF;

    -- 3. EL CANDADO DE JERARQUÍA (Lógica de áreas)
    -- CANDADO 1: Si el documento es nativo de Inscripción, no puede estar en Preinscripción
    IF v_area_nativa = 'Inscripción' AND p_etapa_proceso = 'Preinscripción' THEN
        RAISE EXCEPTION 'Bloqueo: El documento "%" es de área de Inscripción. No puede ser requisito en la etapa de Preinscripción.', p_nombre_documento;
    END IF;

    -- CANDADO 2: Si el documento es nativo de Becas, NO puede estar en Preinscripción ni Inscripción
    IF v_area_nativa = 'Becas' AND p_etapa_proceso IN ('Preinscripción', 'Inscripción') THEN
        RAISE EXCEPTION 'Bloqueo: El documento "%" es exclusivo para el proceso de Becas. No puede pedirse en %.', 
                        p_nombre_documento, p_etapa_proceso;
    END IF;

    -- Documentos Laborales o Institucionales no deben entrar aquí según tu regla
    IF v_area_nativa NOT IN ('Preinscripción', 'Inscripción', 'Becas') THEN
        RAISE EXCEPTION 'Bloqueo: El documento "%" pertenece al área %, no es apto para trámites estudiantiles.', 
                        p_nombre_documento, v_area_nativa;
    END IF;

    -- ==========================================
    -- 4. INSERCIÓN CON VALORES FORZADOS (True y PDF)
    -- ==========================================
    INSERT INTO "Requisitos" (
        etapa_proceso,
        estado_requisito,              
        formato_exigido,
        "claveTipoDocumento"
    ) VALUES (
        TRIM(p_etapa_proceso),
        TRUE, 
        'PDF', 
        v_claveTipoDocumento
    );

    RAISE NOTICE 'Éxito: "%" ahora es requisito obligatorio (PDF) para la etapa %.', 
                 p_nombre_documento, p_etapa_proceso;

EXCEPTION
    WHEN unique_violation THEN
        RAISE EXCEPTION 'Error: El documento "%" ya está registrado como requisito para %.', 
                        p_nombre_documento, p_etapa_proceso;
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Operación fallida: %', SQLERRM;
END;
$$;
