using System;
using Microsoft.EntityFrameworkCore.Migrations;
using System.IO;

#nullable disable

namespace Telesecundaria.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarDefaultClaveAdjOriginal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var basePath = AppContext.BaseDirectory;
            ExecuteSqlFile(migrationBuilder, basePath, "Sql/Functions/generar_clave_adj_original.sql");

            migrationBuilder.AlterColumn<string>(
                name: "claveAdjOriginal",
                table: "AdjuncionesOriginales",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValueSql: "generar_clave_adj_original()",
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "claveAdjOriginal",
                table: "AdjuncionesOriginales",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18,
                oldDefaultValueSql: "generar_clave_adj_original()");
        }

        private static void ExecuteSqlFile(MigrationBuilder migrationBuilder, string basePath, string relativePath)
        {
            var fullPath = Path.Combine(basePath, relativePath);
            var sql = File.ReadAllText(fullPath);
            migrationBuilder.Sql(sql);
        }
    }
}
