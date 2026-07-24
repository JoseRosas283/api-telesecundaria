CREATE OR REPLACE PROCEDURE sp_actualizar_ruta_documento_rechazado(
    p_claveDocAspirante VARCHAR(18),
    p_ruta_archivo_nueva VARCHAR(255)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_ruta_actual VARCHAR(255);
    v_documento_existe BOOLEAN;
    v_estatus_detalle VARCHAR(20);
BEGIN
    -- 1. VALIDACIÓN DE NULIDAD
    IF p_ruta_archivo_nueva IS NULL OR TRIM(p_ruta_archivo_nueva) = '' THEN
        RAISE EXCEPTION 'Error de Validación: La nueva ruta del archivo no puede estar vacía.';
    END IF;

    -- 2. VERIFICAR EXISTENCIA DEL DOCUMENTO (PRIMER FILTRO OBLIGATORIO)
    SELECT EXISTS(SELECT 1 FROM "DocumentosAspirante" WHERE "claveDocAspirante" = p_claveDocAspirante)
    INTO v_documento_existe;

    IF NOT v_documento_existe THEN
        RAISE EXCEPTION 'Error Crítico: El documento con clave % no existe. No se puede actualizar.', p_claveDocAspirante;
    END IF;

    -- 3. VALIDACIÓN DE ESTADO EN DETALLE ADJUNCION (REGLA DE FLUJO)
    -- Buscamos el estatus más reciente que tiene asignado este documento
    SELECT da."estatus_documento" INTO v_estatus_detalle
    FROM "DetalleAdjuncion" da
    WHERE da."claveDocAspirante" = p_claveDocAspirante
    ORDER BY da."fecha_evaluacion" DESC, da."claveAdjuncion" DESC
    LIMIT 1;

    -- Si se encuentra en Pendiente o Aceptado, se bloquea el Update inmediatamente
    IF v_estatus_detalle IN ('Pendiente', 'Aceptado') THEN
        RAISE EXCEPTION 'Bloqueo de Flujo: El documento se encuentra en estatus "%". Solo se permite cambiar la ruta si el documento está Rechazado.', v_estatus_detalle;
    END IF;

    -- 4. VALIDACIÓN DE RUTA ÚNICA GENERAL (Para evitar duplicados con otros registros)
    IF EXISTS (
        SELECT 1 
        FROM "DocumentosAspirante" 
        WHERE "ruta_archivo" = TRIM(p_ruta_archivo_nueva) 
          AND "claveDocAspirante" <> p_claveDocAspirante
    ) THEN
        RAISE EXCEPTION 'Error de Integridad: El archivo en la ruta % ya está registrado en otro documento.', p_ruta_archivo_nueva;
    END IF;

    -- 5. OBTENER LA RUTA ACTUAL PARA VALIDAR CAMBIO REAL
    SELECT "ruta_archivo" INTO v_ruta_actual 
    FROM "DocumentosAspirante" 
    WHERE "claveDocAspirante" = p_claveDocAspirante;

    IF v_ruta_actual = TRIM(p_ruta_archivo_nueva) THEN
        RAISE EXCEPTION 'Bloqueo: La nueva ruta es idéntica a la actual. Debes proporcionar un archivo diferente para corregir el rechazo.';
    END IF;

    -- 6. ACTUALIZACIÓN OPERATIVA
    UPDATE "DocumentosAspirante"
    SET "ruta_archivo" = TRIM(p_ruta_archivo_nueva)
    WHERE "claveDocAspirante" = p_claveDocAspirante;

    RAISE NOTICE 'Éxito: Ruta del documento % actualizada correctamente a: %', 
                 p_claveDocAspirante, TRIM(p_ruta_archivo_nueva);
END;
$$;