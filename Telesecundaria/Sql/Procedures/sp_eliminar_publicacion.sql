CREATE OR REPLACE PROCEDURE sp_eliminar_publicacion(
    p_clavePublicacion VARCHAR(18),
    p_nombreUsuario VARCHAR(100)
)
LANGUAGE plpgsql
AS $$
DECLARE
    -- Variables de control de usuario
    v_claveUsuario_Editor VARCHAR(18);
    v_rol_clave VARCHAR(18);
    v_rol_usuario VARCHAR(50);
    v_puede_eliminar BOOLEAN;
    
    -- Variables de la publicación
    v_categoria_actual VARCHAR(25);
    v_claveConvocatoria VARCHAR(18);
    v_estatus_visible BOOLEAN;
BEGIN
    -- 1. IDENTIFICACIÓN DE LA PUBLICACIÓN
    SELECT categoria, estatus_visible, "claveConvocatoria"
    IN INTO v_categoria_actual, v_estatus_visible, v_claveConvocatoria
    FROM "Publicaciones" 
    WHERE "clavePublicacion" = p_clavePublicacion;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Error: La publicación % no existe.', p_clavePublicacion;
    END IF;

    -- CANDADO DE ESTADO: Evita procesar si ya está fuera de aire
    IF v_estatus_visible = FALSE THEN
        RAISE EXCEPTION 'Aviso: La publicación ya se encuentra desactivada (estatus_visible = FALSE).';
    END IF;

    -- 2. VALIDACIÓN DE USUARIO (Debe estar Activo)
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
    IF NOT EXISTS (
        SELECT 1 FROM "Logueos" 
        WHERE "claveUsuario" = v_claveUsuario_Editor 
          AND estatus_intento = 'Exitoso' 
          AND fecha_cierre IS NULL
    ) THEN
        RAISE EXCEPTION 'Acceso Denegado: No se detectó una sesión activa para el usuario %.', p_nombreUsuario;
    END IF;

    -- 4. SELLO DE ROLES AUTORIZADOS
    IF v_rol_usuario NOT IN ('Administrativo', 'Docente', 'Directivo') THEN
        RAISE EXCEPTION 'Acceso Denegado: El rol % no tiene privilegios para esta operación.', v_rol_usuario;
    END IF;

    -- 5. PERMISO DINÁMICO POR MÓDULO
    SELECT p.puede_eliminar INTO v_puede_eliminar
    FROM "Permisos" p
    INNER JOIN "Modulos" m ON p."claveModulo" = m."claveModulo"
    WHERE p."claveRol" = v_rol_clave 
      AND m.nombre_modulo = 'PantallaPublicaciones'
      AND m.estado_modulo = TRUE;

    IF NOT COALESCE(v_puede_eliminar, FALSE) THEN
        RAISE EXCEPTION 'Acceso Denegado: El usuario no cuenta con permisos de ELIMINACIÓN en el módulo de Publicaciones.';
    END IF;

    -- 6. REGLA DE NEGOCIO: CONVOCATORIAS
    -- Solo el rol Directivo puede retirar publicaciones de tipo Convocatoria
    IF v_categoria_actual = 'Convocatorias' AND v_rol_usuario <> 'Directivo' THEN
        RAISE EXCEPTION 'Acceso Denegado: Solo un usuario con rol Directivo puede retirar una Convocatoria del portal.';
    END IF;

    -- 7. CANDADO DE INTEGRIDAD REFERENCIAL EVOLUCIONADO (ADJUNCIONES)
    -- Si hay documentos cargados, la publicación se queda por integridad del proceso
    IF v_categoria_actual = 'Convocatorias' AND v_claveConvocatoria IS NOT NULL THEN
        IF EXISTS (
            SELECT 1 
            FROM "Aspirantes" asp
            INNER JOIN "Adjunciones" adj ON asp."claveAspirante" = adj."claveAspirante"
            WHERE asp."claveConvocatoria" = v_claveConvocatoria
        ) THEN
            RAISE EXCEPTION 'Bloqueo de Integridad: La convocatoria ya tiene documentos cargados por aspirantes. No se puede retirar por transparencia y seguridad del proceso.';
        END IF;

        -- Aviso informativo si hay gente registrada pero sin archivos
        IF EXISTS (
            SELECT 1 FROM "Aspirantes" 
            WHERE "claveConvocatoria" = v_claveConvocatoria
        ) THEN
            RAISE NOTICE 'Aviso: Se retira la publicación. Existen aspirantes registrados que perderán la vista de la convocatoria, aunque ninguno había subido documentos aún.';
        END IF;
    END IF;

    -- ==========================================================
    -- 8. CIERRE LÓGICO Y REGISTRO DE FECHA DE RETIRO
    -- ==========================================================
    UPDATE "Publicaciones" SET
        estatus_visible = FALSE,
        destacado = FALSE,
        fecha_retiro = CURRENT_TIMESTAMP 
    WHERE "clavePublicacion" = p_clavePublicacion;

    RAISE NOTICE 'Éxito: Publicación % retirada del portal el %. La convocatoria vinculada (%) ha sido  eliminada correctamente.', 
                  p_clavePublicacion, CURRENT_TIMESTAMP, COALESCE(v_claveConvocatoria, 'N/A');

END;
$$;
