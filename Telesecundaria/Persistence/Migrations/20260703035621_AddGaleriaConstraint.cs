using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Telesecundaria.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGaleriaConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_tipo_recurso_galeria",
                table: "GaleriaImagenes");

            migrationBuilder.AddCheckConstraint(
                name: "ck_tipo_recurso_galeria",
                table: "GaleriaImagenes",
                sql: "tipo_recurso IN ('Eventos Culturales', 'Noticia', 'Aviso', 'Convocatorias', 'Galería', 'otros')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_tipo_recurso_galeria",
                table: "GaleriaImagenes");

            migrationBuilder.AddCheckConstraint(
                name: "ck_tipo_recurso_galeria",
                table: "GaleriaImagenes",
                sql: "tipo_recurso IN ('Eventos Culturales', 'Noticia', 'Aviso', 'Convocatorias', 'otros')");
        }
    }
}
