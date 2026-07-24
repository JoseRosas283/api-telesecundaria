using Microsoft.EntityFrameworkCore.Migrations;
using System.IO;

#nullable disable

namespace Telesecundaria.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EjecutarScriptsSqlFuncionesTriggersEtc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var basePath = AppContext.BaseDirectory;

            // ===================== FUNCTIONS =====================
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/fn_actualizar_inasistencias_citas.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/fn_mantenimiento_convocatorias_web.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/fn_mantenimiento_sesiones_tutor.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_adjuncion.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_adj_original.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_Alumno.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_asig.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_aspirante.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_ciclo.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_cita.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_codigo_recuperacion.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_convocatoria.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_destino.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_direccion.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_Documento.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_doc_aspirante.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_empleado.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_entrega.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_envio.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_Expediente.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_grupo.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_imagen.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_inscripcion.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_logueo.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_modulo.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_notificacion.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_pago.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_periodo.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_publicacion.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_receptor.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_Requisito.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_revision.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_revision_aceptada.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_Rol.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_ruta_rechazada.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_tipo_doc.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_tipo_notificacion.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_tutor.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_tutor_aspirante.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_usuario.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_lugar_fila_virtual.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_token_convocatoria.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/genera_clave_carga.sql");

            // ===================== TRIGGERS =====================
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Triggers/tg_notificar_cierre_adjuncion.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Triggers/tg_recorrer_fila_baja.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Triggers/trg_cuidador_despues_insertar.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Triggers/trg_cuidador_monitoreo_adjuncion.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Triggers/trg_entrega_completada.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Triggers/trg_notificar_cierre.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Triggers/tr_monitorear_cupo.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Triggers/tr_monitoreo_estados_inteligente.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Triggers/tr_notificar_cita_completa.sql");

            // ===================== PROCEDURES =====================
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/actualizar_detalle_revision.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/cerrar_revision_final.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/cierre_tutorAspirante_sesion.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/eliminar_expediente.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/procesar_revision.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/registrar_aceptacion_buffer.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/registrar_detalle_revision.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_actualizar_aspirante.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_actualizar_convocatoria.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_actualizar_expediente.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_actualizar_publicacion.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_actualizar_receptor.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_actualizar_ruta_documento_rechazado.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_actualizar_tutor.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_actualizar_tutor_Aspirante.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_actualizar_usuario.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_agendar_cita_individual.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_apertura_ciclo_escolar.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_asignar_alumno_grupo.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_asignar_rol_empleado.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_asignar_tutor_alumno.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_cerrar_sesion.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_configurar_destino_notificacion.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_configurar_requisito_etapa.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_confirmar_cambio_contrasena_tutor.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_cotejar_documento_fisico.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_crear_adjuncion_segura.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_dar_baja_rol_empleado.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_eliminar_aspirante.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_eliminar_convocatoria.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_eliminar_publicacion.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_eliminar_receptor.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_eliminar_tutor.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_eliminar_tutor_aspirante.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_eliminar_usuario.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_emitir_notificacion.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_formar_aspirante_fila.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_generar_codigo_recuperacion_tutor.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_generar_receptor.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_gestionar_permisos_por_nombre.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_inicializar_entrega.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_iniciar_sesion.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_insertar_alumno.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_insertar_aspirante.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_insertar_convocatoria.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_insertar_detalle_adjuncion.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_insertar_documento.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_insertar_documento_aspirante.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_insertar_empleado.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_insertar_expediente.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_insertar_imagen_galeria.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_insertar_modulo.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_insertar_publicacion.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_insertar_rol.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_insertar_tipo_documento.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_insertar_tipo_notificacion.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_insertar_tutor.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_insertar_tutor_con_direccion.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_insertar_usuario.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_login_tutor_aspirante.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_realizar_inscripcion.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_registrar_adjuncion_original.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_registrar_carga_documental.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_registrar_detalle_adjuncion_original.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_registrar_detalle_carga.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_registrar_envio_pendiente.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_registrar_pago.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/sp_validar_codigo_recuperacion_tutor.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Procedures/validar_e_insertar_ruta_rechazada.sql");

            // ===================== VIEWS =====================
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Views/Informacion_aspirantes.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Views/vista_detalles_adjuncion_completa.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Views/vista_monitoreo_fila.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Views/vw_detalle_requisitos.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Views/v_auditoria_completa_accesos.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Views/v_bandeja_priorizada_total.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Views/v_empleados_roles_basica.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Views/v_expediente_empleado_rol_usuario.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Views/v_matriz_permisos_configuracion.sql");

            // ===================== INDEXES =====================
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Indexes/idx_codigo_recuperacion_vigente.sql");
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Indexes/idx_envio_masivo_citas.sql");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }

        private static void ExecuteSqlFile(MigrationBuilder migrationBuilder, string basePath, string relativePath)
        {
            var fullPath = Path.Combine(basePath, relativePath);
            var sql = File.ReadAllText(fullPath);
            migrationBuilder.Sql(sql);
        }
    }
}
