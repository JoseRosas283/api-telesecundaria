using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Telesecundaria.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarDeferrableFilaVirtual : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "fecha_carga",
                table: "AdjuncionesOriginales",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.Sql(@"
                ALTER TABLE ""FilaVirtual"" 
                DROP CONSTRAINT IF EXISTS uq_lugar_convocatoria;
                DROP INDEX IF EXISTS uq_lugar_convocatoria;
                ALTER TABLE ""FilaVirtual"" 
                ADD CONSTRAINT uq_lugar_convocatoria 
                UNIQUE (""claveConvocatoria"", ""numero_lugar"") 
                DEFERRABLE INITIALLY IMMEDIATE;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "fecha_carga",
                table: "AdjuncionesOriginales",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.Sql(@"
                ALTER TABLE ""FilaVirtual"" 
                DROP CONSTRAINT IF EXISTS uq_lugar_convocatoria;
            ");
        }
    }
}
