using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Telesecundaria.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarFuncionesClaves : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "claveUsuario",
                table: "Usuarios",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValueSql: "generar_clave_usuario()",
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18);

            migrationBuilder.AlterColumn<string>(
                name: "claveTutor",
                table: "Tutores",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValueSql: "generar_clave_tutor()",
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18);

            migrationBuilder.AlterColumn<string>(
                name: "claveTutorAspirante",
                table: "TutorAspirante",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValueSql: "generar_clave_tutor_aspirante()",
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18);

            migrationBuilder.AlterColumn<string>(
                name: "claveTipoNotificacion",
                table: "TipoNotificaciones",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValueSql: "generar_clave_tipo_notificacion()",
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18);

            migrationBuilder.AlterColumn<string>(
                name: "claveTipoDocumento",
                table: "TipoDocumentos",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValueSql: "generar_clave_tipo_doc()",
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18);

            migrationBuilder.AlterColumn<string>(
                name: "claveRol",
                table: "Roles",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValueSql: "generar_clave_Rol()",
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18);

            migrationBuilder.AlterColumn<string>(
                name: "claveRevisionAceptada",
                table: "RevisionesAceptadas",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValueSql: "generar_clave_revision_aceptada()",
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18);

            migrationBuilder.AlterColumn<string>(
                name: "claveRevision",
                table: "Revisiones",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValueSql: "generar_clave_revision()",
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18);

            migrationBuilder.AlterColumn<string>(
                name: "claveRequisito",
                table: "Requisitos",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValueSql: "generar_clave_Requisito()",
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18);

            migrationBuilder.AlterColumn<string>(
                name: "claveReceptor",
                table: "Receptores",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValueSql: "generar_clave_receptor()",
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18);

            migrationBuilder.AlterColumn<string>(
                name: "clavePublicacion",
                table: "Publicaciones",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValueSql: "generar_clave_publicacion()",
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18);

            migrationBuilder.AlterColumn<string>(
                name: "claveNotificacion",
                table: "Notificaciones",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValueSql: "generar_clave_notificacion()",
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18);

            migrationBuilder.AlterColumn<string>(
                name: "claveImagen",
                table: "GaleriaImagenes",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValueSql: "generar_clave_imagen()",
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18);

            migrationBuilder.AlterColumn<string>(
                name: "claveFila",
                table: "FilaVirtual",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValueSql: "generar_lugar_fila_virtual()",
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18);

            migrationBuilder.AlterColumn<string>(
                name: "claveExpediente",
                table: "Expedientes",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValueSql: "generar_clave_Expediente()",
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18);

            migrationBuilder.AlterColumn<string>(
                name: "claveEnvio",
                table: "Envios",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValueSql: "generar_clave_envio()",
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18);

            migrationBuilder.AlterColumn<string>(
                name: "claveEntrega",
                table: "Entregas",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValueSql: "generar_clave_entrega()",
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18);

            migrationBuilder.AlterColumn<string>(
                name: "claveEmpleado",
                table: "Empleados",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValueSql: "generar_clave_empleado()",
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18);

            migrationBuilder.AlterColumn<string>(
                name: "claveDocAspirante",
                table: "DocumentosAspirante",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValueSql: "generar_clave_doc_aspirante()",
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18);

            migrationBuilder.AlterColumn<string>(
                name: "claveDocumento",
                table: "Documentos",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValueSql: "generar_clave_Documento()",
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18);

            migrationBuilder.AlterColumn<string>(
                name: "claveDireccion",
                table: "Direcciones",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValueSql: "generar_clave_direccion()",
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18);

            migrationBuilder.AlterColumn<string>(
                name: "claveDestino",
                table: "DestinoNotificacion",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValueSql: "generar_clave_destino()",
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18);

            migrationBuilder.AlterColumn<string>(
                name: "claveConvocatoria",
                table: "Convocatorias",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValueSql: "generar_clave_convocatoria()",
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18);

            migrationBuilder.AlterColumn<string>(
                name: "claveCita",
                table: "CitasInscripcion",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValueSql: "generar_clave_cita()",
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18);

            migrationBuilder.AlterColumn<string>(
                name: "claveAspirante",
                table: "Aspirantes",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValueSql: "generar_clave_aspirante()",
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18);

            migrationBuilder.AlterColumn<string>(
                name: "claveAlumno",
                table: "Alumnos",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValueSql: "generar_clave_Alumno()",
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18);

            migrationBuilder.AlterColumn<string>(
                name: "claveAdjuncion",
                table: "Adjunciones",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValueSql: "generar_clave_adjuncion()",
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "claveUsuario",
                table: "Usuarios",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18,
                oldDefaultValueSql: "generar_clave_usuario()");

            migrationBuilder.AlterColumn<string>(
                name: "claveTutor",
                table: "Tutores",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18,
                oldDefaultValueSql: "generar_clave_tutor()");

            migrationBuilder.AlterColumn<string>(
                name: "claveTutorAspirante",
                table: "TutorAspirante",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18,
                oldDefaultValueSql: "generar_clave_tutor_aspirante()");

            migrationBuilder.AlterColumn<string>(
                name: "claveTipoNotificacion",
                table: "TipoNotificaciones",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18,
                oldDefaultValueSql: "generar_clave_tipo_notificacion()");

            migrationBuilder.AlterColumn<string>(
                name: "claveTipoDocumento",
                table: "TipoDocumentos",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18,
                oldDefaultValueSql: "generar_clave_tipo_doc()");

            migrationBuilder.AlterColumn<string>(
                name: "claveRol",
                table: "Roles",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18,
                oldDefaultValueSql: "generar_clave_Rol()");

            migrationBuilder.AlterColumn<string>(
                name: "claveRevisionAceptada",
                table: "RevisionesAceptadas",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18,
                oldDefaultValueSql: "generar_clave_revision_aceptada()");

            migrationBuilder.AlterColumn<string>(
                name: "claveRevision",
                table: "Revisiones",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18,
                oldDefaultValueSql: "generar_clave_revision()");

            migrationBuilder.AlterColumn<string>(
                name: "claveRequisito",
                table: "Requisitos",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18,
                oldDefaultValueSql: "generar_clave_Requisito()");

            migrationBuilder.AlterColumn<string>(
                name: "claveReceptor",
                table: "Receptores",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18,
                oldDefaultValueSql: "generar_clave_receptor()");

            migrationBuilder.AlterColumn<string>(
                name: "clavePublicacion",
                table: "Publicaciones",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18,
                oldDefaultValueSql: "generar_clave_publicacion()");

            migrationBuilder.AlterColumn<string>(
                name: "claveNotificacion",
                table: "Notificaciones",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18,
                oldDefaultValueSql: "generar_clave_notificacion()");

            migrationBuilder.AlterColumn<string>(
                name: "claveImagen",
                table: "GaleriaImagenes",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18,
                oldDefaultValueSql: "generar_clave_imagen()");

            migrationBuilder.AlterColumn<string>(
                name: "claveFila",
                table: "FilaVirtual",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18,
                oldDefaultValueSql: "generar_lugar_fila_virtual()");

            migrationBuilder.AlterColumn<string>(
                name: "claveExpediente",
                table: "Expedientes",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18,
                oldDefaultValueSql: "generar_clave_Expediente()");

            migrationBuilder.AlterColumn<string>(
                name: "claveEnvio",
                table: "Envios",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18,
                oldDefaultValueSql: "generar_clave_envio()");

            migrationBuilder.AlterColumn<string>(
                name: "claveEntrega",
                table: "Entregas",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18,
                oldDefaultValueSql: "generar_clave_entrega()");

            migrationBuilder.AlterColumn<string>(
                name: "claveEmpleado",
                table: "Empleados",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18,
                oldDefaultValueSql: "generar_clave_empleado()");

            migrationBuilder.AlterColumn<string>(
                name: "claveDocAspirante",
                table: "DocumentosAspirante",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18,
                oldDefaultValueSql: "generar_clave_doc_aspirante()");

            migrationBuilder.AlterColumn<string>(
                name: "claveDocumento",
                table: "Documentos",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18,
                oldDefaultValueSql: "generar_clave_Documento()");

            migrationBuilder.AlterColumn<string>(
                name: "claveDireccion",
                table: "Direcciones",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18,
                oldDefaultValueSql: "generar_clave_direccion()");

            migrationBuilder.AlterColumn<string>(
                name: "claveDestino",
                table: "DestinoNotificacion",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18,
                oldDefaultValueSql: "generar_clave_destino()");

            migrationBuilder.AlterColumn<string>(
                name: "claveConvocatoria",
                table: "Convocatorias",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18,
                oldDefaultValueSql: "generar_clave_convocatoria()");

            migrationBuilder.AlterColumn<string>(
                name: "claveCita",
                table: "CitasInscripcion",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18,
                oldDefaultValueSql: "generar_clave_cita()");

            migrationBuilder.AlterColumn<string>(
                name: "claveAspirante",
                table: "Aspirantes",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18,
                oldDefaultValueSql: "generar_clave_aspirante()");

            migrationBuilder.AlterColumn<string>(
                name: "claveAlumno",
                table: "Alumnos",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18,
                oldDefaultValueSql: "generar_clave_Alumno()");

            migrationBuilder.AlterColumn<string>(
                name: "claveAdjuncion",
                table: "Adjunciones",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18,
                oldDefaultValueSql: "generar_clave_adjuncion()");
        }
    }
}
