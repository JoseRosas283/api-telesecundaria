CREATE OR REPLACE PROCEDURE sp_insertar_publicacion(
    p_titulo VARCHAR(150),
    p_subtitulo VARCHAR(200),
    p_cuerpo TEXT,
    p_categoria VARCHAR(25),
    p_nombreUsuario VARCHAR(100),
    -- Nuevos Slots
    p_imgPrincipal VARCHAR(18),
    p_imgSecundaria VARCHAR(18) DEFAULT NULL,
    p_imgTercera VARCHAR(18) DEFAULT NULL,
    p_claveConvocatoria VARCHAR(18) DEFAULT NULL,
    p_destacado BOOLEAN DEFAULT FALSE,
    p_estatus_visible BOOLEAN DEFAULT TRUE
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_claveUsuario VARCHAR(18);
    v_rol_clave VARCHAR(18);
    v_rol_usuario VARCHAR(50);
    v_sesion_activa BOOLEAN;
    v_puede_crear BOOLEAN;
    v_fecha_apertura TIMESTAMP;
    v_fecha_cierre TIMESTAMP;
    v_ya_publicada BOOLEAN;
    v_tipo_recurso_img VARCHAR(25);
    -- Array para iterar validaciones de congruencia
    v_imagenes VARCHAR(18)[] := ARRAY[p_imgPrincipal, p_imgSecundaria, p_imgTercera];
    v_img_actual VARCHAR(18);
BEGIN
    -- 1. VALIDACIÓN DE CAMPOS OBLIGATORIOS (AJUSTADO PARA GALERÍA)
    IF p_categoria IS NULL OR TRIM(p_categoria) = '' THEN
        RAISE EXCEPTION 'Error: La Categoría es obligatoria.';
    END IF;

    IF p_categoria <> 'Galería' THEN
        IF p_titulo IS NULL OR TRIM(p_titulo) = '' THEN
            RAISE EXCEPTION 'Error: El Título es obligatorio.';
        END IF;
        IF p_cuerpo IS NULL OR TRIM(p_cuerpo) = '' THEN
            RAISE EXCEPTION 'Error: El Cuerpo del contenido es obligatorio.';
        END IF;
    END IF;

    -- 2. VALIDACIÓN DE USUARIO, ROL Y ESTADO LABORAL (INTACTO)
    SELECT u."claveUsuario", r."claveRol", r.nombre_rol 
    INTO v_claveUsuario, v_rol_clave, v_rol_usuario
    FROM "Usuarios" u
    INNER JOIN "Empleados" e ON u."claveEmpleado" = e."claveEmpleado"
    INNER JOIN "EmpleadoRol" er ON e."claveEmpleado" = er."claveEmpleado"
    INNER JOIN "Roles" r ON er."claveRol" = r."claveRol"
    WHERE u.nombre_usuario = TRIM(p_nombreUsuario) 
      AND u.estado = TRUE 
      AND e.estatus_laboral = 'Activo';

    IF v_claveUsuario IS NULL THEN
        RAISE EXCEPTION 'Error: Usuario "%" inexistente o inactivo.', p_nombreUsuario;
    END IF;

    -- 3. CANDADO DE SESIÓN ACTIVA (INTACTO)
    SELECT EXISTS (
        SELECT 1 FROM "Logueos" 
        WHERE "claveUsuario" = v_claveUsuario 
          AND estatus_intento = 'Exitoso' 
          AND fecha_cierre IS NULL
    ) INTO v_sesion_activa;

    IF NOT v_sesion_activa THEN
        RAISE EXCEPTION 'Acceso Denegado: No se detectó una sesión activa.';
    END IF;

    -- 4. SELLO DE ROLES AUTORIZADOS (INTACTO)
    IF v_rol_usuario NOT IN ('Administrativo', 'Docente', 'Directivo') THEN
        RAISE EXCEPTION 'Acceso Denegado: El rol % no está autorizado para realizar publicaciones.', v_rol_usuario;
    END IF;

    -- 5. CANDADO DE PERMISOS DINÁMICOS (INTACTO)
    SELECT p.puede_crear INTO v_puede_crear
    FROM "Permisos" p
    INNER JOIN "Modulos" m ON p."claveModulo" = m."claveModulo"
    WHERE p."claveRol" = v_rol_clave 
      AND m.nombre_modulo = 'PantallaPublicaciones'
      AND m.estado_modulo = TRUE;

    IF v_puede_crear IS NULL OR v_puede_crear = FALSE THEN
        RAISE EXCEPTION 'Acceso Denegado: No cuenta con permisos de creación en el módulo Publicaciones.';
    END IF;

    -- 6. REGLA DE NEGOCIO CRÍTICA: Convocatorias (INTACTO)
    IF p_categoria = 'Convocatorias' AND v_rol_usuario <> 'Directivo' THEN
        RAISE EXCEPTION 'Acceso Denegado: Solo el Directivo tiene la facultad de publicar Convocatorias.';
    END IF;

    -- 7. REGLAS DE OBLIGATORIEDAD DE IMÁGENES POR CATEGORÍA
    IF p_categoria = 'Convocatorias' AND (p_imgPrincipal IS NULL OR TRIM(p_imgPrincipal) = '') THEN
        RAISE EXCEPTION 'Error: Convocatorias requieren imagen principal obligatoriamente.';
    END IF;

    IF p_categoria IN ('Eventos Culturales', 'Galería') THEN
        IF (p_imgPrincipal IS NULL OR TRIM(p_imgPrincipal) = '') OR 
           (p_imgSecundaria IS NULL OR TRIM(p_imgSecundaria) = '') OR 
           (p_imgTercera IS NULL OR TRIM(p_imgTercera) = '') THEN
            RAISE EXCEPTION 'Error: % requiere las 3 imágenes obligatoriamente (no pueden estar vacías).', p_categoria;
        END IF;
    END IF;

    -- 7.2 RESTRICCIÓN DE EXCESO DE IMÁGENES (NUEVO CAMBIO)
    IF p_categoria IN ('Noticia', 'Aviso', 'Convocatorias') 
       AND (p_imgSecundaria IS NOT NULL OR p_imgTercera IS NOT NULL) THEN
        RAISE EXCEPTION 'Bloqueo: La categoría % solo admite una imagen (Slot Principal). Limpie los slots secundarios.', p_categoria;
    END IF;

    -- 8. CONTROL DE CONGRUENCIA Y USO ACTIVO (APLICADO A LOS 3 SLOTS)
    FOREACH v_img_actual IN ARRAY v_imagenes
    LOOP
        IF v_img_actual IS NOT NULL AND TRIM(v_img_actual) <> '' THEN
            -- Validación de Existencia y Congruencia
            SELECT tipo_recurso INTO v_tipo_recurso_img 
            FROM "GaleriaImagenes" WHERE "claveImagen" = v_img_actual;

            IF v_tipo_recurso_img IS NULL THEN
                RAISE EXCEPTION 'Error: La imagen % no existe.', v_img_actual;
            END IF;

            IF v_tipo_recurso_img <> p_categoria AND v_tipo_recurso_img <> 'otros' THEN
                RAISE EXCEPTION 'Bloqueo de Congruencia: Imagen % tipo "%" no válida para publicación "%".', v_img_actual, v_tipo_recurso_img, p_categoria;
            END IF;

            -- Validación de Uso Activo
            IF EXISTS (
                SELECT 1 FROM "Publicaciones" 
                WHERE estatus_visible = TRUE AND (
                    "claveImagenPrincipal" = v_img_actual OR 
                    "claveImagenSecundaria" = v_img_actual OR 
                    "claveImagenTercera" = v_img_actual
                )
            ) THEN
                RAISE EXCEPTION 'Error: La imagen % ya está en uso activo.', v_img_actual;
            END IF;
        END IF;
    END LOOP;

    -- 9. LÓGICA PARA CONVOCATORIAS
    IF p_categoria = 'Convocatorias' THEN
        IF p_claveConvocatoria IS NULL OR TRIM(p_claveConvocatoria) = '' THEN
            RAISE EXCEPTION 'Error: Se requiere claveConvocatoria.';
        END IF;

        SELECT EXISTS (
            SELECT 1 FROM "Publicaciones" 
            WHERE "claveConvocatoria" = p_claveConvocatoria AND estatus_visible = TRUE
        ) INTO v_ya_publicada;

        IF v_ya_publicada THEN
            RAISE EXCEPTION 'Error: La convocatoria % ya tiene un post activo.', p_claveConvocatoria;
        END IF;

        SELECT fecha_inicio, fecha_fin INTO v_fecha_apertura, v_fecha_cierre
        FROM "Convocatorias" WHERE "claveConvocatoria" = p_claveConvocatoria;

        IF NOT FOUND THEN
            RAISE EXCEPTION 'Error: La convocatoria % no existe.', p_claveConvocatoria;
        END IF;
    ELSE
        IF p_claveConvocatoria IS NOT NULL THEN
            RAISE EXCEPTION 'Error: No se vinculan convocatorias a otros tipos de post.';
        END IF;
        v_fecha_apertura := CURRENT_TIMESTAMP;
        v_fecha_cierre := NULL;
    END IF;

    -- 10. INSERCIÓN FINAL
    INSERT INTO "Publicaciones" (
        titulo, subtitulo, cuerpo_contenido, categoria, 
        fecha_aparicion, fecha_retiro, "claveUsuario", 
        "claveConvocatoria", "claveImagenPrincipal", "claveImagenSecundaria", "claveImagenTercera", 
        destacado, estatus_visible
    ) VALUES (
        TRIM(p_titulo), TRIM(p_subtitulo), TRIM(p_cuerpo), p_categoria, 
        v_fecha_apertura, v_fecha_cierre, v_claveUsuario, 
        p_claveConvocatoria, p_imgPrincipal, p_imgSecundaria, p_imgTercera, 
        p_destacado, p_estatus_visible
    );

    RAISE NOTICE 'Éxito: Publicación registrada por el usuario % (%)', p_nombreUsuario, v_rol_usuario;
END;
$$;