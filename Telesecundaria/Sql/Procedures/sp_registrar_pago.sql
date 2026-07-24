CREATE OR REPLACE PROCEDURE sp_registrar_pago(
    p_claveTutor        VARCHAR(18),
    p_nombre_usuario    VARCHAR(100),
    p_monto             DECIMAL(10,2),
    p_metodo_pago       VARCHAR(30),
    p_comprobante_pago  VARCHAR(100)
)
LANGUAGE plpgsql
AS $$
DECLARE
    -- Variables de Control de Negocio
    v_claveCiclo_act    VARCHAR(18);
    v_nombre_ciclo      VARCHAR(100);
    v_clavePago_nueva   VARCHAR(18);
    v_referencia_auto   TEXT;
    
    -- Variables de Seguridad y Auditoría
    v_claveUsuario_op   VARCHAR(18);
    v_usuario_activo    BOOLEAN;
    v_sesion_valida     BOOLEAN;
    v_rol_autorizado    BOOLEAN;
    v_permiso_modulo    BOOLEAN;
BEGIN
    -- ============================================================
    -- 1. IDENTIFICACIÓN DEL CONTEXTO (CICLO ESCOLAR)
    -- ============================================================
    SELECT "claveCiclo", "nombre_ciclo" 
    INTO v_claveCiclo_act, v_nombre_ciclo 
    FROM "Ciclos" 
    WHERE "estado_ciclo" = TRUE 
    LIMIT 1;

    IF v_claveCiclo_act IS NULL THEN
        RAISE EXCEPTION 'BLOQUEO: No existe un ciclo escolar activo para procesar cobros.';
    END IF;

    -- ============================================================
    -- 2. VALIDACIONES DE MATERIA PRIMA (TUTOR Y ALUMNOS)
    -- ============================================================
    
    -- 2.1 Verificación de existencia del Tutor
    IF NOT EXISTS (SELECT 1 FROM "Tutores" WHERE "claveTutor" = p_claveTutor) THEN
        RAISE EXCEPTION 'ERROR: El tutor con clave "%" no existe.', p_claveTutor;
    END IF;

    -- 2.2 Verificación de hijos/alumnos vinculados y activos
    IF NOT EXISTS (
        SELECT 1 FROM "TutoresAlumnos" 
        WHERE "claveTutor" = p_claveTutor AND fecha_baja IS NULL
    ) THEN
        RAISE EXCEPTION 'BLOQUEO: El tutor no tiene alumnos activos asignados bajo su cargo.';
    END IF;

    -- ============================================================
    -- 3. VALIDACIÓN DE ELEGIBILIDAD PARA PAGO (INSCRIPCIONES)
    -- ============================================================
    
    -- 3.1 Verificar si los alumnos están registrados en el ciclo activo
    IF NOT EXISTS (
        SELECT 1 FROM "Inscripciones" i
        JOIN "TutoresAlumnos" ta ON i."claveAlumno" = ta."claveAlumno"
        WHERE ta."claveTutor" = p_claveTutor 
          AND i."claveCiclo" = v_claveCiclo_act
          AND ta.fecha_baja IS NULL
    ) THEN
        RAISE EXCEPTION 'BLOQUEO: No se encontraron inscripciones para los alumnos de este tutor en el ciclo %.', v_nombre_ciclo;
    END IF;

    -- 3.2 Verificar si existe al menos una inscripción pendiente de pago (clavePago IS NULL)
    IF NOT EXISTS (
        SELECT 1 FROM "Inscripciones" i
        JOIN "TutoresAlumnos" ta ON i."claveAlumno" = ta."claveAlumno"
        WHERE ta."claveTutor" = p_claveTutor 
          AND i."claveCiclo" = v_claveCiclo_act 
          AND i."clavePago" IS NULL 
          AND ta.fecha_baja IS NULL
    ) THEN
        RAISE EXCEPTION 'BLOQUEO: Las inscripciones para este ciclo ya cuentan con un pago registrado.';
    END IF;

    -- ============================================================
    -- 4. VALIDACIÓN DE DATOS DE TRANSACCIÓN
    -- ============================================================
    
    -- 4.1 Validación de monto positivo
    IF p_monto <= 0 THEN
        RAISE EXCEPTION 'ERROR: El monto del pago debe ser mayor a cero.';
    END IF;

    -- 4.2 Validación de comprobante obligatorio para métodos bancarios
    IF p_metodo_pago != 'Efectivo' AND (p_comprobante_pago IS NULL OR TRIM(p_comprobante_pago) = '') THEN
        RAISE EXCEPTION 'ERROR: El método % requiere obligatoriamente un número de comprobante/folio.', p_metodo_pago;
    END IF;

    -- ============================================================
    -- 5. SEGURIDAD Y PERMISOS DEL OPERADOR
    -- ============================================================
    
    -- 5.1 Identidad y estado del usuario
    SELECT "claveUsuario", estado INTO v_claveUsuario_op, v_usuario_activo 
    FROM "Usuarios" WHERE "nombre_usuario" = TRIM(p_nombre_usuario);

    IF v_claveUsuario_op IS NULL OR NOT v_usuario_activo THEN
        RAISE EXCEPTION 'ACCESO DENEGADO: Usuario inexistente o inactivo.';
    END IF;

    -- 5.2 Verificación de sesión activa (Logueos)
    SELECT EXISTS (
        SELECT 1 FROM "Logueos" 
        WHERE "claveUsuario" = v_claveUsuario_op AND estatus_intento = 'Exitoso' AND fecha_cierre IS NULL
    ) INTO v_sesion_valida;

    IF NOT v_sesion_valida THEN 
        RAISE EXCEPTION 'ACCESO DENEGADO: El usuario no tiene una sesión activa.'; 
    END IF;

    -- 5.3 Verificación de Rol Autorizado
    SELECT EXISTS (
        SELECT 1 FROM "EmpleadoRol" er 
        JOIN "Roles" r ON er."claveRol" = r."claveRol"
        INNER JOIN "Usuarios" u ON u."claveEmpleado" = er."claveEmpleado"
        WHERE u."claveUsuario" = v_claveUsuario_op 
          AND r.nombre_rol IN ('Administrativo', 'Directivo') 
          AND er.fecha_fin IS NULL
    ) INTO v_rol_autorizado;

    IF NOT v_rol_autorizado THEN 
        RAISE EXCEPTION 'ACCESO DENEGADO: El rol del usuario no está autorizado para realizar cobros.'; 
    END IF;

    -- 5.4 Verificación de Permiso específico en el Módulo
    SELECT EXISTS (
        SELECT 1 FROM "Permisos" p 
        JOIN "Modulos" m ON p."claveModulo" = m."claveModulo"
        INNER JOIN "EmpleadoRol" er ON p."claveRol" = er."claveRol"
        INNER JOIN "Usuarios" u ON u."claveEmpleado" = er."claveEmpleado"
        WHERE u."claveUsuario" = v_claveUsuario_op 
          AND m.nombre_modulo = 'Inscribir' 
          AND p.puede_crear = TRUE 
          AND er.fecha_fin IS NULL
    ) INTO v_permiso_modulo;

    IF NOT v_permiso_modulo THEN 
        RAISE EXCEPTION 'ACCESO DENEGADO: No cuenta con permisos de creación en el módulo "Inscribir".'; 
    END IF;

    -- ============================================================
    -- 6. EJECUCIÓN DEL REGISTRO DE PAGO
    -- ============================================================
    v_referencia_auto := 'Cubrió cuota voluntaria del ciclo ' || v_nombre_ciclo;

    INSERT INTO "Pagos" (
        "claveTutor", 
        "claveUsuario", 
        "claveCiclo", 
        monto, 
        metodo_pago, 
        comprobante_pago, 
        referencia
    ) VALUES (
        p_claveTutor, 
        v_claveUsuario_op, 
        v_claveCiclo_act, 
        p_monto, 
        p_metodo_pago, 
        p_comprobante_pago, 
        v_referencia_auto
    ) RETURNING clavePago INTO v_clavePago_nueva;

    -- ============================================================
    -- 7. VINCULACIÓN FINAL (CIERRE DE PROCESO)
    -- ============================================================
    UPDATE "Inscripciones" 
    SET "clavePago" = v_clavePago_nueva
    WHERE "claveCiclo" = v_claveCiclo_act 
      AND "clavePago" IS NULL
      AND "claveAlumno" IN (
          SELECT "claveAlumno" FROM "TutoresAlumnos" 
          WHERE "claveTutor" = p_claveTutor AND fecha_baja IS NULL
      );

    -- Confirmación
    RAISE NOTICE 'PAGO EXITOSO: Folio % generado para el ciclo %.', v_clavePago_nueva, v_nombre_ciclo;

END;
$$;
