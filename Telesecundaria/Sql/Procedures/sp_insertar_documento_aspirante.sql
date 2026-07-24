CREATE OR REPLACE PROCEDURE sp_insertar_documento_aspirante(
    p_ruta_archivo VARCHAR(255),
    p_claveAspirante VARCHAR(18),
    p_nombreTipoDocumento VARCHAR(50), -- Cambiamos clave por nombre
	OUT p_claveDocAspirante VARCHAR(18)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_aspirante_existe BOOLEAN;
    v_claveTipoDoc_encontrada VARCHAR(18);
    v_total_etapas_catalogo INTEGER;
    v_total_documentos_actuales INTEGER;
    v_folio_generico VARCHAR(50);
BEGIN
    -- 1. VALIDACIONES DE NULIDAD
    IF p_ruta_archivo IS NULL OR TRIM(p_ruta_archivo) = '' THEN
        RAISE EXCEPTION 'Error: La ruta del archivo no puede estar vacía.';
    END IF;

    IF p_nombreTipoDocumento IS NULL OR TRIM(p_nombreTipoDocumento) = '' THEN
        RAISE EXCEPTION 'Error: El nombre del tipo de documento es obligatorio.';
    END IF;

    -- 2. VALIDACIÓN DE EXISTENCIA DEL ASPIRANTE
    SELECT EXISTS(SELECT 1 FROM "Aspirantes" a WHERE a."claveAspirante" = p_claveAspirante) 
    INTO v_aspirante_existe;
    
    IF NOT v_aspirante_existe THEN
        RAISE EXCEPTION 'Validación fallida: El Aspirante % no existe.', p_claveAspirante;
    END IF;

    -- 3. TRADUCCIÓN: BUSCAR CLAVE POR NOMBRE (NATIVO)
    -- Buscamos la clave usando el nombre único que definimos en el catálogo
    SELECT td."claveTipoDocumento" INTO v_claveTipoDoc_encontrada
    FROM "TipoDocumentos" td
    WHERE UPPER(TRIM(td.nombre_documento)) = UPPER(TRIM(p_nombreTipoDocumento));

    IF v_claveTipoDoc_encontrada IS NULL THEN
        RAISE EXCEPTION 'Validación fallida: El tipo de documento "%" no existe en el catálogo global.', p_nombreTipoDocumento;
    END IF;

    -- 4. VALIDACIÓN DE RUTA ÚNICA
    IF EXISTS (SELECT 1 FROM "DocumentosAspirante" da WHERE da.ruta_archivo = TRIM(p_ruta_archivo)) THEN
        RAISE EXCEPTION 'Error de Integridad: El archivo en la ruta % ya está registrado.', p_ruta_archivo;
    END IF;

    -- ============================================================
    -- 5. LÓGICA DE TOPES POR CATÁLOGO (PRE/INS) + REQUISITO ACTIVO
    -- ============================================================
    -- CAMBIO REQUERIDO: Se agrega el filtro para validar que el requisito esté activo
    SELECT COUNT(*) INTO v_total_etapas_catalogo
    FROM "Requisitos" r
    WHERE r."claveTipoDocumento" = v_claveTipoDoc_encontrada 
      AND r.etapa_proceso IN ('Preinscripción', 'Inscripción')
      AND r.estado_requisito = TRUE;

    IF v_total_etapas_catalogo = 0 THEN
        RAISE EXCEPTION 'Bloqueo: El documento "%" no es requisito en las etapas permitidas.', p_nombreTipoDocumento;
    END IF;

    -- 6. CONTEO POR ASPIRANTE
    SELECT COUNT(*) INTO v_total_documentos_actuales
    FROM "DocumentosAspirante" da
    WHERE da."claveAspirante" = p_claveAspirante 
      AND da."claveTipoDocumento" = v_claveTipoDoc_encontrada;

    -- REGLA MAESTRA
    IF v_total_documentos_actuales >= v_total_etapas_catalogo THEN
        RAISE EXCEPTION 'Límite alcanzado: El aspirante ya entregó el máximo de % documento(s) para "%".', 
            v_total_etapas_catalogo, p_nombreTipoDocumento;
    END IF;

    -- 7. REGISTRO FINAL
    v_folio_generico := 'GEN-' || p_claveAspirante || '-' || v_claveTipoDoc_encontrada || '-' || (v_total_documentos_actuales + 1);

    INSERT INTO "DocumentosAspirante" (
        folio_documento,
        valor_analitico,
        ruta_archivo,
        "claveAspirante",
        "claveTipoDocumento"
    ) VALUES (
        v_folio_generico,
        'Copia Digital',
        TRIM(p_ruta_archivo),
        p_claveAspirante,
        v_claveTipoDoc_encontrada
    );

	SELECT "claveDocAspirante" INTO p_claveDocAspirante
    FROM "DocumentosAspirante"
    WHERE ruta_archivo = TRIM(p_ruta_archivo)
    LIMIT 1;

    RAISE NOTICE 'Éxito: Documento "%" registrado para el Aspirante % con folio %', 
                  p_nombreTipoDocumento, p_claveAspirante, v_folio_generico;
END;
$$;