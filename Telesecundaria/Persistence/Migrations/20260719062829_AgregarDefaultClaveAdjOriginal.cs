using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Telesecundaria.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarDefaultClaveAdjOriginal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
    }
}
