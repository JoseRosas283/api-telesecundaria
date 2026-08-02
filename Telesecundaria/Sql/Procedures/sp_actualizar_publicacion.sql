CREATE OR REPLACE PROCEDURE sp_actualizar_publicacion(
    p_clavePublicacion VARCHAR(18), 
    p_nombreUsuario VARCHAR(100),
    p_titulo VARCHAR(150),
    p_subtitulo VARCHAR(200),
    p_cuerpo TEXT,
    p_imgPrincipal VARCHAR(18),
    p_imgSecundaria VARCHAR(18) DEFAULT NULL,
    p_imgTercera VARCHAR(18) DEFAULT NULL,
    p_destacado BOOLEAN DEFAULT FALSE
)
LANGUAGE plpgsql
AS $$
DECLARE
    -- Variables de control de usuario
    v_claveUsuario_Editor VARCHAR(18);
    v_rol_clave VARCHAR(18);
    v_rol_usuario VARCHAR(50);
    v_puede_editar BOOLEAN;
    
    -- Variables heredadas de la raíz
    v_categoria_actual VARCHAR(25);
    v_claveConvocatoria VARCHAR(18);
    v_visible_heredada BOOLEAN;
    v_fecha_aparicion_pub TIMESTAMP;
    v_fecha_retiro_pub TIMESTAMP;
    v_estado_conv_raiz VARCHAR(15); -- Para el salvoconducto
    
    -- Variable tipo RECORD para capturar la tupla actual completa de la publicación
    v_pub_actual RECORD;
    
    -- Lógica de imágenes
    v_tipo_recurso_img VARCHAR(25);
    v_imagenes VARCHAR(18)[] := ARRAY[p_imgPrincipal, p_imgSecundaria, p_imgTercera];
    v_img_actual VARCHAR(18);
