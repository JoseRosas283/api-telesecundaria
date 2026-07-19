using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Telesecundaria.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarRefresh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CodigosRecuperacionTutor",
                columns: table => new
                {
                    claveCodigoRecuperacion = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false, defaultValueSql: "generar_clave_codigo_recuperacion()"),
                    claveTutorAspirante = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    codigo = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    fecha_expiracion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    usado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    token_confirmacion = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    token_expiracion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    token_usado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodigosRecuperacionTutor", x => x.claveCodigoRecuperacion);
                    table.ForeignKey(
                        name: "fk_codigo_tutor",
                        column: x => x.claveTutorAspirante,
                        principalTable: "TutorAspirante",
                        principalColumn: "claveTutorAspirante",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    clave_refresh_token = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    claveUsuario = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveLogueo = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    token = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    fecha_expiracion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revocado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.clave_refresh_token);
                    table.ForeignKey(
                        name: "fk_refresh_usuario",
                        column: x => x.claveUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "claveUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens_tutor",
                columns: table => new
                {
                    clave_refresh_token = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    claveTutorAspirante = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveTokenConvocatoria = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    token = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    fecha_expiracion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revocado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens_tutor", x => x.clave_refresh_token);
                    table.ForeignKey(
                        name: "fk_refresh_token_convocatoria",
                        column: x => x.claveTokenConvocatoria,
                        principalTable: "TokenConvocatorias",
                        principalColumn: "claveTokenConvocatoria",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_refresh_tutor",
                        column: x => x.claveTutorAspirante,
                        principalTable: "TutorAspirante",
                        principalColumn: "claveTutorAspirante",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CodigosRecuperacionTutor_claveTutorAspirante",
                table: "CodigosRecuperacionTutor",
                column: "claveTutorAspirante");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_claveUsuario",
                table: "refresh_tokens",
                column: "claveUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_token",
                table: "refresh_tokens",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_tutor_claveTokenConvocatoria",
                table: "refresh_tokens_tutor",
                column: "claveTokenConvocatoria");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_tutor_claveTutorAspirante",
                table: "refresh_tokens_tutor",
                column: "claveTutorAspirante");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_tutor_token",
                table: "refresh_tokens_tutor",
                column: "token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CodigosRecuperacionTutor");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "refresh_tokens_tutor");
        }
    }
}
