using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Telesecundaria.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModulosPermisosTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_imagen_galeria",
                table: "Publicaciones");

            migrationBuilder.DropCheckConstraint(
                name: "ck_categoria_pub",
                table: "Publicaciones");

            migrationBuilder.RenameColumn(
                name: "obligatorio",
                table: "Requisitos",
                newName: "estado_requisito");

            migrationBuilder.RenameColumn(
                name: "claveImagen",
                table: "Publicaciones",
                newName: "claveImagenTercera");

            migrationBuilder.RenameIndex(
                name: "IX_Publicaciones_claveImagen",
                table: "Publicaciones",
                newName: "IX_Publicaciones_claveImagenTercera");

            migrationBuilder.AddColumn<string>(
                name: "contrasena",
                table: "TutorAspirante",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "Temporal123");

            migrationBuilder.AddColumn<bool>(
                name: "estado",
                table: "TipoDocumentos",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "claveImagenPrincipal",
                table: "Publicaciones",
                type: "character varying(18)",
                maxLength: 18,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "claveImagenSecundaria",
                table: "Publicaciones",
                type: "character varying(18)",
                maxLength: 18,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "claveAspirante",
                table: "Entregas",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "CargasDocumentos",
                columns: table => new
                {
                    claveCarga = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false, defaultValueSql: "genera_clave_carga()"),
                    claveExpediente = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveUsuario = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    fecha_carga = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    observaciones = table.Column<string>(type: "text", nullable: true),
                    estatus_validacion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "En Proceso")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CargasDocumentos", x => x.claveCarga);
                    table.ForeignKey(
                        name: "fk_expediente_carga",
                        column: x => x.claveExpediente,
                        principalTable: "Expedientes",
                        principalColumn: "claveExpediente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_usuario_carga",
                        column: x => x.claveUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "claveUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Logueos",
                columns: table => new
                {
                    claveLogueo = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false, defaultValueSql: "generar_clave_logueo()"),
                    claveUsuario = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true),
                    fecha_acceso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    estatus_intento = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    direccion_ip = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false, defaultValue: "0.0.0.0"),
                    agente_usuario = table.Column<string>(type: "text", nullable: true, defaultValue: "Desconocido"),
                    fecha_cierre = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logueos", x => x.claveLogueo);
                    table.CheckConstraint("chk_estatus_intento", "estatus_intento IN ('Exitoso','Contraseña Incorrecta','Usuario Suspendido','Usuario Inexistente','Sesión Finalizada')");
                    table.ForeignKey(
                        name: "fk_logueos_usuarios",
                        column: x => x.claveUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "claveUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Modulos",
                columns: table => new
                {
                    claveModulo = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false, defaultValueSql: "generar_clave_modulo()"),
                    nombre_modulo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    url_modulo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    estado_modulo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    claveModuloPadre = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modulos", x => x.claveModulo);
                    table.ForeignKey(
                        name: "fk_modulo_padre",
                        column: x => x.claveModuloPadre,
                        principalTable: "Modulos",
                        principalColumn: "claveModulo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TokenConvocatorias",
                columns: table => new
                {
                    claveTokenConvocatoria = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "generar_token_convocatoria()"),
                    token_original = table.Column<string>(type: "text", nullable: false),
                    claveTutorAspirante = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    fecha_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    fecha_expiracion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ip_origen = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    dispositivo_origen = table.Column<string>(type: "text", nullable: true),
                    estado_sesion = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenConvocatorias", x => x.claveTokenConvocatoria);
                    table.ForeignKey(
                        name: "fk_token_tutor",
                        column: x => x.claveTutorAspirante,
                        principalTable: "TutorAspirante",
                        principalColumn: "claveTutorAspirante",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetalleCarga",
                columns: table => new
                {
                    claveCarga = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveDocumento = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    archivo_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    fecha_subida = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleCarga", x => new { x.claveCarga, x.claveDocumento });
                    table.ForeignKey(
                        name: "fk_carga",
                        column: x => x.claveCarga,
                        principalTable: "CargasDocumentos",
                        principalColumn: "claveCarga",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_documento",
                        column: x => x.claveDocumento,
                        principalTable: "Documentos",
                        principalColumn: "claveDocumento",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Permisos",
                columns: table => new
                {
                    claveRol = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    claveModulo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    puede_ver = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    puede_crear = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    puede_editar = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    puede_eliminar = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    fecha_asignacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permisos", x => new { x.claveRol, x.claveModulo });
                    table.ForeignKey(
                        name: "fk_permiso_modulo",
                        column: x => x.claveModulo,
                        principalTable: "Modulos",
                        principalColumn: "claveModulo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_permiso_rol",
                        column: x => x.claveRol,
                        principalTable: "Roles",
                        principalColumn: "claveRol",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Publicaciones_claveImagenPrincipal",
                table: "Publicaciones",
                column: "claveImagenPrincipal");

            migrationBuilder.CreateIndex(
                name: "IX_Publicaciones_claveImagenSecundaria",
                table: "Publicaciones",
                column: "claveImagenSecundaria");

            migrationBuilder.AddCheckConstraint(
                name: "ck_categoria_pub",
                table: "Publicaciones",
                sql: "categoria IN ('Eventos Culturales','Noticia','Aviso','Convocatorias','Galería')");

            migrationBuilder.CreateIndex(
                name: "IX_Entregas_claveAspirante",
                table: "Entregas",
                column: "claveAspirante",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "ck_alumno_estado",
                table: "Alumnos",
                sql: "estado IN ('Activo','Baja')");

            migrationBuilder.CreateIndex(
                name: "IX_CargasDocumentos_claveExpediente",
                table: "CargasDocumentos",
                column: "claveExpediente",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CargasDocumentos_claveUsuario",
                table: "CargasDocumentos",
                column: "claveUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleCarga_claveDocumento",
                table: "DetalleCarga",
                column: "claveDocumento");

            migrationBuilder.CreateIndex(
                name: "IX_Logueos_claveUsuario",
                table: "Logueos",
                column: "claveUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Modulos_claveModuloPadre",
                table: "Modulos",
                column: "claveModuloPadre");

            migrationBuilder.CreateIndex(
                name: "IX_Modulos_nombre_modulo",
                table: "Modulos",
                column: "nombre_modulo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permisos_claveModulo",
                table: "Permisos",
                column: "claveModulo");

            migrationBuilder.CreateIndex(
                name: "IX_TokenConvocatorias_claveTutorAspirante",
                table: "TokenConvocatorias",
                column: "claveTutorAspirante");

            migrationBuilder.AddForeignKey(
                name: "fk_entregas_aspirante",
                table: "Entregas",
                column: "claveAspirante",
                principalTable: "Aspirantes",
                principalColumn: "claveAspirante",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_imagen_principal",
                table: "Publicaciones",
                column: "claveImagenPrincipal",
                principalTable: "GaleriaImagenes",
                principalColumn: "claveImagen");

            migrationBuilder.AddForeignKey(
                name: "fk_imagen_secundaria",
                table: "Publicaciones",
                column: "claveImagenSecundaria",
                principalTable: "GaleriaImagenes",
                principalColumn: "claveImagen");

            migrationBuilder.AddForeignKey(
                name: "fk_imagen_tercera",
                table: "Publicaciones",
                column: "claveImagenTercera",
                principalTable: "GaleriaImagenes",
                principalColumn: "claveImagen");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_entregas_aspirante",
                table: "Entregas");

            migrationBuilder.DropForeignKey(
                name: "fk_imagen_principal",
                table: "Publicaciones");

            migrationBuilder.DropForeignKey(
                name: "fk_imagen_secundaria",
                table: "Publicaciones");

            migrationBuilder.DropForeignKey(
                name: "fk_imagen_tercera",
                table: "Publicaciones");

            migrationBuilder.DropTable(
                name: "DetalleCarga");

            migrationBuilder.DropTable(
                name: "Logueos");

            migrationBuilder.DropTable(
                name: "Permisos");

            migrationBuilder.DropTable(
                name: "TokenConvocatorias");

            migrationBuilder.DropTable(
                name: "CargasDocumentos");

            migrationBuilder.DropTable(
                name: "Modulos");

            migrationBuilder.DropIndex(
                name: "IX_Publicaciones_claveImagenPrincipal",
                table: "Publicaciones");

            migrationBuilder.DropIndex(
                name: "IX_Publicaciones_claveImagenSecundaria",
                table: "Publicaciones");

            migrationBuilder.DropCheckConstraint(
                name: "ck_categoria_pub",
                table: "Publicaciones");

            migrationBuilder.DropIndex(
                name: "IX_Entregas_claveAspirante",
                table: "Entregas");

            migrationBuilder.DropCheckConstraint(
                name: "ck_alumno_estado",
                table: "Alumnos");

            migrationBuilder.DropColumn(
                name: "contrasena",
                table: "TutorAspirante");

            migrationBuilder.DropColumn(
                name: "estado",
                table: "TipoDocumentos");

            migrationBuilder.DropColumn(
                name: "claveImagenPrincipal",
                table: "Publicaciones");

            migrationBuilder.DropColumn(
                name: "claveImagenSecundaria",
                table: "Publicaciones");

            migrationBuilder.DropColumn(
                name: "claveAspirante",
                table: "Entregas");

            migrationBuilder.RenameColumn(
                name: "estado_requisito",
                table: "Requisitos",
                newName: "obligatorio");

            migrationBuilder.RenameColumn(
                name: "claveImagenTercera",
                table: "Publicaciones",
                newName: "claveImagen");

            migrationBuilder.RenameIndex(
                name: "IX_Publicaciones_claveImagenTercera",
                table: "Publicaciones",
                newName: "IX_Publicaciones_claveImagen");

            migrationBuilder.AddCheckConstraint(
                name: "ck_categoria_pub",
                table: "Publicaciones",
                sql: "categoria IN ('Eventos Culturales','Noticia','Aviso','Convocatorias')");

            migrationBuilder.AddForeignKey(
                name: "fk_imagen_galeria",
                table: "Publicaciones",
                column: "claveImagen",
                principalTable: "GaleriaImagenes",
                principalColumn: "claveImagen");
        }
    }
}
