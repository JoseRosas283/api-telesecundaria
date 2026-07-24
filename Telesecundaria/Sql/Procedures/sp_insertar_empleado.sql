CREATE OR REPLACE PROCEDURE sp_insertar_empleado(
    p_claveExpediente VARCHAR(18),
    p_tipo_contrato VARCHAR(10),
    p_telefono VARCHAR(15),
    p_fecha_contratacion DATE DEFAULT CURRENT_DATE
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_empleado_duplicado BOOLEAN;
    v_tipo_contrato_limpio VARCHAR(10);
    v_tipo_titular VARCHAR(20);
BEGIN
    -- ==========================================
    -- 1. LIMPIEZA Y PREPARACIÓN
    -- ==========================================
    v_tipo_contrato_limpio := INITCAP(TRIM(p_tipo_contrato));

    -- ==========================================
    -- 2. VALIDACIONES DE EXISTENCIA Y TIPO
    -- ==========================================
    
    -- Obtener el tipo de titular del expediente
    SELECT tipo_titular FROM "Expedientes" 
    WHERE "claveExpediente" = p_claveExpediente 
    INTO v_tipo_titular;

    -- Si no se encuentra el expediente
    IF v_tipo_titular IS NULL THEN
        RAISE EXCEPTION 'Error: El expediente % no existe.', p_claveExpediente;
    END IF;

    -- Validar que el expediente sea de tipo Empleado y no Alumno
    IF v_tipo_titular <> 'Empleado' THEN
        RAISE EXCEPTION 'Bloqueo: El expediente % es de un %, no puede ser empleado.', p_claveExpediente, v_tipo_titular;
    END IF;

    -- Verificar que no haya un contrato ACTIVO ya vinculado
    SELECT EXISTS (SELECT 1 FROM Empleados WHERE claveExpediente = p_claveExpediente AND estatus_laboral = 'Activo')
    INTO v_empleado_duplicado;

    IF v_empleado_duplicado THEN
        RAISE EXCEPTION 'Bloqueo: El expediente % ya tiene un contrato activo vigente.', p_claveExpediente;
    END IF;

    -- ==========================================
    -- 3. VALIDACIÓN DE DOMINIO (CHECK)
    -- ==========================================
    IF v_tipo_contrato_limpio NOT IN ('Planta', 'Temporal') THEN
        RAISE EXCEPTION 'Error: Contrato "%" inválido. Use Planta o Temporal.', p_tipo_contrato;
    END IF;

    -- ==========================================
    -- 4. INSERCIÓN FINAL
    -- ==========================================
    INSERT INTO "Empleados" (
        fecha_contratacion,
        tipo_contrato,
        estatus_laboral,
        telefono,
        "claveExpediente"
    ) VALUES (
        p_fecha_contratacion,
        v_tipo_contrato_limpio,
        'Activo',
        TRIM(p_telefono),
        p_claveExpediente
    );

    RAISE NOTICE 'Éxito: Empleado registrado y vinculado al expediente %.', p_claveExpediente;

END;
$$;