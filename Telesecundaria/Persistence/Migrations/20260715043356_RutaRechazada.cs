using System;
using Microsoft.EntityFrameworkCore.Migrations;
using System.IO;

#nullable disable

namespace Telesecundaria.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RutaRechazada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var basePath = AppContext.BaseDirectory;
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_ruta_rechazada.sql");

            migrationBuilder.CreateTable(
                name: "RutasRechazadas",
                columns: table => new
                {
                    claveRuta = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false, defaultValueSql: "generar_clave_ruta_rechazada()"),
                    claveAdjuncion = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveDocAspirante = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveRevision = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    ruta_archivo_rechazado = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    fecha_registro = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RutasRechazadas", x => x.claveRuta);
                    table.ForeignKey(
                        name: "fk_rutas_adjuncion",
                        column: x => x.claveAdjuncion,
                        principalTable: "Adjunciones",
                        principalColumn: "claveAdjuncion",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rutas_documento",
                        column: x => x.claveDocAspirante,
                        principalTable: "DocumentosAspirante",
                        principalColumn: "claveDocAspirante",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rutas_revision",
                        column: x => x.claveRevision,
                        principalTable: "Revisiones",
                        principalColumn: "claveRevision",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RutasRechazadas_claveAdjuncion",
                table: "RutasRechazadas",
                column: "claveAdjuncion");

            migrationBuilder.CreateIndex(
                name: "IX_RutasRechazadas_claveDocAspirante",
                table: "RutasRechazadas",
                column: "claveDocAspirante");

            migrationBuilder.CreateIndex(
                name: "IX_RutasRechazadas_claveRevision",
                table: "RutasRechazadas",
                column: "claveRevision");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RutasRechazadas");
        }

        private static void ExecuteSqlFile(MigrationBuilder migrationBuilder, string basePath, string relativePath)
        {
            var fullPath = Path.Combine(basePath, relativePath);
            var sql = File.ReadAllText(fullPath);
            migrationBuilder.Sql(sql);
        }
    }
}
