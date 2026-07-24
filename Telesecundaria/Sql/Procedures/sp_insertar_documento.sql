CREATE OR REPLACE PROCEDURE sp_insertar_documento(
    p_archivo_url VARCHAR(80),
    p_claveExpediente VARCHAR(18),
    p_nombreTipoDocumento VARCHAR(50)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_tipo_titular VARCHAR(20);
    v_area_documento VARCHAR(20);
    v_claveTipoDocumento VARCHAR(18);
    v_claveEmpleado VARCHAR(18);
    v_tiene_perfil BOOLEAN;
    v_tiene_rol_activo BOOLEAN;
    v_es_intendente BOOLEAN; -- Variable añadida para validar el rol específico
BEGIN
    -- ==========================================
    -- 1. VALIDACIÓN DE URL (Global)
    -- ==========================================
    IF p_archivo_url IS NULL OR TRIM(p_archivo_url) = '' THEN
        RAISE EXCEPTION 'Error: La ruta del archivo (URL) no puede estar vacía.';
    END IF;

    IF EXISTS (SELECT 1 FROM "Documentos" WHERE "archivo_url" = TRIM(p_archivo_url)) THEN
        RAISE EXCEPTION 'Bloqueo: El archivo con la ruta % ya ha sido registrado previamente.', p_archivo_url;
    END IF;

    -- ==========================================
    -- 2. IDENTIFICACIÓN Y CATÁLOGOS (Normalización de nombres)
    -- ==========================================
    SELECT "tipo_titular" INTO v_tipo_titular FROM "Expedientes" WHERE "claveExpediente" = p_claveExpediente;
    IF v_tipo_titular IS NULL THEN RAISE EXCEPTION 'Error: El expediente % no existe.', p_claveExpediente; END IF;

    SELECT "claveTipoDocumento", "area" INTO v_claveTipoDocumento, v_area_documento 
    FROM "TipoDocumentos" 
    WHERE UPPER(TRIM("nombre_documento")) = UPPER(TRIM(p_nombreTipoDocumento));

    IF v_claveTipoDocumento IS NULL THEN 
        RAISE EXCEPTION 'Error: El tipo de documento "%" no existe en el catálogo.', p_nombreTipoDocumento; 
    END IF;

    -- ==========================================
    -- 3. VALIDACIÓN DE UNICIDAD POR TIPO (EL CANDADO SOLICITADO)
    -- ==========================================
    -- No permitimos que un expediente tenga dos documentos del mismo tipo_id
    IF EXISTS (
        SELECT 1 FROM "Documentos" 
        WHERE "claveExpediente" = p_claveExpediente 
        AND "claveTipoDocumento" = v_claveTipoDocumento
    ) THEN
        RAISE EXCEPTION 'Bloqueo: El expediente % ya cuenta con un documento de tipo "%". Solo se permite un archivo por cada tipo de documento.', 
                        p_claveExpediente, p_nombreTipoDocumento;
    END IF;

    -- ==========================================
    -- 4. VALIDACIÓN DE VÍNCULOS (Perfil y Rol)
    -- ==========================================
    IF v_tipo_titular = 'Empleado' THEN
        SELECT "claveEmpleado" INTO v_claveEmpleado 
        FROM "Empleados" 
        WHERE "claveExpediente" = p_claveExpediente;

        IF v_claveEmpleado IS NULL THEN
            RAISE EXCEPTION 'Bloqueo: El expediente % es de tipo Empleado pero no está en la tabla Empleados.', p_claveExpediente;
        END IF;

        SELECT EXISTS (
            SELECT 1 FROM "EmpleadoRol" 
            WHERE "claveEmpleado" = v_claveEmpleado 
            AND "fecha_fin" IS NULL
        ) INTO v_tiene_rol_activo;

        IF NOT v_tiene_rol_activo THEN
            RAISE EXCEPTION 'Bloqueo: El empleado no tiene un ROL ACTIVO asignado. No es posible encapsular documentos sin un puesto vigente.';
        END IF;

        -- ------------------------------------------------------------------
        -- SUB-CANDADO ADICIONAL: Protección para Perfil de Intendente
        -- ------------------------------------------------------------------
        SELECT EXISTS (
            SELECT 1 
            FROM "EmpleadoRol" er
            INNER JOIN "Roles" r ON er."claveRol" = r."claveRol"
            WHERE er."claveEmpleado" = v_claveEmpleado
              AND r."nombre_rol" = 'Intendente'
              AND er."fecha_fin" IS NULL
        ) INTO v_es_intendente;

        IF v_es_intendente THEN
            IF UPPER(TRIM(p_nombreTipoDocumento)) IN ('TÍTULO PROFESIONAL', 'CÉDULA PROFESIONAL') THEN
                RAISE EXCEPTION 'Bloqueo: El perfil de Intendente no requiere ni permite la carga de "%".', p_nombreTipoDocumento;
            END IF;
        END IF;
        -- ------------------------------------------------------------------

    ELSIF v_tipo_titular = 'Alumno' THEN
        SELECT EXISTS (SELECT 1 FROM "Alumnos" WHERE "claveExpediente" = p_claveExpediente) INTO v_tiene_perfil;
        IF NOT v_tiene_perfil THEN
            RAISE EXCEPTION 'Bloqueo: El expediente % no existe en la tabla Alumnos.', p_claveExpediente;
        END IF;
    END IF;

    -- ==========================================
    -- 5. VALIDACIÓN DE CRUCE (Áreas)
    -- ==========================================
    IF v_tipo_titular = 'Empleado' AND v_area_documento <> 'Laboral' THEN
        RAISE EXCEPTION 'Inconsistencia: Empleados solo reciben área Laboral.';
    END IF;

    IF v_tipo_titular = 'Alumno' THEN
        IF v_area_documento = 'Laboral' THEN
            RAISE EXCEPTION 'Inconsistencia: Alumnos no reciben área Laboral.';
        END IF;

        IF v_area_documento NOT IN ('Preinscripción','Inscripción', 'Becas', 'Egreso', 'Institucional') THEN
            RAISE EXCEPTION 'Bloqueo: El área % no es válida para Alumnos.', v_area_documento;
        END IF;
    END IF;

    -- ==========================================
    -- 6. INSERCIÓN FINAL (Estado Original)
    -- ==========================================
    INSERT INTO "Documentos" (
        "archivo_url", 
        "estado", 
        "claveExpediente", 
        "claveTipoDocumento"
    ) VALUES (
        TRIM(p_archivo_url), 
        'Original', 
        p_claveExpediente, 
        v_claveTipoDocumento
    );

    RAISE NOTICE 'Éxito: Documento "%" indexado correctamente en el expediente %.', p_nombreTipoDocumento, p_claveExpediente;

END;
$$;