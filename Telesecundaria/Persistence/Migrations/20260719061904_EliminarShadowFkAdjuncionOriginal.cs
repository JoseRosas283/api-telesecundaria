using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Telesecundaria.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EliminarShadowFkAdjuncionOriginal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entregas_AdjuncionesOriginales_AdjuncionOriginalClaveAdjOri~",
                table: "Entregas");

            migrationBuilder.DropIndex(
                name: "IX_Entregas_AdjuncionOriginalClaveAdjOriginal",
                table: "Entregas");

            migrationBuilder.DropColumn(
                name: "AdjuncionOriginalClaveAdjOriginal",
                table: "Entregas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdjuncionOriginalClaveAdjOriginal",
                table: "Entregas",
                type: "character varying(18)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Entregas_AdjuncionOriginalClaveAdjOriginal",
                table: "Entregas",
                column: "AdjuncionOriginalClaveAdjOriginal");

            migrationBuilder.AddForeignKey(
                name: "FK_Entregas_AdjuncionesOriginales_AdjuncionOriginalClaveAdjOri~",
                table: "Entregas",
                column: "AdjuncionOriginalClaveAdjOriginal",
                principalTable: "AdjuncionesOriginales",
                principalColumn: "claveAdjOriginal",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
