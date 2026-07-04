using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Telesecundaria.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CiclosPeriodosPagosInscripciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Alumnos_claveExpediente",
                table: "Alumnos");

            migrationBuilder.DropCheckConstraint(
                name: "ck_alumno_estado",
                table: "Alumnos");

            migrationBuilder.DropColumn(
                name: "ciclo_escolar",
                table: "AsignacionGrupo");

            migrationBuilder.DropColumn(
                name: "grado",
                table: "Alumnos");

            migrationBuilder.DropColumn(
                name: "grupo",
                table: "Alumnos");

            migrationBuilder.AddColumn<bool>(
                name: "estado",
                table: "Grupos",
                type: "boolean",
                nullable: true,
                defaultValue: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "fecha_asignacion",
                table: "AsignacionGrupo",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "claveCiclo",
                table: "AsignacionGrupo",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "claveUsuario",
                table: "AsignacionGrupo",
                type: "character varying(18)",
                maxLength: 18,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "estatus",
                table: "AsignacionGrupo",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                defaultValue: "ACTIVO");

            migrationBuilder.AlterColumn<string>(
                name: "matricula",
                table: "Alumnos",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "estado",
                table: "Alumnos",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                defaultValue: "Activo",
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.CreateTable(
                name: "CiclosEscolares",
                columns: table => new
                {
                    claveCiclo = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false, defaultValueSql: "generar_clave_ciclo()"),
                    nombreCiclo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    fechaInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    fechaFin = table.Column<DateOnly>(type: "date", nullable: false),
                    estatus = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CiclosEscolares", x => x.claveCiclo);
                    table.CheckConstraint("chk_fechas", "\"fechaFin\" > \"fechaInicio\"");
                });

            migrationBuilder.CreateTable(
                name: "Pagos",
                columns: table => new
                {
                    clavePago = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false, defaultValueSql: "generar_clave_pago()"),
                    claveTutor = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveUsuario = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveCiclo = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    monto = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    fecha_pago = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    metodo_pago = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    comprobante_pago = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    referencia = table.Column<string>(type: "text", nullable: true),
                    estado_pago = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagos", x => x.clavePago);
                    table.CheckConstraint("chk_metodo_pago", "metodo_pago IN ('Efectivo','Transferencia','Deposito')");
                    table.ForeignKey(
                        name: "fk_pago_ciclo",
                        column: x => x.claveCiclo,
                        principalTable: "CiclosEscolares",
                        principalColumn: "claveCiclo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_pago_tutor",
                        column: x => x.claveTutor,
                        principalTable: "Tutores",
                        principalColumn: "claveTutor",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_pago_usuario",
                        column: x => x.claveUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "claveUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Periodos",
                columns: table => new
                {
                    clavePeriodo = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false, defaultValueSql: "generar_clave_periodo()"),
                    claveCiclo = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    nombre_periodo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    fecha_inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    fecha_fin = table.Column<DateOnly>(type: "date", nullable: false),
                    estado_periodo = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    fecha_registro = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Periodos", x => x.clavePeriodo);
                    table.ForeignKey(
                        name: "fk_periodo_ciclo",
                        column: x => x.claveCiclo,
                        principalTable: "CiclosEscolares",
                        principalColumn: "claveCiclo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inscripciones",
                columns: table => new
                {
                    claveInscripcion = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false, defaultValueSql: "generar_clave_inscripcion()"),
                    claveAlumno = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveCiclo = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    clavePeriodo = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveGrupo = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true),
                    claveUsuario = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    clavePago = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true),
                    fecha_inscripcion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    estatus_inscripcion = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true, defaultValue: "PENDIENTE"),
                    observaciones = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inscripciones", x => x.claveInscripcion);
                    table.CheckConstraint("chk_estatus_ins", "estatus_inscripcion IN ('INSCRITO','CANCELADA')");
                    table.ForeignKey(
                        name: "fk_ins_alumno",
                        column: x => x.claveAlumno,
                        principalTable: "Alumnos",
                        principalColumn: "claveAlumno",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ins_ciclo",
                        column: x => x.claveCiclo,
                        principalTable: "CiclosEscolares",
                        principalColumn: "claveCiclo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ins_grupo",
                        column: x => x.claveGrupo,
                        principalTable: "Grupos",
                        principalColumn: "claveGrupo");
                    table.ForeignKey(
                        name: "fk_ins_pago",
                        column: x => x.clavePago,
                        principalTable: "Pagos",
                        principalColumn: "clavePago");
                    table.ForeignKey(
                        name: "fk_ins_periodo",
                        column: x => x.clavePeriodo,
                        principalTable: "Periodos",
                        principalColumn: "clavePeriodo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ins_usuario",
                        column: x => x.claveUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "claveUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionGrupo_claveCiclo",
                table: "AsignacionGrupo",
                column: "claveCiclo");

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionGrupo_claveUsuario",
                table: "AsignacionGrupo",
                column: "claveUsuario");

            migrationBuilder.AddCheckConstraint(
                name: "chk_estatus",
                table: "AsignacionGrupo",
                sql: "estatus IN ('ACTIVO','REPROBADO','APROBADO','BAJA')");

            migrationBuilder.CreateIndex(
                name: "IX_Alumnos_claveExpediente",
                table: "Alumnos",
                column: "claveExpediente",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "ck_alumno_estado",
                table: "Alumnos",
                sql: "estado IN ('Activo','Baja','Egresado')");

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_claveCiclo",
                table: "Inscripciones",
                column: "claveCiclo");

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_claveGrupo",
                table: "Inscripciones",
                column: "claveGrupo");

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_clavePago",
                table: "Inscripciones",
                column: "clavePago");

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_clavePeriodo",
                table: "Inscripciones",
                column: "clavePeriodo");

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_claveUsuario",
                table: "Inscripciones",
                column: "claveUsuario");

            migrationBuilder.CreateIndex(
                name: "uk_ins_alumno_periodo",
                table: "Inscripciones",
                columns: new[] { "claveAlumno", "clavePeriodo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_claveCiclo",
                table: "Pagos",
                column: "claveCiclo");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_claveTutor",
                table: "Pagos",
                column: "claveTutor");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_claveUsuario",
                table: "Pagos",
                column: "claveUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Periodos_claveCiclo",
                table: "Periodos",
                column: "claveCiclo",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_asig_ciclo",
                table: "AsignacionGrupo",
                column: "claveCiclo",
                principalTable: "CiclosEscolares",
                principalColumn: "claveCiclo",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_asig_usuario",
                table: "AsignacionGrupo",
                column: "claveUsuario",
                principalTable: "Usuarios",
                principalColumn: "claveUsuario",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_asig_ciclo",
                table: "AsignacionGrupo");

            migrationBuilder.DropForeignKey(
                name: "fk_asig_usuario",
                table: "AsignacionGrupo");

            migrationBuilder.DropTable(
                name: "Inscripciones");

            migrationBuilder.DropTable(
                name: "Pagos");

            migrationBuilder.DropTable(
                name: "Periodos");

            migrationBuilder.DropTable(
                name: "CiclosEscolares");

            migrationBuilder.DropIndex(
                name: "IX_AsignacionGrupo_claveCiclo",
                table: "AsignacionGrupo");

            migrationBuilder.DropIndex(
                name: "IX_AsignacionGrupo_claveUsuario",
                table: "AsignacionGrupo");

            migrationBuilder.DropCheckConstraint(
                name: "chk_estatus",
                table: "AsignacionGrupo");

            migrationBuilder.DropIndex(
                name: "IX_Alumnos_claveExpediente",
                table: "Alumnos");

            migrationBuilder.DropCheckConstraint(
                name: "ck_alumno_estado",
                table: "Alumnos");

            migrationBuilder.DropColumn(
                name: "estado",
                table: "Grupos");

            migrationBuilder.DropColumn(
                name: "claveCiclo",
                table: "AsignacionGrupo");

            migrationBuilder.DropColumn(
                name: "claveUsuario",
                table: "AsignacionGrupo");

            migrationBuilder.DropColumn(
                name: "estatus",
                table: "AsignacionGrupo");

            migrationBuilder.AlterColumn<DateTime>(
                name: "fecha_asignacion",
                table: "AsignacionGrupo",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "ciclo_escolar",
                table: "AsignacionGrupo",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "matricula",
                table: "Alumnos",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "estado",
                table: "Alumnos",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true,
                oldDefaultValue: "Activo");

            migrationBuilder.AddColumn<string>(
                name: "grado",
                table: "Alumnos",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "grupo",
                table: "Alumnos",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Alumnos_claveExpediente",
                table: "Alumnos",
                column: "claveExpediente");

            migrationBuilder.AddCheckConstraint(
                name: "ck_alumno_estado",
                table: "Alumnos",
                sql: "estado IN ('Activo','Baja')");
        }
    }
}
