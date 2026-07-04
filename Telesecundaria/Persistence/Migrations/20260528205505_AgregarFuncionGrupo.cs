using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Telesecundaria.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarFuncionGrupo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "claveGrupo",
                table: "Grupos",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValueSql: "generar_clave_grupo()",
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "claveGrupo",
                table: "Grupos",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(18)",
                oldMaxLength: 18,
                oldDefaultValueSql: "generar_clave_grupo()");
        }
    }
}
