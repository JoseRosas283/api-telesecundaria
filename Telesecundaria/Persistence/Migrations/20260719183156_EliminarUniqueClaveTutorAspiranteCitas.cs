using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Telesecundaria.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EliminarUniqueClaveTutorAspiranteCitas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CitasInscripcion_claveTutorAspirante",
                table: "CitasInscripcion");

            migrationBuilder.CreateIndex(
                name: "IX_CitasInscripcion_claveTutorAspirante",
                table: "CitasInscripcion",
                column: "claveTutorAspirante");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CitasInscripcion_claveTutorAspirante",
                table: "CitasInscripcion");

            migrationBuilder.CreateIndex(
                name: "IX_CitasInscripcion_claveTutorAspirante",
                table: "CitasInscripcion",
                column: "claveTutorAspirante",
                unique: true);
        }
    }
}