BEGIN
    -- 1. IDENTIFICACIÓN Y DATOS DE LA RAÍZ (Incluimos estado de convocatoria)
    -- Traemos la tupla completa (*) para alimentar el filtro inteligente ROW sin perder la raíz
    SELECT p.*, c.estado AS estado_conv_raiz
    INTO v_pub_actual
    FROM "Publicaciones" p
    LEFT JOIN "Convocatorias" c ON p."claveConvocatoria" = c."claveConvocatoria"
    WHERE p."clavePublicacion" = p_clavePublicacion;

    IF v_pub_actual."clavePublicacion" IS NULL THEN
        RAISE EXCEPTION 'Error: La publicación % no existe.', p_clavePublicacion;
    END IF;

    -- Asignación de tus variables originales a partir del registro obtenido
    v_categoria_actual    := v_pub_actual.categoria;
    v_claveConvocatoria   := v_pub_actual."claveConvocatoria";
    v_visible_heredada     := v_pub_actual.estatus_visible;
    v_fecha_aparicion_pub := v_pub_actual.fecha_aparicion;
    v_fecha_retiro_pub    := v_pub_actual.fecha_retiro;
    v_estado_conv_raiz    := v_pub_actual.estado_conv_raiz;

    -- CANDADO DE ESTADO CON SALVOCONDUCTO
    IF v_visible_heredada = FALSE THEN
        -- Excepción: Si es Convocatoria y está Programada, permitimos el acceso
        IF NOT (v_categoria_actual = 'Convocatorias' AND v_estado_conv_raiz = 'Programada') THEN
            RAISE EXCEPTION 'Acceso Denegado: La publicación está fuera de aire. Solo se permite editar si es una Convocatoria Programada.';
        END IF;
        
        RAISE NOTICE 'Aviso: Editando Convocatoria Programada fuera del aire.';
    END IF;

    -- =========================================================================
    -- 1.5 FILTRO DE CAMBIOS REALES (AÑADIDO SIN ALTERAR LAS DEMÁS VALIDACIONES)
    -- =========================================================================
    IF ROW(v_pub_actual.titulo, v_pub_actual.subtitulo, v_pub_actual.cuerpo_contenido, 
           v_pub_actual."claveImagenPrincipal", v_pub_actual."claveImagenSecundaria", v_pub_actual."claveImagenTercera", 
           v_pub_actual.destacado)
       IS NOT DISTINCT FROM
       ROW(TRIM(p_titulo), TRIM(p_subtitulo), TRIM(p_cuerpo), 
           p_imgPrincipal, NULLIF(p_imgSecundaria, ''), NULLIF(p_imgTercera, ''), 
           p_destacado)
    THEN
        RAISE NOTICE 'Éxito: Contenido guardado. Los datos enviados son idénticos a los actuales; no se realizaron cambios ni se generaron nuevas versiones %.', p_clavePublicacion;
        RETURN; -- Detiene la ejecución limpia e inmediata antes de procesar el resto de la lógica
    END IF;

    -- 2. VALIDACIÓN DE USUARIO, ROL Y ESTADO LABORAL
    SELECT u."claveUsuario", r."claveRol", r.nombre_rol 
    INTO v_claveUsuario_Editor, v_rol_clave, v_rol_usuario
    FROM "Usuarios" u
    INNER JOIN "Empleados" e ON u."claveEmpleado" = e."claveEmpleado"
    INNER JOIN "EmpleadoRol" er ON e."claveEmpleado" = er."claveEmpleado"
    INNER JOIN "Roles" r ON er."claveRol" = r."claveRol"
    WHERE u."nombre_usuario" = TRIM(p_nombreUsuario) 
      AND u.estado = TRUE 
      AND e.estatus_laboral = 'Activo';

    IF v_claveUsuario_Editor IS NULL THEN
        RAISE EXCEPTION 'Error: Usuario "%" inexistente o inactivo.', p_nombreUsuario;
    END IF;

    -- 3. CANDADO DE SESIÓN ACTIVA
    IF NOT EXISTS (SELECT 1 FROM "Logueos" WHERE "claveUsuario" = v_claveUsuario_Editor AND estatus_intento = 'Exitoso' AND fecha_cierre IS NULL) THEN
        RAISE EXCEPTION 'Acceso Denegado: No se detectó una sesión activa.';
    END IF;

    -- 4. SELLO DE ROLES AUTORIZADOS
    IF v_rol_usuario NOT IN ('Administrativo', 'Docente', 'Directivo') THEN
        RAISE EXCEPTION 'Acceso Denegado: El rol % no está autorizado.', v_rol_usuario;
    END IF;

    -- 5. PERMISO DINÁMICO (puede_editar)
    SELECT p.puede_editar INTO v_puede_editar
    FROM "Permisos" p
    INNER JOIN "Modulos" m ON p."claveModulo" = m."claveModulo"
    WHERE p."claveRol" = v_rol_clave 
      AND m.nombre_modulo = 'PantallaPublicaciones'
      AND m.estado_modulo = TRUE;

    IF NOT COALESCE(v_puede_editar, FALSE) THEN
        RAISE EXCEPTION 'Acceso Denegado: No cuenta con permisos de EDICIÓN.';
    END IF;

    -- 6. OBLIGATORIEDAD DE TEXTO
    IF v_categoria_actual <> 'Galería' THEN
        IF p_titulo IS NULL OR TRIM(p_titulo) = '' OR p_cuerpo IS NULL OR TRIM(p_cuerpo) = '' THEN
            RAISE EXCEPTION 'Error: El Título y el Cuerpo son obligatorios para la categoría %.', v_categoria_actual;
        END IF;
    END IF;

    -- 7. RESTRICCIÓN DE ROL PARA CONVOCATORIAS
    IF v_categoria_actual = 'Convocatorias' AND v_rol_usuario <> 'Directivo' THEN
        RAISE EXCEPTION 'Acceso Denegado: Solo el Directivo edita contenido de Convocatorias.';
    END IF;

    -- 8. VALIDACIÓN DE IMÁGENES
    IF v_categoria_actual = 'Convocatorias' AND (p_imgPrincipal IS NULL OR TRIM(p_imgPrincipal) = '') THEN
        RAISE EXCEPTION 'Error: Convocatorias requieren imagen principal obligatoriamente.';
    END IF;

    IF v_categoria_actual IN ('Eventos Culturales', 'Galería') THEN
        IF p_imgPrincipal IS NULL OR p_imgSecundaria IS NULL OR p_imgTercera IS NULL THEN
            RAISE EXCEPTION 'Error: Requiere las 3 imágenes obligatoriamente.';
        END IF;
    END IF;

    IF v_categoria_actual IN ('Noticia', 'Aviso', 'Convocatorias') 
       AND (p_imgSecundaria IS NOT NULL OR p_imgTercera IS NOT NULL) THEN
        RAISE EXCEPTION 'Bloqueo: La categoría % solo admite una imagen.', v_categoria_actual;
    END IF;

    -- 9. CONTROL DE CONGRUENCIA DE IMÁGENES
    FOREACH v_img_actual IN ARRAY v_imagenes
    LOOP
        IF v_img_actual IS NOT NULL AND TRIM(v_img_actual) <> '' THEN
            SELECT tipo_recurso INTO v_tipo_recurso_img 
            FROM "GaleriaImagenes" WHERE "claveImagen" = v_img_actual;

            IF v_tipo_recurso_img IS NULL THEN
                RAISE EXCEPTION 'Error: La imagen % no existe.', v_img_actual;
            END IF;

            IF v_tipo_recurso_img <> v_categoria_actual AND v_tipo_recurso_img <> 'otros' THEN
                RAISE EXCEPTION 'Bloqueo: Imagen % no válida para %.', v_img_actual, v_categoria_actual;
            END IF;

            IF EXISTS (
                SELECT 1 FROM "Publicaciones" 
                WHERE estatus_visible = TRUE 
                  AND "clavePublicacion" <> p_clavePublicacion 
                  AND ("claveImagenPrincipal" = v_img_actual OR 
                       "claveImagenSecundaria" = v_img_actual OR 
                       "claveImagenTercera" = v_img_actual)
            ) THEN
                RAISE EXCEPTION 'Error: La imagen % ya está en uso activo.', v_img_actual;
            END IF;
        END IF;
    END LOOP;

    -- 10. RELEVO DE VERSIONES (Con sello de tiempo de retiro)
    UPDATE "Publicaciones" SET
        estatus_visible = FALSE,
        destacado = FALSE,
        fecha_retiro = CURRENT_TIMESTAMP  
    WHERE "clavePublicacion" = p_clavePublicacion;

    INSERT INTO "Publicaciones" (
        titulo, subtitulo, cuerpo_contenido, categoria,
        fecha_aparicion, fecha_retiro, "claveUsuario", 
        "claveConvocatoria", "claveImagenPrincipal", 
        "claveImagenSecundaria", "claveImagenTercera",
        destacado, estatus_visible
    )
    VALUES (
        TRIM(p_titulo), TRIM(p_subtitulo), TRIM(p_cuerpo), v_categoria_actual,
        v_fecha_aparicion_pub, v_fecha_retiro_pub, v_claveUsuario_Editor, 
        v_claveConvocatoria, p_imgPrincipal, p_imgSecundaria, p_imgTercera,
        p_destacado, TRUE 
    );

    RAISE NOTICE 'Éxito: Publicación % actualizada (Relevo de versión con salvoconducto aplicado).', p_clavePublicacion;
END;
$$;
