using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Telesecundaria.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Convocatorias",
                columns: table => new
                {
                    claveConvocatoria = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    titulo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: false),
                    fecha_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_fin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ciclo_escolar = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    cupo_maximo = table.Column<int>(type: "integer", nullable: true),
                    cupo_disponible = table.Column<int>(type: "integer", nullable: true),
                    activacion = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    estado = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    fecha_registro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Convocatorias", x => x.claveConvocatoria);
                });

            migrationBuilder.CreateTable(
                name: "GaleriaImagenes",
                columns: table => new
                {
                    claveImagen = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    nombre_archivo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ruta_url = table.Column<string>(type: "text", nullable: false),
                    tipo_recurso = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    fecha_registro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GaleriaImagenes", x => x.claveImagen);
                    table.CheckConstraint("ck_tipo_recurso_galeria", "tipo_recurso IN ('Eventos Culturales', 'Noticia', 'Aviso', 'Convocatorias', 'otros')");
                });

            migrationBuilder.CreateTable(
                name: "Grupos",
                columns: table => new
                {
                    claveGrupo = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    grado = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    seccion = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    capacidad_maxima = table.Column<int>(type: "integer", nullable: false),
                    generacion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grupos", x => x.claveGrupo);
                    table.CheckConstraint("ck_grado_grupo", "grado IN ('1','2','3')");
                    table.CheckConstraint("ck_seccion_grupo", "seccion IN ('A','B')");
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    claveRol = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    nombre_rol = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.claveRol);
                    table.CheckConstraint("ck_nombre_rol", "nombre_rol IN ('Directivo','Administrativo','Docente','Intendente')");
                });

            migrationBuilder.CreateTable(
                name: "TipoDocumentos",
                columns: table => new
                {
                    claveTipoDocumento = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    nombre_documento = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    area = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoDocumentos", x => x.claveTipoDocumento);
                    table.CheckConstraint("ck_area_tipo_doc", "area IN ('Preinscripción','Inscripción','Becas','Egreso','Laboral','Institucional')");
                });

            migrationBuilder.CreateTable(
                name: "TipoNotificaciones",
                columns: table => new
                {
                    claveTipoNotificacion = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    nombre_proceso = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: false),
                    icono = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    color = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoNotificaciones", x => x.claveTipoNotificacion);
                    table.CheckConstraint("ck_nombre_proceso", "nombre_proceso IN ('Documentos Rechazados','Documentos Aceptados','Cierre de Adjuncion','Citas', 'Inscripciones', 'Institucionales','Docencia','Directivas','Administrativas')");
                });

            migrationBuilder.CreateTable(
                name: "TutorAspirante",
                columns: table => new
                {
                    claveTutorAspirante = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    apellido_paterno = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    apellido_materno = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    curp_tutor = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    telefono = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    correo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    parentesco = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TutorAspirante", x => x.claveTutorAspirante);
                });

            migrationBuilder.CreateTable(
                name: "Tutores",
                columns: table => new
                {
                    claveTutor = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    apellido_paterno = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    apellido_materno = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    curp_tutor = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    telefono = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    correo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    parentesco = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tutores", x => x.claveTutor);
                });

            migrationBuilder.CreateTable(
                name: "Requisitos",
                columns: table => new
                {
                    claveRequisito = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    etapa_proceso = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    obligatorio = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    formato_exigido = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    claveTipoDocumento = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requisitos", x => x.claveRequisito);
                    table.CheckConstraint("ck_etapa_proceso", "etapa_proceso IN ('Preinscripción','Inscripción','Becas')");
                    table.ForeignKey(
                        name: "fk_tipo_doc",
                        column: x => x.claveTipoDocumento,
                        principalTable: "TipoDocumentos",
                        principalColumn: "claveTipoDocumento",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DestinoNotificacion",
                columns: table => new
                {
                    claveDestino = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveTipoNotificacion = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    tipo_receptor = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DestinoNotificacion", x => x.claveDestino);
                    table.CheckConstraint("ck_tipo_receptor_destino", "tipo_receptor IN ('TutorAspirante','Tutor','Usuario')");
                    table.ForeignKey(
                        name: "fk_tipo_notif",
                        column: x => x.claveTipoNotificacion,
                        principalTable: "TipoNotificaciones",
                        principalColumn: "claveTipoNotificacion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Aspirantes",
                columns: table => new
                {
                    claveAspirante = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    apellido_paterno = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    apellido_materno = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    curp = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    escuela_procedencia = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    promedio_primaria = table.Column<decimal>(type: "numeric(3,1)", precision: 3, scale: 1, nullable: false),
                    tiene_discapacidad = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    nombre_enfermedad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Hermano_Plantel = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    curp_hermano = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    estatus_aspirante = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    fecha_registro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    claveConvocatoria = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveTutorAspirante = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aspirantes", x => x.claveAspirante);
                    table.CheckConstraint("ck_estatus_aspirante", "estatus_aspirante IN ('En proceso','Aceptado','Rechazado')");
                    table.ForeignKey(
                        name: "fk_aspirante_convocatoria",
                        column: x => x.claveConvocatoria,
                        principalTable: "Convocatorias",
                        principalColumn: "claveConvocatoria",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_aspirante_tutor",
                        column: x => x.claveTutorAspirante,
                        principalTable: "TutorAspirante",
                        principalColumn: "claveTutorAspirante",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Direcciones",
                columns: table => new
                {
                    claveDireccion = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    calle_numero = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    colonia = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    codigo_postal = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    municipio = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    estado_verificacion = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    claveTutorAspirante = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Direcciones", x => x.claveDireccion);
                    table.ForeignKey(
                        name: "fk_direcciones_tutor",
                        column: x => x.claveTutorAspirante,
                        principalTable: "TutorAspirante",
                        principalColumn: "claveTutorAspirante",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Adjunciones",
                columns: table => new
                {
                    claveAdjuncion = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    fecha_envio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    estatus_gral = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    estatus_operativo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Abierta"),
                    claveTutorAspirante = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveAspirante = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adjunciones", x => x.claveAdjuncion);
                    table.CheckConstraint("ck_estatus_gral", "estatus_gral IN ('Pendiente','Aceptada','Rechazada')");
                    table.CheckConstraint("ck_estatus_operativo_adj", "estatus_operativo IN ('Abierta','Cerrada')");
                    table.ForeignKey(
                        name: "fk_adj_aspirante",
                        column: x => x.claveAspirante,
                        principalTable: "Aspirantes",
                        principalColumn: "claveAspirante",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_adj_tutor",
                        column: x => x.claveTutorAspirante,
                        principalTable: "TutorAspirante",
                        principalColumn: "claveTutorAspirante",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentosAspirante",
                columns: table => new
                {
                    claveDocAspirante = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    folio_documento = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    valor_analitico = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ruta_archivo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    claveAspirante = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveTipoDocumento = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentosAspirante", x => x.claveDocAspirante);
                    table.CheckConstraint("ck_valor_analitico", "valor_analitico IN ('Copia Digital','Original')");
                    table.ForeignKey(
                        name: "fk_doc_aspirante",
                        column: x => x.claveAspirante,
                        principalTable: "Aspirantes",
                        principalColumn: "claveAspirante",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_doc_tipo",
                        column: x => x.claveTipoDocumento,
                        principalTable: "TipoDocumentos",
                        principalColumn: "claveTipoDocumento",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FilaVirtual",
                columns: table => new
                {
                    claveFila = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveConvocatoria = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveAspirante = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    numero_lugar = table.Column<int>(type: "integer", nullable: false),
                    fecha_asignacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilaVirtual", x => x.claveFila);
                    table.ForeignKey(
                        name: "fk_fila_aspirante",
                        column: x => x.claveAspirante,
                        principalTable: "Aspirantes",
                        principalColumn: "claveAspirante",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_fila_convocatoria",
                        column: x => x.claveConvocatoria,
                        principalTable: "Convocatorias",
                        principalColumn: "claveConvocatoria",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetalleAdjuncion",
                columns: table => new
                {
                    claveAdjuncion = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveDocAspirante = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    estatus_documento = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    motivo_rechazo = table.Column<string>(type: "text", nullable: false),
                    fecha_evaluacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_DATE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleAdjuncion", x => new { x.claveAdjuncion, x.claveDocAspirante });
                    table.CheckConstraint("ck_estatus_doc_detalle", "estatus_documento IN ('Aceptado','Rechazado','Pendiente')");
                    table.ForeignKey(
                        name: "fk_detalle_adj",
                        column: x => x.claveAdjuncion,
                        principalTable: "Adjunciones",
                        principalColumn: "claveAdjuncion",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_detalle_doc",
                        column: x => x.claveDocAspirante,
                        principalTable: "DocumentosAspirante",
                        principalColumn: "claveDocAspirante",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdjuncionesOriginales",
                columns: table => new
                {
                    claveAdjOriginal = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveEntrega = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveUsuario = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    fecha_carga = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdjuncionesOriginales", x => x.claveAdjOriginal);
                });

            migrationBuilder.CreateTable(
                name: "DetalleAdjuncionOriginal",
                columns: table => new
                {
                    claveAdjOriginal = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveDocAspirante = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    ruta_pdf_original = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleAdjuncionOriginal", x => new { x.claveAdjOriginal, x.claveDocAspirante });
                    table.ForeignKey(
                        name: "fk_detalle_doc_aspirante",
                        column: x => x.claveDocAspirante,
                        principalTable: "DocumentosAspirante",
                        principalColumn: "claveDocAspirante",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_detalle_maestro",
                        column: x => x.claveAdjOriginal,
                        principalTable: "AdjuncionesOriginales",
                        principalColumn: "claveAdjOriginal",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Alumnos",
                columns: table => new
                {
                    claveAlumno = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    matricula = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    grado = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    grupo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    estado = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    claveExpediente = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alumnos", x => x.claveAlumno);
                });

            migrationBuilder.CreateTable(
                name: "AsignacionGrupo",
                columns: table => new
                {
                    claveAsignacion = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveAlumno = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveGrupo = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    fecha_asignacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ciclo_escolar = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsignacionGrupo", x => x.claveAsignacion);
                    table.ForeignKey(
                        name: "fk_asig_alumno",
                        column: x => x.claveAlumno,
                        principalTable: "Alumnos",
                        principalColumn: "claveAlumno",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_asig_grupo",
                        column: x => x.claveGrupo,
                        principalTable: "Grupos",
                        principalColumn: "claveGrupo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TutoresAlumnos",
                columns: table => new
                {
                    claveAlumno = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveTutor = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    fecha_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    fecha_baja = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TutoresAlumnos", x => new { x.claveAlumno, x.claveTutor });
                    table.ForeignKey(
                        name: "fk_rel_alumno",
                        column: x => x.claveAlumno,
                        principalTable: "Alumnos",
                        principalColumn: "claveAlumno",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rel_tutor",
                        column: x => x.claveTutor,
                        principalTable: "Tutores",
                        principalColumn: "claveTutor",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CitasInscripcion",
                columns: table => new
                {
                    claveCita = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    fecha_cita = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    hora_cita = table.Column<TimeSpan>(type: "interval", nullable: false),
                    estado_cita = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Programada"),
                    observaciones = table.Column<string>(type: "text", nullable: false),
                    create_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    claveRevision = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveTutorAspirante = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitasInscripcion", x => x.claveCita);
                    table.CheckConstraint("ck_estado_cita", "estado_cita IN ('Programada','Asistió','No Asistió')");
                    table.ForeignKey(
                        name: "fk_cita_tutor",
                        column: x => x.claveTutorAspirante,
                        principalTable: "TutorAspirante",
                        principalColumn: "claveTutorAspirante",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetalleRevision",
                columns: table => new
                {
                    claveRevision = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveDocAspirante = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    estatus_doc = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    motivo_rechazo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleRevision", x => new { x.claveRevision, x.claveDocAspirante });
                    table.CheckConstraint("ck_estatus_doc_revision", "estatus_doc IN ('Aceptado','Rechazado')");
                    table.ForeignKey(
                        name: "fk_detalle_documento",
                        column: x => x.claveDocAspirante,
                        principalTable: "DocumentosAspirante",
                        principalColumn: "claveDocAspirante",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documentos",
                columns: table => new
                {
                    claveDocumento = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    archivo_url = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    estado = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    fecha_subida = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    claveExpediente = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveTipoDocumento = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documentos", x => x.claveDocumento);
                    table.ForeignKey(
                        name: "fk_documentos_tipo",
                        column: x => x.claveTipoDocumento,
                        principalTable: "TipoDocumentos",
                        principalColumn: "claveTipoDocumento",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmpleadoRol",
                columns: table => new
                {
                    claveEmpleado = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveRol = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    fecha_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_DATE"),
                    fecha_fin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpleadoRol", x => new { x.claveEmpleado, x.claveRol, x.fecha_inicio });
                    table.ForeignKey(
                        name: "fk_rel_rol",
                        column: x => x.claveRol,
                        principalTable: "Roles",
                        principalColumn: "claveRol",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Empleados",
                columns: table => new
                {
                    claveEmpleado = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    fecha_contratacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tipo_contrato = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    estatus_laboral = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    telefono = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    claveExpediente = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleados", x => x.claveEmpleado);
                    table.CheckConstraint("ck_estatus_laboral", "estatus_laboral IN ('Activo','Baja')");
                    table.CheckConstraint("ck_tipo_contrato", "tipo_contrato IN ('Planta','Temporal')");
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    claveUsuario = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    nombre_usuario = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    contrasenia = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    correo_institucional = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    claveEmpleado = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.claveUsuario);
                    table.ForeignKey(
                        name: "fk_usuario_empleado",
                        column: x => x.claveEmpleado,
                        principalTable: "Empleados",
                        principalColumn: "claveEmpleado",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Entregas",
                columns: table => new
                {
                    claveEntrega = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    fecha_formalizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    estado_final = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    claveCita = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveTutorAspirante = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveUsuario = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    AdjuncionOriginalClaveAdjOriginal = table.Column<string>(type: "character varying(18)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entregas", x => x.claveEntrega);
                    table.ForeignKey(
                        name: "FK_Entregas_AdjuncionesOriginales_AdjuncionOriginalClaveAdjOri~",
                        column: x => x.AdjuncionOriginalClaveAdjOriginal,
                        principalTable: "AdjuncionesOriginales",
                        principalColumn: "claveAdjOriginal",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_entregas_cita",
                        column: x => x.claveCita,
                        principalTable: "CitasInscripcion",
                        principalColumn: "claveCita",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_entregas_tutor",
                        column: x => x.claveTutorAspirante,
                        principalTable: "TutorAspirante",
                        principalColumn: "claveTutorAspirante",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_entregas_usuario",
                        column: x => x.claveUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "claveUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Publicaciones",
                columns: table => new
                {
                    clavePublicacion = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    titulo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    subtitulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    cuerpo_contenido = table.Column<string>(type: "text", nullable: false),
                    categoria = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    fecha_aparicion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    fecha_retiro = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    claveUsuario = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveConvocatoria = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true),
                    claveImagen = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true),
                    fecha_registro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    destacado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    estatus_visible = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publicaciones", x => x.clavePublicacion);
                    table.CheckConstraint("ck_categoria_pub", "categoria IN ('Eventos Culturales','Noticia','Aviso','Convocatorias')");
                    table.ForeignKey(
                        name: "fk_convocatoria_publicada",
                        column: x => x.claveConvocatoria,
                        principalTable: "Convocatorias",
                        principalColumn: "claveConvocatoria");
                    table.ForeignKey(
                        name: "fk_imagen_galeria",
                        column: x => x.claveImagen,
                        principalTable: "GaleriaImagenes",
                        principalColumn: "claveImagen");
                    table.ForeignKey(
                        name: "fk_usuario_publicador",
                        column: x => x.claveUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "claveUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Receptores",
                columns: table => new
                {
                    claveReceptor = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    tipo_receptor = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    claveTutorAspirante = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true),
                    claveTutor = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true),
                    claveUsuario = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true),
                    correo_destino = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receptores", x => x.claveReceptor);
                    table.CheckConstraint("ck_tipo_receptor", "tipo_receptor IN ('TutorAspirante','Tutor','Usuario')");
                    table.ForeignKey(
                        name: "fk_rec_tutor",
                        column: x => x.claveTutor,
                        principalTable: "Tutores",
                        principalColumn: "claveTutor");
                    table.ForeignKey(
                        name: "fk_rec_tutor_asp",
                        column: x => x.claveTutorAspirante,
                        principalTable: "TutorAspirante",
                        principalColumn: "claveTutorAspirante");
                    table.ForeignKey(
                        name: "fk_rec_usuario",
                        column: x => x.claveUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "claveUsuario");
                });

            migrationBuilder.CreateTable(
                name: "Revisiones",
                columns: table => new
                {
                    claveRevision = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    estatus_revision = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    estado_operativo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Abierta"),
                    observacion_general = table.Column<string>(type: "text", nullable: false),
                    fecha_revision = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    claveAdjuncion = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveUsuario = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Revisiones", x => x.claveRevision);
                    table.CheckConstraint("ck_estado_operativo_rev", "estado_operativo IN ('Abierta','Cerrada')");
                    table.CheckConstraint("ck_estatus_revision", "estatus_revision IN ('Aceptada','Rechazada','Pendiente')");
                    table.ForeignKey(
                        name: "fk_rev_adjuncion",
                        column: x => x.claveAdjuncion,
                        principalTable: "Adjunciones",
                        principalColumn: "claveAdjuncion",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rev_usuario",
                        column: x => x.claveUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "claveUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Expedientes",
                columns: table => new
                {
                    claveExpediente = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    nombre = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    apellido_paterno = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    apellido_materno = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    curp = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    tipo_titular = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    claveEntrega = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expedientes", x => x.claveExpediente);
                    table.CheckConstraint("ck_tipo_titular", "tipo_titular IN ('Alumno','Empleado')");
                    table.ForeignKey(
                        name: "fk_expediente_entrega",
                        column: x => x.claveEntrega,
                        principalTable: "Entregas",
                        principalColumn: "claveEntrega");
                });

            migrationBuilder.CreateTable(
                name: "ValidacionDocumentos",
                columns: table => new
                {
                    claveEntrega = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveDocAspirante = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    estatus_cotejo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    fecha_validacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidacionDocumentos", x => new { x.claveEntrega, x.claveDocAspirante });
                    table.ForeignKey(
                        name: "fk_validacion_documento",
                        column: x => x.claveDocAspirante,
                        principalTable: "DocumentosAspirante",
                        principalColumn: "claveDocAspirante",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_validacion_entrega",
                        column: x => x.claveEntrega,
                        principalTable: "Entregas",
                        principalColumn: "claveEntrega",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notificaciones",
                columns: table => new
                {
                    claveNotificacion = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    titulo = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    mensaje = table.Column<string>(type: "text", nullable: false),
                    prioridad = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    datos = table.Column<string>(type: "jsonb", nullable: false),
                    visualizacion = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    claveTipoNotificacion = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveReceptor = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificaciones", x => x.claveNotificacion);
                    table.ForeignKey(
                        name: "fk_notif_receptor",
                        column: x => x.claveReceptor,
                        principalTable: "Receptores",
                        principalColumn: "claveReceptor",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_notif_tipo",
                        column: x => x.claveTipoNotificacion,
                        principalTable: "TipoNotificaciones",
                        principalColumn: "claveTipoNotificacion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RevisionesAceptadas",
                columns: table => new
                {
                    claveRevisionAceptada = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveRevision = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveReceptor = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveConvocatoria = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    fecha_aceptacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Estado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RevisionesAceptadas", x => x.claveRevisionAceptada);
                    table.ForeignKey(
                        name: "fk_convocatoria_buffer",
                        column: x => x.claveConvocatoria,
                        principalTable: "Convocatorias",
                        principalColumn: "claveConvocatoria",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_receptor_buffer",
                        column: x => x.claveReceptor,
                        principalTable: "Receptores",
                        principalColumn: "claveReceptor",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rev_buffer",
                        column: x => x.claveRevision,
                        principalTable: "Revisiones",
                        principalColumn: "claveRevision",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Envios",
                columns: table => new
                {
                    claveEnvio = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    claveNotificacion = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    destino = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    reintento_num = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    estatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Pendiente"),
                    confirmacion_lectura = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    fecha_envio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error_log = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Envios", x => x.claveEnvio);
                    table.CheckConstraint("ck_estatus_envio", "estatus IN ('Pendiente','Enviado','Fallido','En Proceso')");
                    table.ForeignKey(
                        name: "fk_envios_notificacion",
                        column: x => x.claveNotificacion,
                        principalTable: "Notificaciones",
                        principalColumn: "claveNotificacion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Adjunciones_claveAspirante",
                table: "Adjunciones",
                column: "claveAspirante");

            migrationBuilder.CreateIndex(
                name: "IX_Adjunciones_claveTutorAspirante",
                table: "Adjunciones",
                column: "claveTutorAspirante");

            migrationBuilder.CreateIndex(
                name: "IX_AdjuncionesOriginales_claveEntrega",
                table: "AdjuncionesOriginales",
                column: "claveEntrega",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdjuncionesOriginales_claveUsuario",
                table: "AdjuncionesOriginales",
                column: "claveUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Alumnos_claveExpediente",
                table: "Alumnos",
                column: "claveExpediente");

            migrationBuilder.CreateIndex(
                name: "IX_Alumnos_matricula",
                table: "Alumnos",
                column: "matricula",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionGrupo_claveAlumno",
                table: "AsignacionGrupo",
                column: "claveAlumno");

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionGrupo_claveGrupo",
                table: "AsignacionGrupo",
                column: "claveGrupo");

            migrationBuilder.CreateIndex(
                name: "IX_Aspirantes_claveConvocatoria",
                table: "Aspirantes",
                column: "claveConvocatoria");

            migrationBuilder.CreateIndex(
                name: "IX_Aspirantes_claveTutorAspirante",
                table: "Aspirantes",
                column: "claveTutorAspirante");

            migrationBuilder.CreateIndex(
                name: "IX_Aspirantes_curp",
                table: "Aspirantes",
                column: "curp",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CitasInscripcion_claveRevision",
                table: "CitasInscripcion",
                column: "claveRevision",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CitasInscripcion_claveTutorAspirante",
                table: "CitasInscripcion",
                column: "claveTutorAspirante",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DestinoNotificacion_claveTipoNotificacion_tipo_receptor",
                table: "DestinoNotificacion",
                columns: new[] { "claveTipoNotificacion", "tipo_receptor" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetalleAdjuncion_claveDocAspirante",
                table: "DetalleAdjuncion",
                column: "claveDocAspirante");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleAdjuncionOriginal_claveDocAspirante",
                table: "DetalleAdjuncionOriginal",
                column: "claveDocAspirante");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleRevision_claveDocAspirante",
                table: "DetalleRevision",
                column: "claveDocAspirante");

            migrationBuilder.CreateIndex(
                name: "IX_Direcciones_claveTutorAspirante",
                table: "Direcciones",
                column: "claveTutorAspirante");

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_claveExpediente",
                table: "Documentos",
                column: "claveExpediente");

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_claveTipoDocumento",
                table: "Documentos",
                column: "claveTipoDocumento");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosAspirante_claveAspirante",
                table: "DocumentosAspirante",
                column: "claveAspirante");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosAspirante_claveTipoDocumento",
                table: "DocumentosAspirante",
                column: "claveTipoDocumento");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadoRol_claveRol",
                table: "EmpleadoRol",
                column: "claveRol");

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_claveExpediente",
                table: "Empleados",
                column: "claveExpediente");

            migrationBuilder.CreateIndex(
                name: "IX_Entregas_AdjuncionOriginalClaveAdjOriginal",
                table: "Entregas",
                column: "AdjuncionOriginalClaveAdjOriginal");

            migrationBuilder.CreateIndex(
                name: "IX_Entregas_claveCita",
                table: "Entregas",
                column: "claveCita",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Entregas_claveTutorAspirante",
                table: "Entregas",
                column: "claveTutorAspirante");

            migrationBuilder.CreateIndex(
                name: "IX_Entregas_claveUsuario",
                table: "Entregas",
                column: "claveUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Envios_claveNotificacion",
                table: "Envios",
                column: "claveNotificacion");

            migrationBuilder.CreateIndex(
                name: "IX_Expedientes_claveEntrega",
                table: "Expedientes",
                column: "claveEntrega");

            migrationBuilder.CreateIndex(
                name: "IX_Expedientes_curp",
                table: "Expedientes",
                column: "curp",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FilaVirtual_claveAspirante",
                table: "FilaVirtual",
                column: "claveAspirante",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_lugar_convocatoria",
                table: "FilaVirtual",
                columns: new[] { "claveConvocatoria", "numero_lugar" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_claveReceptor",
                table: "Notificaciones",
                column: "claveReceptor");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_claveTipoNotificacion",
                table: "Notificaciones",
                column: "claveTipoNotificacion");

            migrationBuilder.CreateIndex(
                name: "IX_Publicaciones_claveConvocatoria",
                table: "Publicaciones",
                column: "claveConvocatoria");

            migrationBuilder.CreateIndex(
                name: "IX_Publicaciones_claveImagen",
                table: "Publicaciones",
                column: "claveImagen");

            migrationBuilder.CreateIndex(
                name: "IX_Publicaciones_claveUsuario",
                table: "Publicaciones",
                column: "claveUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Receptores_claveTutor",
                table: "Receptores",
                column: "claveTutor");

            migrationBuilder.CreateIndex(
                name: "IX_Receptores_claveTutorAspirante",
                table: "Receptores",
                column: "claveTutorAspirante");

            migrationBuilder.CreateIndex(
                name: "IX_Receptores_claveUsuario",
                table: "Receptores",
                column: "claveUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Requisitos_claveTipoDocumento",
                table: "Requisitos",
                column: "claveTipoDocumento");

            migrationBuilder.CreateIndex(
                name: "IX_Revisiones_claveAdjuncion",
                table: "Revisiones",
                column: "claveAdjuncion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Revisiones_claveUsuario",
                table: "Revisiones",
                column: "claveUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_RevisionesAceptadas_claveConvocatoria",
                table: "RevisionesAceptadas",
                column: "claveConvocatoria");

            migrationBuilder.CreateIndex(
                name: "IX_RevisionesAceptadas_claveReceptor",
                table: "RevisionesAceptadas",
                column: "claveReceptor");

            migrationBuilder.CreateIndex(
                name: "IX_RevisionesAceptadas_claveRevision",
                table: "RevisionesAceptadas",
                column: "claveRevision",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TipoNotificaciones_nombre_proceso",
                table: "TipoNotificaciones",
                column: "nombre_proceso",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TutorAspirante_curp_tutor",
                table: "TutorAspirante",
                column: "curp_tutor",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tutores_curp_tutor",
                table: "Tutores",
                column: "curp_tutor",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TutoresAlumnos_claveTutor",
                table: "TutoresAlumnos",
                column: "claveTutor");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_claveEmpleado",
                table: "Usuarios",
                column: "claveEmpleado",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_correo_institucional",
                table: "Usuarios",
                column: "correo_institucional",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_nombre_usuario",
                table: "Usuarios",
                column: "nombre_usuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ValidacionDocumentos_claveDocAspirante",
                table: "ValidacionDocumentos",
                column: "claveDocAspirante");

            migrationBuilder.AddForeignKey(
                name: "FK_AdjuncionesOriginales_Usuarios_claveUsuario",
                table: "AdjuncionesOriginales",
                column: "claveUsuario",
                principalTable: "Usuarios",
                principalColumn: "claveUsuario",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_adj_entrega",
                table: "AdjuncionesOriginales",
                column: "claveEntrega",
                principalTable: "Entregas",
                principalColumn: "claveEntrega",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_alumno_expediente",
                table: "Alumnos",
                column: "claveExpediente",
                principalTable: "Expedientes",
                principalColumn: "claveExpediente",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_cita_revision",
                table: "CitasInscripcion",
                column: "claveRevision",
                principalTable: "Revisiones",
                principalColumn: "claveRevision",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_detalle_revision",
                table: "DetalleRevision",
                column: "claveRevision",
                principalTable: "Revisiones",
                principalColumn: "claveRevision",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_documentos_expediente",
                table: "Documentos",
                column: "claveExpediente",
                principalTable: "Expedientes",
                principalColumn: "claveExpediente",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_rel_empleado",
                table: "EmpleadoRol",
                column: "claveEmpleado",
                principalTable: "Empleados",
                principalColumn: "claveEmpleado",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_emp_expediente",
                table: "Empleados",
                column: "claveExpediente",
                principalTable: "Expedientes",
                principalColumn: "claveExpediente",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_adj_aspirante",
                table: "Adjunciones");

            migrationBuilder.DropForeignKey(
                name: "fk_adj_tutor",
                table: "Adjunciones");

            migrationBuilder.DropForeignKey(
                name: "fk_cita_tutor",
                table: "CitasInscripcion");

            migrationBuilder.DropForeignKey(
                name: "fk_entregas_tutor",
                table: "Entregas");

            migrationBuilder.DropForeignKey(
                name: "FK_AdjuncionesOriginales_Usuarios_claveUsuario",
                table: "AdjuncionesOriginales");

            migrationBuilder.DropForeignKey(
                name: "fk_entregas_usuario",
                table: "Entregas");

            migrationBuilder.DropForeignKey(
                name: "fk_rev_usuario",
                table: "Revisiones");

            migrationBuilder.DropForeignKey(
                name: "fk_adj_entrega",
                table: "AdjuncionesOriginales");

            migrationBuilder.DropTable(
                name: "AsignacionGrupo");

            migrationBuilder.DropTable(
                name: "DestinoNotificacion");

            migrationBuilder.DropTable(
                name: "DetalleAdjuncion");

            migrationBuilder.DropTable(
                name: "DetalleAdjuncionOriginal");

            migrationBuilder.DropTable(
                name: "DetalleRevision");

            migrationBuilder.DropTable(
                name: "Direcciones");

            migrationBuilder.DropTable(
                name: "Documentos");

            migrationBuilder.DropTable(
                name: "EmpleadoRol");

            migrationBuilder.DropTable(
                name: "Envios");

            migrationBuilder.DropTable(
                name: "FilaVirtual");

            migrationBuilder.DropTable(
                name: "Publicaciones");

            migrationBuilder.DropTable(
                name: "Requisitos");

            migrationBuilder.DropTable(
                name: "RevisionesAceptadas");

            migrationBuilder.DropTable(
                name: "TutoresAlumnos");

            migrationBuilder.DropTable(
                name: "ValidacionDocumentos");

            migrationBuilder.DropTable(
                name: "Grupos");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Notificaciones");

            migrationBuilder.DropTable(
                name: "GaleriaImagenes");

            migrationBuilder.DropTable(
                name: "Alumnos");

            migrationBuilder.DropTable(
                name: "DocumentosAspirante");

            migrationBuilder.DropTable(
                name: "Receptores");

            migrationBuilder.DropTable(
                name: "TipoNotificaciones");

            migrationBuilder.DropTable(
                name: "TipoDocumentos");

            migrationBuilder.DropTable(
                name: "Tutores");

            migrationBuilder.DropTable(
                name: "Aspirantes");

            migrationBuilder.DropTable(
                name: "Convocatorias");

            migrationBuilder.DropTable(
                name: "TutorAspirante");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Empleados");

            migrationBuilder.DropTable(
                name: "Expedientes");

            migrationBuilder.DropTable(
                name: "Entregas");

            migrationBuilder.DropTable(
                name: "AdjuncionesOriginales");

            migrationBuilder.DropTable(
                name: "CitasInscripcion");

            migrationBuilder.DropTable(
                name: "Revisiones");

            migrationBuilder.DropTable(
                name: "Adjunciones");
        }
    }
}
