using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Telesecundaria.Models;

namespace Telesecundaria.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // DbSets
        public DbSet<GaleriaImagenesEntity> GaleriaImagenes { get; set; }
        public DbSet<TipoDocumentosEntity> TipoDocumentos { get; set; }
        public DbSet<TipoNotificacionesEntity> TipoNotificaciones { get; set; }
        public DbSet<ConvocatoriasEntity> Convocatorias { get; set; }
        public DbSet<RolesEntity> Roles { get; set; }
        public DbSet<TutorAspiranteEntity> TutorAspirante { get; set; }
        public DbSet<TutoresEntity> Tutores { get; set; }
        public DbSet<AspirantesEntity> Aspirantes { get; set; }
        public DbSet<ReceptoresEntity> Receptores { get; set; }
        public DbSet<DireccionesEntity> Direcciones { get; set; }
        public DbSet<PublicacionesEntity> Publicaciones { get; set; }
        public DbSet<FilaVirtualEntity> FilaVirtual { get; set; }
        public DbSet<DocumentosAspiranteEntity> DocumentosAspirante { get; set; }
        public DbSet<AdjuncionesEntity> Adjunciones { get; set; }
        public DbSet<RequisitosEntity> Requisitos { get; set; }
        public DbSet<DestinoNotificacionEntity> DestinoNotificacion { get; set; }
        public DbSet<NotificacionesEntity> Notificaciones { get; set; }
        public DbSet<DetalleAdjuncionEntity> DetalleAdjuncion { get; set; }
        public DbSet<RevisionesEntity> Revisiones { get; set; }
        public DbSet<CitasInscripcionEntity> CitasInscripcion { get; set; }
        public DbSet<EnviosEntity> Envios { get; set; }
        public DbSet<RevisionesAceptadasEntity> RevisionesAceptadas { get; set; }
        public DbSet<DetalleRevisionEntity> DetalleRevision { get; set; }
        public DbSet<EntregasEntity> Entregas { get; set; }
        public DbSet<ExpedientesEntity> Expedientes { get; set; }
        public DbSet<AlumnosEntity> Alumnos { get; set; }
        public DbSet<TutoresAlumnosEntity> TutoresAlumnos { get; set; }
        public DbSet<GruposEntity> Grupos { get; set; }
        public DbSet<AsignacionGrupoEntity> AsignacionGrupo { get; set; }
        public DbSet<ValidacionDocumentosEntity> ValidacionDocumentos { get; set; }
        public DbSet<AdjuncionesOriginalesEntity> AdjuncionesOriginales { get; set; }
        public DbSet<DetalleAdjuncionOriginalEntity> DetalleAdjuncionOriginal { get; set; }
        public DbSet<DocumentosEntity> Documentos { get; set; }
        public DbSet<EmpleadosEntity> Empleados { get; set; }
        public DbSet<UsuariosEntity> Usuarios { get; set; }
        public DbSet<EmpleadoRolEntity> EmpleadoRol { get; set; }

        // DbSets nuevos
        public DbSet<ModulosEntity> Modulos { get; set; }
        public DbSet<PermisosEntity> Permisos { get; set; }
        public DbSet<LogueosEntity> Logueos { get; set; }
        public DbSet<TokenConvocatoriasEntity> TokenConvocatorias { get; set; }
        public DbSet<CargasDocumentosEntity> CargasDocumentos { get; set; }
        public DbSet<DetalleCargaEntity> DetalleCarga { get; set; }

        // DbSets nuevos
        public DbSet<CiclosEscolaresEntity> CiclosEscolares { get; set; }
        public DbSet<PeriodosEntity> Periodos { get; set; }
        public DbSet<PagosEntity> Pagos { get; set; }
        public DbSet<InscripcionesEntity> Inscripciones { get; set; }

        public DbSet<RutaRechazadaEntity> RutasRechazadas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // GaleriaImagenes
            modelBuilder.Entity<GaleriaImagenesEntity>(entity =>
            {
                entity.ToTable("GaleriaImagenes");
                entity.HasKey(e => e.ClaveImagen);

                entity.Property(e => e.ClaveImagen)
                      .HasColumnName("claveImagen")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_imagen()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.NombreArchivo)
                      .HasColumnName("nombre_archivo")
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.RutaUrl)
                      .HasColumnName("ruta_url")
                      .IsRequired();

                entity.Property(e => e.TipoRecurso)
                      .HasColumnName("tipo_recurso")
                      .HasMaxLength(25)
                      .IsRequired();

                entity.Property(e => e.FechaRegistro)
                      .HasColumnName("fecha_registro")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasCheckConstraint("ck_tipo_recurso_galeria",
                    "tipo_recurso IN ('Eventos Culturales', 'Noticia', 'Aviso', 'Convocatorias', 'Galería', 'otros')");
            });

            // TipoDocumentos
            modelBuilder.Entity<TipoDocumentosEntity>(entity =>
            {
                entity.ToTable("TipoDocumentos");
                entity.HasKey(e => e.ClaveTipoDocumento);

                entity.Property(e => e.ClaveTipoDocumento)
                      .HasColumnName("claveTipoDocumento")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_tipo_doc()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.NombreDocumento)
                      .HasColumnName("nombre_documento")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.Area)
                      .HasColumnName("area")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.Descripcion)
                      .HasColumnName("descripcion");

                entity.Property(e => e.Estado)
                      .HasColumnName("estado")
                      .HasDefaultValue(true)
                      .IsRequired();

                entity.HasCheckConstraint("ck_area_tipo_doc",
                    "area IN ('Preinscripción','Inscripción','Becas','Egreso','Laboral','Institucional')");
            });

            // TipoNotificaciones
            modelBuilder.Entity<TipoNotificacionesEntity>(entity =>
            {
                entity.ToTable("TipoNotificaciones");
                entity.HasKey(e => e.ClaveTipoNotificacion);

                entity.Property(e => e.ClaveTipoNotificacion)
                      .HasColumnName("claveTipoNotificacion")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_tipo_notificacion()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.NombreProceso)
                      .HasColumnName("nombre_proceso")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.HasIndex(e => e.NombreProceso).IsUnique();

                entity.Property(e => e.Descripcion)
                      .HasColumnName("descripcion")
                      .IsRequired();

                entity.Property(e => e.Icono)
                      .HasColumnName("icono")
                      .HasMaxLength(40)
                      .IsRequired();

                entity.Property(e => e.Color)
                      .HasColumnName("color")
                      .HasMaxLength(9)
                      .IsRequired();

                entity.HasCheckConstraint("ck_nombre_proceso",
                    "nombre_proceso IN ('Documentos Rechazados','Documentos Aceptados','Cierre de Adjuncion','Citas', 'Inscripciones', 'Institucionales','Docencia','Directivas','Administrativas')");
            });

            // Convocatorias
            modelBuilder.Entity<ConvocatoriasEntity>(entity =>
            {
                entity.ToTable("Convocatorias");
                entity.HasKey(e => e.ClaveConvocatoria);

                entity.Property(e => e.ClaveConvocatoria)
                      .HasColumnName("claveConvocatoria")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_convocatoria()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.Titulo)
                      .HasColumnName("titulo")
                      .HasMaxLength(150)
                      .IsRequired();

                entity.Property(e => e.Descripcion)
                      .HasColumnName("descripcion");

                entity.Property(e => e.FechaInicio)
                      .HasColumnName("fecha_inicio")
                      .IsRequired();

                entity.Property(e => e.FechaFin)
                      .HasColumnName("fecha_fin")
                      .IsRequired();

                entity.Property(e => e.CicloEscolar)
                      .HasColumnName("ciclo_escolar")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.CupoMaximo)
                      .HasColumnName("cupo_maximo");

                entity.Property(e => e.CupoDisponible)
                      .HasColumnName("cupo_disponible");

                entity.Property(e => e.Activacion)
                      .HasColumnName("activacion")
                      .HasDefaultValue(false);

                entity.Property(e => e.Estado)
                      .HasColumnName("estado")
                      .HasMaxLength(15);

                entity.Property(e => e.FechaRegistro)
                      .HasColumnName("fecha_registro")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Roles
            modelBuilder.Entity<RolesEntity>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(e => e.ClaveRol);

                entity.Property(e => e.ClaveRol)
                      .HasColumnName("claveRol")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_Rol()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.NombreRol)
                      .HasColumnName("nombre_rol")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.Descripcion)
                      .HasColumnName("descripcion")
                      .HasMaxLength(100)
                      .IsRequired();

                entity.HasCheckConstraint("ck_nombre_rol",
                    "nombre_rol IN ('Directivo','Administrativo','Docente','Intendente')");

                entity.HasMany(r => r.Permisos)
                      .WithOne(p => p.Rol)
                      .HasForeignKey(p => p.ClaveRol)
                      .HasConstraintName("fk_permiso_rol");
            });

            // TutorAspirante
            modelBuilder.Entity<TutorAspiranteEntity>(entity =>
            {
                entity.ToTable("TutorAspirante");
                entity.HasKey(e => e.ClaveTutorAspirante);

                entity.Property(e => e.ClaveTutorAspirante)
                      .HasColumnName("claveTutorAspirante")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_tutor_aspirante()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.Nombre)
                      .HasColumnName("nombre")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.ApellidoPaterno)
                      .HasColumnName("apellido_paterno")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.ApellidoMaterno)
                      .HasColumnName("apellido_materno")
                      .HasMaxLength(50);

                entity.Property(e => e.CurpTutor)
                      .HasColumnName("curp_tutor")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.HasIndex(e => e.CurpTutor).IsUnique();

                entity.Property(e => e.Telefono)
                      .HasColumnName("telefono")
                      .HasMaxLength(15)
                      .IsRequired();

                entity.Property(e => e.Correo)
                      .HasColumnName("correo")
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.Parentesco)
                      .HasColumnName("parentesco")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.Estado)
                      .HasColumnName("estado")
                      .HasDefaultValue(true);

                entity.Property(e => e.Contrasena)
                      .HasColumnName("contrasena")
                      .HasMaxLength(255)
                      .HasDefaultValue("Temporal123")
                      .IsRequired();
            });

            // Tutores
            modelBuilder.Entity<TutoresEntity>(entity =>
            {
                entity.ToTable("Tutores");
                entity.HasKey(e => e.ClaveTutor);

                entity.Property(e => e.ClaveTutor)
                      .HasColumnName("claveTutor")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_tutor()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.Nombre)
                      .HasColumnName("nombre")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.ApellidoPaterno)
                      .HasColumnName("apellido_paterno")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.ApellidoMaterno)
                      .HasColumnName("apellido_materno")
                      .HasMaxLength(50);

                entity.Property(e => e.CurpTutor)
                      .HasColumnName("curp_tutor")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.HasIndex(e => e.CurpTutor).IsUnique();

                entity.Property(e => e.Telefono)
                      .HasColumnName("telefono")
                      .HasMaxLength(15)
                      .IsRequired();

                entity.Property(e => e.Correo)
                      .HasColumnName("correo")
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.Parentesco)
                      .HasColumnName("parentesco")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.Estado)
                      .HasColumnName("estado")
                      .HasDefaultValue(true);
            });

            // Aspirantes
            modelBuilder.Entity<AspirantesEntity>(entity =>
            {
                entity.ToTable("Aspirantes");
                entity.HasKey(e => e.ClaveAspirante);

                entity.Property(e => e.ClaveAspirante)
                      .HasColumnName("claveAspirante")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_aspirante()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.Nombre)
                      .HasColumnName("nombre")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.ApellidoPaterno)
                      .HasColumnName("apellido_paterno")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.ApellidoMaterno)
                      .HasColumnName("apellido_materno")
                      .HasMaxLength(50);

                entity.Property(e => e.Curp)
                      .HasColumnName("curp")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.HasIndex(e => e.Curp).IsUnique();

                entity.Property(e => e.EscuelaProcedencia)
                      .HasColumnName("escuela_procedencia")
                      .HasMaxLength(150)
                      .IsRequired();

                entity.Property(e => e.PromedioPrimaria)
                      .HasColumnName("promedio_primaria")
                      .HasPrecision(3, 1)
                      .IsRequired();

                entity.Property(e => e.TieneDiscapacidad)
                      .HasColumnName("tiene_discapacidad")
                      .HasDefaultValue(false)
                      .IsRequired();

                entity.Property(e => e.NombreEnfermedad)
                      .HasColumnName("nombre_enfermedad")
                      .HasMaxLength(100);

                entity.Property(e => e.HermanoPlantel)
                      .HasColumnName("Hermano_Plantel")
                      .HasDefaultValue(false)
                      .IsRequired();

                entity.Property(e => e.CurpHermano)
                      .HasColumnName("curp_hermano")
                      .HasMaxLength(18);

                entity.Property(e => e.EstatusAspirante)
                      .HasColumnName("estatus_aspirante")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.HasCheckConstraint("ck_estatus_aspirante",
                    "estatus_aspirante IN ('En proceso','Aceptado','Rechazado')");

                entity.Property(e => e.FechaRegistro)
                      .HasColumnName("fecha_registro")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP")
                      .IsRequired();

                entity.Property(e => e.Estado)
                      .HasColumnName("estado")
                      .HasDefaultValue(true);

                entity.Property(e => e.ClaveConvocatoria)
                      .HasColumnName("claveConvocatoria")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.ClaveTutorAspirante)
                      .HasColumnName("claveTutorAspirante")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.HasOne(e => e.Convocatoria)
                      .WithMany(c => c.Aspirantes)
                      .HasForeignKey(e => e.ClaveConvocatoria)
                      .HasConstraintName("fk_aspirante_convocatoria");

                entity.HasOne(e => e.TutorAspirante)
                      .WithMany(t => t.Aspirantes)
                      .HasForeignKey(e => e.ClaveTutorAspirante)
                      .HasConstraintName("fk_aspirante_tutor");
            });

            // Direcciones
            modelBuilder.Entity<DireccionesEntity>(entity =>
            {
                entity.ToTable("Direcciones");
                entity.HasKey(e => e.ClaveDireccion);

                entity.Property(e => e.ClaveDireccion)
                      .HasColumnName("claveDireccion")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_direccion()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.CalleNumero)
                      .HasColumnName("calle_numero")
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.Colonia)
                      .HasColumnName("colonia")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.CodigoPostal)
                      .HasColumnName("codigo_postal")
                      .HasMaxLength(5)
                      .IsRequired();

                entity.Property(e => e.Municipio)
                      .HasColumnName("municipio")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.EstadoVerificacion)
                      .HasColumnName("estado_verificacion")
                      .HasDefaultValue(true);

                entity.Property(e => e.ClaveTutorAspirante)
                      .HasColumnName("claveTutorAspirante")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.HasOne(e => e.TutorAspirante)
                      .WithMany(t => t.Direcciones)
                      .HasForeignKey(e => e.ClaveTutorAspirante)
                      .HasConstraintName("fk_direcciones_tutor");
            });

            // FilaVirtual
            modelBuilder.Entity<FilaVirtualEntity>(entity =>
            {
                entity.ToTable("FilaVirtual");
                entity.HasKey(e => e.ClaveFila);

                entity.Property(e => e.ClaveFila)
                      .HasColumnName("claveFila")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_lugar_fila_virtual()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.ClaveConvocatoria)
                      .HasColumnName("claveConvocatoria")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.ClaveAspirante)
                      .HasColumnName("claveAspirante")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.HasIndex(e => e.ClaveAspirante).IsUnique();

                entity.Property(e => e.NumeroLugar)
                      .HasColumnName("numero_lugar")
                      .IsRequired();

                entity.Property(e => e.FechaAsignacion)
                      .HasColumnName("fecha_asignacion")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => new { e.ClaveConvocatoria, e.NumeroLugar })
                      .IsUnique()
                      .HasDatabaseName("uq_lugar_convocatoria");

                entity.HasOne(e => e.Convocatoria)
                      .WithMany(c => c.FilaVirtual)
                      .HasForeignKey(e => e.ClaveConvocatoria)
                      .HasConstraintName("fk_fila_convocatoria");

                entity.HasOne(e => e.Aspirante)
                      .WithOne(a => a.FilaVirtual)
                      .HasForeignKey<FilaVirtualEntity>(e => e.ClaveAspirante)
                      .HasConstraintName("fk_fila_aspirante");
            });

            // Receptores
            modelBuilder.Entity<ReceptoresEntity>(entity =>
            {
                entity.ToTable("Receptores");
                entity.HasKey(e => e.ClaveReceptor);

                entity.Property(e => e.ClaveReceptor)
                      .HasColumnName("claveReceptor")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_receptor()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.TipoReceptor)
                      .HasColumnName("tipo_receptor")
                      .HasMaxLength(80)
                      .IsRequired();

                entity.HasCheckConstraint("ck_tipo_receptor",
                    "tipo_receptor IN ('TutorAspirante','Tutor','Usuario')");

                entity.Property(e => e.ClaveTutorAspirante)
                      .HasColumnName("claveTutorAspirante")
                      .HasMaxLength(18);

                entity.Property(e => e.ClaveTutor)
                      .HasColumnName("claveTutor")
                      .HasMaxLength(18);

                entity.Property(e => e.ClaveUsuario)
                      .HasColumnName("claveUsuario")
                      .HasMaxLength(18);

                entity.Property(e => e.CorreoDestino)
                      .HasColumnName("correo_destino")
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.Estado)
                      .HasColumnName("estado")
                      .HasDefaultValue(true)
                      .IsRequired();

                entity.HasOne(e => e.TutorAspirante)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveTutorAspirante)
                      .HasConstraintName("fk_rec_tutor_asp")
                      .IsRequired(false);

                entity.HasOne(e => e.Tutor)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveTutor)
                      .HasConstraintName("fk_rec_tutor")
                      .IsRequired(false);
            });

            // Publicaciones
            modelBuilder.Entity<PublicacionesEntity>(entity =>
            {
                entity.ToTable("Publicaciones");
                entity.HasKey(e => e.ClavePublicacion);

                entity.Property(e => e.ClavePublicacion)
                      .HasColumnName("clavePublicacion")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_publicacion()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.Titulo)
                      .HasColumnName("titulo")
                      .HasMaxLength(150)
                      .IsRequired();

                entity.Property(e => e.Subtitulo)
                      .HasColumnName("subtitulo")
                      .HasMaxLength(200);

                entity.Property(e => e.CuerpoContenido)
                      .HasColumnName("cuerpo_contenido")
                      .IsRequired();

                entity.Property(e => e.Categoria)
                      .HasColumnName("categoria")
                      .HasMaxLength(25)
                      .IsRequired();

                entity.HasCheckConstraint("ck_categoria_pub",
                    "categoria IN ('Eventos Culturales','Noticia','Aviso','Convocatorias','Galería')");

                entity.Property(e => e.FechaAparicion)
                      .HasColumnName("fecha_aparicion")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaRetiro)
                      .HasColumnName("fecha_retiro");

                entity.Property(e => e.ClaveUsuario)
                      .HasColumnName("claveUsuario")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.ClaveConvocatoria)
                      .HasColumnName("claveConvocatoria")
                      .HasMaxLength(18);

                entity.Property(e => e.ClaveImagenPrincipal)
                      .HasColumnName("claveImagenPrincipal")
                      .HasMaxLength(18);

                entity.Property(e => e.ClaveImagenSecundaria)
                      .HasColumnName("claveImagenSecundaria")
                      .HasMaxLength(18);

                entity.Property(e => e.ClaveImagenTercera)
                      .HasColumnName("claveImagenTercera")
                      .HasMaxLength(18);

                entity.Property(e => e.FechaRegistro)
                      .HasColumnName("fecha_registro")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Destacado)
                      .HasColumnName("destacado")
                      .HasDefaultValue(false);

                entity.Property(e => e.EstatusVisible)
                      .HasColumnName("estatus_visible")
                      .HasDefaultValue(true);

                entity.HasOne(e => e.Convocatoria)
                      .WithMany(c => c.Publicaciones)
                      .HasForeignKey(e => e.ClaveConvocatoria)
                      .HasConstraintName("fk_convocatoria_publicada")
                      .IsRequired(false);

                entity.HasOne(e => e.ImagenPrincipal)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveImagenPrincipal)
                      .HasConstraintName("fk_imagen_principal")
                      .IsRequired(false);

                entity.HasOne(e => e.ImagenSecundaria)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveImagenSecundaria)
                      .HasConstraintName("fk_imagen_secundaria")
                      .IsRequired(false);

                entity.HasOne(e => e.ImagenTercera)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveImagenTercera)
                      .HasConstraintName("fk_imagen_tercera")
                      .IsRequired(false);
            });

            // DocumentosAspirante
            modelBuilder.Entity<DocumentosAspiranteEntity>(entity =>
            {
                entity.ToTable("DocumentosAspirante");
                entity.HasKey(e => e.ClaveDocAspirante);

                entity.Property(e => e.ClaveDocAspirante)
                      .HasColumnName("claveDocAspirante")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_doc_aspirante()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.FolioDocumento)
                      .HasColumnName("folio_documento")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.ValorAnalitico)
                      .HasColumnName("valor_analitico")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.HasCheckConstraint("ck_valor_analitico",
                    "valor_analitico IN ('Copia Digital','Original')");

                entity.Property(e => e.RutaArchivo)
                      .HasColumnName("ruta_archivo")
                      .HasMaxLength(255)
                      .IsRequired();

                entity.Property(e => e.ClaveAspirante)
                      .HasColumnName("claveAspirante")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.ClaveTipoDocumento)
                      .HasColumnName("claveTipoDocumento")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.HasOne(e => e.Aspirante)
                      .WithMany(a => a.DocumentosAspirante)
                      .HasForeignKey(e => e.ClaveAspirante)
                      .HasConstraintName("fk_doc_aspirante");

                entity.HasOne(e => e.TipoDocumento)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveTipoDocumento)
                      .HasConstraintName("fk_doc_tipo");
            });

            // Adjunciones
            modelBuilder.Entity<AdjuncionesEntity>(entity =>
            {
                entity.ToTable("Adjunciones");
                entity.HasKey(e => e.ClaveAdjuncion);

                entity.Property(e => e.ClaveAdjuncion)
                      .HasColumnName("claveAdjuncion")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_adjuncion()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.FechaEnvio)
                      .HasColumnName("fecha_envio")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.EstatusGral)
                      .HasColumnName("estatus_gral")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.HasCheckConstraint("ck_estatus_gral",
                    "estatus_gral IN ('Pendiente','Aceptada','Rechazada')");

                entity.Property(e => e.EstatusOperativo)
                      .HasColumnName("estatus_operativo")
                      .HasMaxLength(20)
                      .HasDefaultValue("Abierta")
                      .IsRequired();

                entity.HasCheckConstraint("ck_estatus_operativo_adj",
                    "estatus_operativo IN ('Abierta','Cerrada')");

                entity.Property(e => e.ClaveTutorAspirante)
                      .HasColumnName("claveTutorAspirante")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.ClaveAspirante)
                      .HasColumnName("claveAspirante")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.HasOne(e => e.TutorAspirante)
                      .WithMany(t => t.Adjunciones)
                      .HasForeignKey(e => e.ClaveTutorAspirante)
                      .HasConstraintName("fk_adj_tutor");

                entity.HasOne(e => e.Aspirante)
                      .WithMany(a => a.Adjunciones)
                      .HasForeignKey(e => e.ClaveAspirante)
                      .HasConstraintName("fk_adj_aspirante");
            });

            // Requisitos
            modelBuilder.Entity<RequisitosEntity>(entity =>
            {
                entity.ToTable("Requisitos");
                entity.HasKey(e => e.ClaveRequisito);

                entity.Property(e => e.ClaveRequisito)
                      .HasColumnName("claveRequisito")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_Requisito()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.EtapaProceso)
                      .HasColumnName("etapa_proceso")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.HasCheckConstraint("ck_etapa_proceso",
                    "etapa_proceso IN ('Preinscripción','Inscripción','Becas')");

                entity.Property(e => e.EstadoRequisito)
                      .HasColumnName("estado_requisito")
                      .HasDefaultValue(true)
                      .IsRequired();

                entity.Property(e => e.FormatoExigido)
                      .HasColumnName("formato_exigido")
                      .HasMaxLength(20);

                entity.Property(e => e.ClaveTipoDocumento)
                      .HasColumnName("claveTipoDocumento")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.HasOne(e => e.TipoDocumento)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveTipoDocumento)
                      .HasConstraintName("fk_tipo_doc");
            });

            // DestinoNotificacion
            modelBuilder.Entity<DestinoNotificacionEntity>(entity =>
            {
                entity.ToTable("DestinoNotificacion");
                entity.HasKey(e => e.ClaveDestino);

                entity.Property(e => e.ClaveDestino)
                      .HasColumnName("claveDestino")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_destino()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.ClaveTipoNotificacion)
                      .HasColumnName("claveTipoNotificacion")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.TipoReceptor)
                      .HasColumnName("tipo_receptor")
                      .HasMaxLength(80)
                      .IsRequired();

                entity.HasCheckConstraint("ck_tipo_receptor_destino",
                    "tipo_receptor IN ('TutorAspirante','Tutor','Usuario')");

                entity.HasIndex(e => new { e.ClaveTipoNotificacion, e.TipoReceptor }).IsUnique();

                entity.HasOne(e => e.TipoNotificacion)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveTipoNotificacion)
                      .HasConstraintName("fk_tipo_notif");
            });

            // Notificaciones
            modelBuilder.Entity<NotificacionesEntity>(entity =>
            {
                entity.ToTable("Notificaciones");
                entity.HasKey(e => e.ClaveNotificacion);

                entity.Property(e => e.ClaveNotificacion)
                      .HasColumnName("claveNotificacion")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_notificacion()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.Titulo)
                      .HasColumnName("titulo")
                      .HasMaxLength(80)
                      .IsRequired();

                entity.Property(e => e.Mensaje)
                      .HasColumnName("mensaje")
                      .IsRequired();

                entity.Property(e => e.Prioridad)
                      .HasColumnName("prioridad")
                      .HasDefaultValue((short)1);

                entity.Property(e => e.Datos)
                      .HasColumnName("datos")
                      .HasColumnType("jsonb");

                entity.Property(e => e.Visualizacion)
                      .HasColumnName("visualizacion")
                      .HasDefaultValue(false);

                entity.Property(e => e.FechaCreacion)
                      .HasColumnName("fecha_creacion")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.ClaveTipoNotificacion)
                      .HasColumnName("claveTipoNotificacion")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.ClaveReceptor)
                      .HasColumnName("claveReceptor")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.HasOne(e => e.TipoNotificacion)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveTipoNotificacion)
                      .HasConstraintName("fk_notif_tipo");

                entity.HasOne(e => e.Receptor)
                      .WithMany(r => r.Notificaciones)
                      .HasForeignKey(e => e.ClaveReceptor)
                      .HasConstraintName("fk_notif_receptor");
            });


            // DetalleAdjuncion
            modelBuilder.Entity<DetalleAdjuncionEntity>(entity =>
            {
                entity.ToTable("DetalleAdjuncion");
                entity.HasKey(e => new { e.ClaveAdjuncion, e.ClaveDocAspirante });

                entity.Property(e => e.ClaveAdjuncion)
                      .HasColumnName("claveAdjuncion")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.ClaveDocAspirante)
                      .HasColumnName("claveDocAspirante")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.EstatusDocumento)
                      .HasColumnName("estatus_documento")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.HasCheckConstraint("ck_estatus_doc_detalle",
                    "estatus_documento IN ('Aceptado','Rechazado','Pendiente')");

                entity.Property(e => e.MotivoRechazo)
                      .HasColumnName("motivo_rechazo");

                entity.Property(e => e.FechaEvaluacion)
                      .HasColumnName("fecha_evaluacion")
                      .HasDefaultValueSql("CURRENT_DATE");

                entity.HasOne(e => e.Adjuncion)
                      .WithMany(a => a.DetalleAdjuncion)
                      .HasForeignKey(e => e.ClaveAdjuncion)
                      .HasConstraintName("fk_detalle_adj");

                entity.HasOne(e => e.DocumentoAspirante)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveDocAspirante)
                      .HasConstraintName("fk_detalle_doc");
            });

            // Revisiones
            modelBuilder.Entity<RevisionesEntity>(entity =>
            {
                entity.ToTable("Revisiones");
                entity.HasKey(e => e.ClaveRevision);

                entity.Property(e => e.ClaveRevision)
                      .HasColumnName("claveRevision")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_revision()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.EstatusRevision)
                      .HasColumnName("estatus_revision")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.HasCheckConstraint("ck_estatus_revision",
                    "estatus_revision IN ('Aceptada','Rechazada','Pendiente')");

                entity.Property(e => e.EstadoOperativo)
                      .HasColumnName("estado_operativo")
                      .HasMaxLength(20)
                      .HasDefaultValue("Abierta")
                      .IsRequired();

                entity.HasCheckConstraint("ck_estado_operativo_rev",
                    "estado_operativo IN ('Abierta','Cerrada')");

                entity.Property(e => e.ObservacionGeneral)
                      .HasColumnName("observacion_general");

                entity.Property(e => e.FechaRevision)
                      .HasColumnName("fecha_revision")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.ClaveAdjuncion)
                      .HasColumnName("claveAdjuncion")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.HasIndex(e => e.ClaveAdjuncion).IsUnique();

                entity.Property(e => e.ClaveUsuario)
                      .HasColumnName("claveUsuario")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.HasOne(e => e.Adjuncion)
                      .WithOne(a => a.Revision)
                      .HasForeignKey<RevisionesEntity>(e => e.ClaveAdjuncion)
                      .HasConstraintName("fk_rev_adjuncion");
            });

            // Envios
            modelBuilder.Entity<EnviosEntity>(entity =>
            {
                entity.ToTable("Envios");
                entity.HasKey(e => e.ClaveEnvio);

                entity.Property(e => e.ClaveEnvio)
                      .HasColumnName("claveEnvio")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_envio()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.ClaveNotificacion)
                      .HasColumnName("claveNotificacion")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.Destino)
                      .HasColumnName("destino")
                      .HasMaxLength(150)
                      .IsRequired();

                entity.Property(e => e.ReintentoNum)
                      .HasColumnName("reintento_num")
                      .HasDefaultValue(0);

                entity.Property(e => e.Estatus)
                      .HasColumnName("estatus")
                      .HasMaxLength(20)
                      .HasDefaultValue("Pendiente");

                entity.HasCheckConstraint("ck_estatus_envio",
                    "estatus IN ('Pendiente','Enviado','Fallido','En Proceso')");

                entity.Property(e => e.ConfirmacionLectura)
                      .HasColumnName("confirmacion_lectura")
                      .HasDefaultValue(false);

                entity.Property(e => e.FechaEnvio)
                      .HasColumnName("fecha_envio");

                entity.Property(e => e.ErrorLog)
                      .HasColumnName("error_log")
                      .IsRequired(false);

                entity.HasOne(e => e.Notificacion)
                      .WithMany(n => n.Envios)
                      .HasForeignKey(e => e.ClaveNotificacion)
                      .HasConstraintName("fk_envios_notificacion");
            });

            // RevisionesAceptadas
            modelBuilder.Entity<RevisionesAceptadasEntity>(entity =>
            {
                entity.ToTable("RevisionesAceptadas");
                entity.HasKey(e => e.ClaveRevisionAceptada);

                entity.Property(e => e.ClaveRevisionAceptada)
                      .HasColumnName("claveRevisionAceptada")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_revision_aceptada()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.ClaveRevision)
                      .HasColumnName("claveRevision")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.HasIndex(e => e.ClaveRevision).IsUnique();

                entity.Property(e => e.ClaveReceptor)
                      .HasColumnName("claveReceptor")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.ClaveConvocatoria)
                      .HasColumnName("claveConvocatoria")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.FechaAceptacion)
                      .HasColumnName("fecha_aceptacion")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Estado)
                      .HasColumnName("Estado")
                      .HasDefaultValue(true);

                entity.HasOne(e => e.Revision)
                      .WithOne(r => r.RevisionAceptada)
                      .HasForeignKey<RevisionesAceptadasEntity>(e => e.ClaveRevision)
                      .HasConstraintName("fk_rev_buffer");

                entity.HasOne(e => e.Receptor)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveReceptor)
                      .HasConstraintName("fk_receptor_buffer");

                entity.HasOne(e => e.Convocatoria)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveConvocatoria)
                      .HasConstraintName("fk_convocatoria_buffer");
            });

            // CitasInscripcion
            modelBuilder.Entity<CitasInscripcionEntity>(entity =>
            {
                entity.ToTable("CitasInscripcion");
                entity.HasKey(e => e.ClaveCita);

                entity.Property(e => e.ClaveCita)
                      .HasColumnName("claveCita")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_cita()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.FechaCita)
                      .HasColumnName("fecha_cita")
                      .IsRequired();

                entity.Property(e => e.HoraCita)
                      .HasColumnName("hora_cita")
                      .IsRequired();

                entity.Property(e => e.EstadoCita)
                      .HasColumnName("estado_cita")
                      .HasMaxLength(20)
                      .HasDefaultValue("Programada");

                entity.HasCheckConstraint("ck_estado_cita",
                    "estado_cita IN ('Programada','Asistió','No Asistió')");

                entity.Property(e => e.Observaciones)
                      .HasColumnName("observaciones")
                      .IsRequired(false);

                entity.Property(e => e.CreateAt)
                      .HasColumnName("create_at")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.ClaveRevision)
                      .HasColumnName("claveRevision")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.HasIndex(e => e.ClaveRevision).IsUnique();

                entity.Property(e => e.ClaveTutorAspirante)
                      .HasColumnName("claveTutorAspirante")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.HasIndex(e => e.ClaveTutorAspirante).IsUnique();

                entity.HasOne(e => e.Revision)
                      .WithOne(r => r.CitaInscripcion)
                      .HasForeignKey<CitasInscripcionEntity>(e => e.ClaveRevision)
                      .HasConstraintName("fk_cita_revision");

                entity.HasOne(e => e.TutorAspirante)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveTutorAspirante)
                      .HasConstraintName("fk_cita_tutor");
            });

            // DetalleRevision
            modelBuilder.Entity<DetalleRevisionEntity>(entity =>
            {
                entity.ToTable("DetalleRevision");
                entity.HasKey(e => new { e.ClaveRevision, e.ClaveDocAspirante });

                entity.Property(e => e.ClaveRevision)
                      .HasColumnName("claveRevision")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.ClaveDocAspirante)
                      .HasColumnName("claveDocAspirante")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.EstatusDoc)
                      .HasColumnName("estatus_doc")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.HasCheckConstraint("ck_estatus_doc_revision",
                    "estatus_doc IN ('Aceptado','Rechazado')");

                entity.Property(e => e.MotivoRechazo)
                      .HasColumnName("motivo_rechazo")
                      .IsRequired(false);

                entity.HasOne(e => e.Revision)
                      .WithMany(r => r.DetalleRevisiones)
                      .HasForeignKey(e => e.ClaveRevision)
                      .HasConstraintName("fk_detalle_revision");

                entity.HasOne(e => e.DocumentoAspirante)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveDocAspirante)
                      .HasConstraintName("fk_detalle_documento");
            });

            // Entregas
            modelBuilder.Entity<EntregasEntity>(entity =>
            {
                entity.ToTable("Entregas");
                entity.HasKey(e => e.ClaveEntrega);

                entity.Property(e => e.ClaveEntrega)
                      .HasColumnName("claveEntrega")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_entrega()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.FechaFormalizacion)
                      .HasColumnName("fecha_formalizacion")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.EstadoFinal)
                      .HasColumnName("estado_final")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.ClaveCita)
                      .HasColumnName("claveCita")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.HasIndex(e => e.ClaveCita).IsUnique();

                entity.Property(e => e.ClaveTutorAspirante)
                      .HasColumnName("claveTutorAspirante")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.ClaveAspirante)
                      .HasColumnName("claveAspirante")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.HasIndex(e => e.ClaveAspirante).IsUnique();

                entity.Property(e => e.ClaveUsuario)
                      .HasColumnName("claveUsuario")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.HasOne(e => e.CitaInscripcion)
                      .WithOne(c => c.Entrega)
                      .HasForeignKey<EntregasEntity>(e => e.ClaveCita)
                      .HasConstraintName("fk_entregas_cita");

                entity.HasOne(e => e.TutorAspirante)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveTutorAspirante)
                      .HasConstraintName("fk_entregas_tutor");

                entity.HasOne(e => e.Aspirante)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveAspirante)
                      .HasConstraintName("fk_entregas_aspirante");

                // NUEVO: relación con Usuario (existía la columna pero no la relación)
                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveUsuario)
                      .HasConstraintName("fk_entregas_usuario");

                // NUEVO: la FK real de la relación 1:1 vive en AdjuncionesOriginales, no en Entregas
                entity.HasOne(e => e.AdjuncionOriginal)
                      .WithOne(a => a.Entrega) // ajusta el nombre si tu navegación inversa se llama distinto
                      .HasForeignKey<AdjuncionesOriginalesEntity>(a => a.ClaveEntrega)
                      .HasConstraintName("fk_adj_entrega");
            });

            // Expedientes
            modelBuilder.Entity<ExpedientesEntity>(entity =>
            {
                entity.ToTable("Expedientes");
                entity.HasKey(e => e.ClaveExpediente);

                entity.Property(e => e.ClaveExpediente)
                      .HasColumnName("claveExpediente")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_Expediente()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.Nombre)
                      .HasColumnName("nombre")
                      .HasMaxLength(80)
                      .IsRequired();

                entity.Property(e => e.ApellidoPaterno)
                      .HasColumnName("apellido_paterno")
                      .HasMaxLength(80)
                      .IsRequired();

                entity.Property(e => e.ApellidoMaterno)
                      .HasColumnName("apellido_materno")
                      .HasMaxLength(80);

                entity.Property(e => e.Curp)
                      .HasColumnName("curp")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.HasIndex(e => e.Curp).IsUnique();

                entity.Property(e => e.TipoTitular)
                      .HasColumnName("tipo_titular")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.HasCheckConstraint("ck_tipo_titular",
                    "tipo_titular IN ('Alumno','Empleado')");

                entity.Property(e => e.ClaveEntrega)
                      .HasColumnName("claveEntrega")
                      .HasMaxLength(18);

                entity.HasOne(e => e.Entrega)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveEntrega)
                      .HasConstraintName("fk_expediente_entrega")
                      .IsRequired(false);
            });

            // Empleados
            modelBuilder.Entity<EmpleadosEntity>(entity =>
            {
                entity.ToTable("Empleados");
                entity.HasKey(e => e.ClaveEmpleado);

                entity.Property(e => e.ClaveEmpleado)
                      .HasColumnName("claveEmpleado")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_empleado()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.FechaContratacion)
                      .HasColumnName("fecha_contratacion")
                      .IsRequired();

                entity.Property(e => e.TipoContrato)
                      .HasColumnName("tipo_contrato")
                      .HasMaxLength(10)
                      .IsRequired();

                entity.HasCheckConstraint("ck_tipo_contrato",
                    "tipo_contrato IN ('Planta','Temporal')");

                entity.Property(e => e.EstatusLaboral)
                      .HasColumnName("estatus_laboral")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.HasCheckConstraint("ck_estatus_laboral",
                    "estatus_laboral IN ('Activo','Baja')");

                entity.Property(e => e.Telefono)
                      .HasColumnName("telefono")
                      .HasMaxLength(15);

                entity.Property(e => e.ClaveExpediente)
                      .HasColumnName("claveExpediente")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.HasOne(e => e.Expediente)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveExpediente)
                      .HasConstraintName("fk_emp_expediente");
            });

            // Usuarios
            modelBuilder.Entity<UsuariosEntity>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(e => e.ClaveUsuario);

                entity.Property(e => e.ClaveUsuario)
                      .HasColumnName("claveUsuario")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_usuario()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.NombreUsuario)
                      .HasColumnName("nombre_usuario")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.HasIndex(e => e.NombreUsuario).IsUnique();

                entity.Property(e => e.Contrasenia)
                      .HasColumnName("contrasenia")
                      .HasMaxLength(255)
                      .IsRequired();

                entity.Property(e => e.CorreoInstitucional)
                      .HasColumnName("correo_institucional")
                      .HasMaxLength(100);

                entity.HasIndex(e => e.CorreoInstitucional).IsUnique();

                entity.Property(e => e.Estado)
                      .HasColumnName("estado")
                      .HasDefaultValue(true)
                      .IsRequired();

                entity.Property(e => e.ClaveEmpleado)
                      .HasColumnName("claveEmpleado")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.HasIndex(e => e.ClaveEmpleado).IsUnique();

                entity.HasOne(e => e.Empleado)
                      .WithOne(emp => emp.Usuario)
                      .HasForeignKey<UsuariosEntity>(e => e.ClaveEmpleado)
                      .HasConstraintName("fk_usuario_empleado");

                // FKs diferidas que apuntan a Usuarios

                // Publicaciones - Usuarios
                entity.HasMany(u => u.Publicaciones)
                      .WithOne(p => p.Usuario)
                      .HasForeignKey(p => p.ClaveUsuario)
                      .HasConstraintName("fk_usuario_publicador");

                // Revisiones - Usuarios
                entity.HasMany(u => u.Revisiones)
                      .WithOne(r => r.Usuario)
                      .HasForeignKey(r => r.ClaveUsuario)
                      .HasConstraintName("fk_rev_usuario");

                // Receptores - Usuarios
                entity.HasMany(u => u.Receptores)
                      .WithOne(rec => rec.Usuario)
                      .HasForeignKey(rec => rec.ClaveUsuario)
                      .HasConstraintName("fk_rec_usuario")
                      .IsRequired(false);

                // Entregas - Usuarios
                entity.HasMany(u => u.Entregas)
                      .WithOne(e2 => e2.Usuario)
                      .HasForeignKey(e2 => e2.ClaveUsuario)
                      .HasConstraintName("fk_entregas_usuario");

                entity.HasMany(u => u.Logueos)
                      .WithOne(l => l.Usuario)
                      .HasForeignKey(l => l.ClaveUsuario)
                      .HasConstraintName("fk_logueos_usuarios")
                      .IsRequired(false);
            });

            // EmpleadoRol
            modelBuilder.Entity<EmpleadoRolEntity>(entity =>
            {
                entity.ToTable("EmpleadoRol");
                entity.HasKey(e => new { e.ClaveEmpleado, e.ClaveRol, e.FechaInicio });

                entity.Property(e => e.ClaveEmpleado)
                      .HasColumnName("claveEmpleado")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.ClaveRol)
                      .HasColumnName("claveRol")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.FechaInicio)
                      .HasColumnName("fecha_inicio")
                      .HasDefaultValueSql("CURRENT_DATE")
                      .IsRequired();

                entity.Property(e => e.FechaFin)
                      .HasColumnName("fecha_fin");

                entity.HasOne(e => e.Empleado)
                      .WithMany(emp => emp.EmpleadoRoles)
                      .HasForeignKey(e => e.ClaveEmpleado)
                      .HasConstraintName("fk_rel_empleado")
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Rol)
                      .WithMany(r => r.EmpleadoRoles)
                      .HasForeignKey(e => e.ClaveRol)
                      .HasConstraintName("fk_rel_rol")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Alumnos
            modelBuilder.Entity<AlumnosEntity>(entity =>
            {
                entity.ToTable("Alumnos");
                entity.HasKey(e => e.ClaveAlumno);

                entity.Property(e => e.ClaveAlumno)
                      .HasColumnName("claveAlumno")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_Alumno()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.Matricula)
                      .HasColumnName("matricula")
                      .HasMaxLength(20);

                entity.HasIndex(e => e.Matricula).IsUnique();

                entity.Property(e => e.Estado)
                      .HasColumnName("estado")
                      .HasMaxLength(10)
                      .HasDefaultValue("Activo");

                entity.HasCheckConstraint("ck_alumno_estado", "estado IN ('Activo','Baja','Egresado')");

                entity.Property(e => e.ClaveExpediente)
                      .HasColumnName("claveExpediente")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.HasIndex(e => e.ClaveExpediente).IsUnique();

                entity.HasOne(e => e.Expediente)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveExpediente)
                      .HasConstraintName("fk_alumno_expediente");
            });

            // Grupos
            modelBuilder.Entity<GruposEntity>(entity =>
            {
                entity.ToTable("Grupos");
                entity.HasKey(e => e.ClaveGrupo);

                entity.Property(e => e.ClaveGrupo)
                      .HasColumnName("claveGrupo")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_grupo()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.Grado)
                      .HasColumnName("grado")
                      .HasMaxLength(1)
                      .IsRequired();

                entity.HasCheckConstraint("ck_grado_grupo", "grado IN ('1','2','3')");

                entity.Property(e => e.Seccion)
                      .HasColumnName("seccion")
                      .HasMaxLength(1)
                      .IsRequired();

                entity.HasCheckConstraint("ck_seccion_grupo", "seccion IN ('A','B')");

                entity.Property(e => e.CapacidadMaxima)
                      .HasColumnName("capacidad_maxima")
                      .IsRequired();

                entity.Property(e => e.Generacion)
                      .HasColumnName("generacion")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.Estado)      
                      .HasColumnName("estado")
                      .HasDefaultValue(true);
            });

            // TutoresAlumnos
            modelBuilder.Entity<TutoresAlumnosEntity>(entity =>
            {
                entity.ToTable("TutoresAlumnos");
                entity.HasKey(e => new { e.ClaveAlumno, e.ClaveTutor });

                entity.Property(e => e.ClaveAlumno)
                      .HasColumnName("claveAlumno")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.ClaveTutor)
                      .HasColumnName("claveTutor")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.FechaInicio)
                      .HasColumnName("fecha_inicio")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaBaja)
                      .HasColumnName("fecha_baja");

                entity.HasOne(e => e.Alumno)
                      .WithMany(a => a.TutoresAlumnos)
                      .HasForeignKey(e => e.ClaveAlumno)
                      .HasConstraintName("fk_rel_alumno");

                entity.HasOne(e => e.Tutor)
                      .WithMany(t => t.TutoresAlumnos)
                      .HasForeignKey(e => e.ClaveTutor)
                      .HasConstraintName("fk_rel_tutor");
            });

            // CiclosEscolares
            modelBuilder.Entity<CiclosEscolaresEntity>(entity =>
            {
                entity.ToTable("CiclosEscolares");
                entity.HasKey(e => e.ClaveCiclo);

                entity.Property(e => e.ClaveCiclo)
                      .HasColumnName("claveCiclo")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_ciclo()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.NombreCiclo)
                      .HasColumnName("nombreCiclo")
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.FechaInicio)
                      .HasColumnName("fechaInicio")
                      .IsRequired();

                entity.Property(e => e.FechaFin)
                      .HasColumnName("fechaFin")
                      .IsRequired();

                entity.Property(e => e.Estatus)
                      .HasColumnName("estatus")
                      .HasDefaultValue(true);

                entity.HasCheckConstraint("chk_fechas", "\"fechaFin\" > \"fechaInicio\"");
            });

            // AsignacionGrupo
            modelBuilder.Entity<AsignacionGrupoEntity>(entity =>
            {
                entity.ToTable("AsignacionGrupo");
                entity.HasKey(e => e.ClaveAsignacion);

                entity.Property(e => e.ClaveAsignacion)
                      .HasColumnName("claveAsignacion")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.ClaveAlumno)
                      .HasColumnName("claveAlumno")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.ClaveGrupo)
                      .HasColumnName("claveGrupo")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.ClaveUsuario)
                     .HasColumnName("claveUsuario")
                     .HasMaxLength(18)
                     .IsRequired();

                entity.Property(e => e.ClaveCiclo)
                      .HasColumnName("claveCiclo")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.FechaAsignacion)
                      .HasColumnName("fecha_asignacion")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Estatus)
                      .HasColumnName("estatus")
                      .HasMaxLength(20)
                      .HasDefaultValue("ACTIVO");

                entity.HasCheckConstraint("chk_estatus",
                    "estatus IN ('ACTIVO','REPROBADO','APROBADO','BAJA')");

                entity.HasOne(e => e.Alumno)
                      .WithMany(a => a.AsignacionGrupos)
                      .HasForeignKey(e => e.ClaveAlumno)
                      .HasConstraintName("fk_asig_alumno");

                entity.HasOne(e => e.Grupo)
                      .WithMany(g => g.AsignacionGrupos)
                      .HasForeignKey(e => e.ClaveGrupo)
                      .HasConstraintName("fk_asig_grupo");

                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveUsuario)
                      .HasConstraintName("fk_asig_usuario");

                entity.HasOne(e => e.CicloEscolar)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveCiclo)
                      .HasConstraintName("fk_asig_ciclo");
            });

            // ValidacionDocumentos
            modelBuilder.Entity<ValidacionDocumentosEntity>(entity =>
            {
                entity.ToTable("ValidacionDocumentos");
                entity.HasKey(e => new { e.ClaveEntrega, e.ClaveDocAspirante });

                entity.Property(e => e.ClaveEntrega)
                      .HasColumnName("claveEntrega")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.ClaveDocAspirante)
                      .HasColumnName("claveDocAspirante")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.EstatusCotejo)
                      .HasColumnName("estatus_cotejo")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.FechaValidacion)
                      .HasColumnName("fecha_validacion")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Entrega)
                      .WithMany(e2 => e2.ValidacionDocumentos)
                      .HasForeignKey(e => e.ClaveEntrega)
                      .HasConstraintName("fk_validacion_entrega");

                entity.HasOne(e => e.DocumentoAspirante)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveDocAspirante)
                      .HasConstraintName("fk_validacion_documento");
            });

            // AdjuncionesOriginales
            modelBuilder.Entity<AdjuncionesOriginalesEntity>(entity =>
            {
                entity.ToTable("AdjuncionesOriginales");
                entity.HasKey(e => e.ClaveAdjOriginal);

                entity.Property(e => e.ClaveAdjOriginal)
                      .HasColumnName("claveAdjOriginal")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_adj_original()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.ClaveEntrega)
                      .HasColumnName("claveEntrega")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.HasIndex(e => e.ClaveEntrega).IsUnique();

                entity.Property(e => e.ClaveUsuario)
                      .HasColumnName("claveUsuario")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.FechaCarga)
                      .HasColumnName("fecha_carga")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveUsuario)
                      .IsRequired();
            });

            // DetalleAdjuncionOriginal
            modelBuilder.Entity<DetalleAdjuncionOriginalEntity>(entity =>
            {
                entity.ToTable("DetalleAdjuncionOriginal");
                entity.HasKey(e => new { e.ClaveAdjOriginal, e.ClaveDocAspirante });

                entity.Property(e => e.ClaveAdjOriginal)
                      .HasColumnName("claveAdjOriginal")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.ClaveDocAspirante)
                      .HasColumnName("claveDocAspirante")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.RutaPdfOriginal)
                      .HasColumnName("ruta_pdf_original")
                      .IsRequired();

                entity.HasOne(e => e.AdjuncionOriginal)
                      .WithMany(a => a.Detalles)
                      .HasForeignKey(e => e.ClaveAdjOriginal)
                      .HasConstraintName("fk_detalle_maestro");

                entity.HasOne(e => e.DocumentoAspirante)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveDocAspirante)
                      .HasConstraintName("fk_detalle_doc_aspirante");
            });

            // Documentos
            modelBuilder.Entity<DocumentosEntity>(entity =>
            {
                entity.ToTable("Documentos");
                entity.HasKey(e => e.ClaveDocumento);

                entity.Property(e => e.ClaveDocumento)
                      .HasColumnName("claveDocumento")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_Documento()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.ArchivoUrl)
                      .HasColumnName("archivo_url")
                      .HasMaxLength(255)
                      .IsRequired();

                entity.Property(e => e.Estado)
                      .HasColumnName("estado")
                      .HasMaxLength(80);

                entity.Property(e => e.FechaSubida)
                      .HasColumnName("fecha_subida")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.ClaveExpediente)
                      .HasColumnName("claveExpediente")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.ClaveTipoDocumento)
                      .HasColumnName("claveTipoDocumento")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.HasOne(e => e.Expediente)
                      .WithMany(ex => ex.Documentos)
                      .HasForeignKey(e => e.ClaveExpediente)
                      .HasConstraintName("fk_documentos_expediente");

                entity.HasOne(e => e.TipoDocumento)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveTipoDocumento)
                      .HasConstraintName("fk_documentos_tipo");
            });

            // Nuevas tablas para OnModelCreating
            // Modulos
            modelBuilder.Entity<ModulosEntity>(entity =>
            {
                entity.ToTable("Modulos");
                entity.HasKey(e => e.ClaveModulo);

                entity.Property(e => e.ClaveModulo)
                      .HasColumnName("claveModulo")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_modulo()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.NombreModulo)
                      .HasColumnName("nombre_modulo")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.HasIndex(e => e.NombreModulo).IsUnique();

                entity.Property(e => e.Descripcion)
                      .HasColumnName("descripcion");

                entity.Property(e => e.UrlModulo)
                      .HasColumnName("url_modulo")
                      .HasMaxLength(100);

                entity.Property(e => e.EstadoModulo)
                      .HasColumnName("estado_modulo")
                      .HasDefaultValue(true);

                entity.Property(e => e.ClaveModuloPadre)
                      .HasColumnName("claveModuloPadre")
                      .HasMaxLength(18);

                // Autorreferencia: un módulo puede tener un padre del mismo tipo
                entity.HasOne(e => e.ModuloPadre)
                      .WithMany(e => e.SubModulos)
                      .HasForeignKey(e => e.ClaveModuloPadre)
                      .HasConstraintName("fk_modulo_padre")
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired(false);
            });

            // Permisos
            modelBuilder.Entity<PermisosEntity>(entity =>
            {
                entity.ToTable("Permisos");
                entity.HasKey(e => new { e.ClaveRol, e.ClaveModulo });

                entity.Property(e => e.ClaveRol)
                      .HasColumnName("claveRol")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.ClaveModulo)
                      .HasColumnName("claveModulo")
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(e => e.PuedeVer)
                      .HasColumnName("puede_ver")
                      .HasDefaultValue(true);

                entity.Property(e => e.PuedeCrear)
                      .HasColumnName("puede_crear")
                      .HasDefaultValue(false);

                entity.Property(e => e.PuedeEditar)
                      .HasColumnName("puede_editar")
                      .HasDefaultValue(false);

                entity.Property(e => e.PuedeEliminar)
                      .HasColumnName("puede_eliminar")
                      .HasDefaultValue(false);

                entity.Property(e => e.FechaAsignacion)
                      .HasColumnName("fecha_asignacion")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Rol)
                      .WithMany(r => r.Permisos)
                      .HasForeignKey(e => e.ClaveRol)
                      .HasConstraintName("fk_permiso_rol");

                entity.HasOne(e => e.Modulo)
                      .WithMany(m => m.Permisos)
                      .HasForeignKey(e => e.ClaveModulo)
                      .HasConstraintName("fk_permiso_modulo");
            });

            // Logueos
            modelBuilder.Entity<LogueosEntity>(entity =>
            {
                entity.ToTable("Logueos");
                entity.HasKey(e => e.ClaveLogueo);

                entity.Property(e => e.ClaveLogueo)
                      .HasColumnName("claveLogueo")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_logueo()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.ClaveUsuario)
                      .HasColumnName("claveUsuario")
                      .HasMaxLength(18);

                entity.Property(e => e.FechaAcceso)
                      .HasColumnName("fecha_acceso")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.EstatusIntento)
                      .HasColumnName("estatus_intento")
                      .HasMaxLength(25)
                      .IsRequired();

                entity.HasCheckConstraint("chk_estatus_intento",
                    "estatus_intento IN ('Exitoso','Contraseña Incorrecta','Usuario Suspendido','Usuario Inexistente','Sesión Finalizada')");

                entity.Property(e => e.DireccionIp)
                      .HasColumnName("direccion_ip")
                      .HasMaxLength(45)
                      .HasDefaultValue("0.0.0.0");

                entity.Property(e => e.AgenteUsuario)
                      .HasColumnName("agente_usuario")
                      .HasDefaultValue("Desconocido");

                entity.Property(e => e.FechaCierre)
                      .HasColumnName("fecha_cierre");

                // FK nullable: el usuario puede no existir en caso de intento fallido
                entity.HasOne(e => e.Usuario)
                      .WithMany(u => u.Logueos)
                      .HasForeignKey(e => e.ClaveUsuario)
                      .HasConstraintName("fk_logueos_usuarios")
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired(false);
            });

            // TokenConvocatorias
            modelBuilder.Entity<TokenConvocatoriasEntity>(entity =>
            {
                entity.ToTable("TokenConvocatorias");
                entity.HasKey(e => e.ClaveTokenConvocatoria);

                entity.Property(e => e.ClaveTokenConvocatoria)
                      .HasColumnName("claveTokenConvocatoria")
                      .HasMaxLength(20)
                      .HasDefaultValueSql("generar_token_convocatoria()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.TokenOriginal)
                      .HasColumnName("token_original")
                      .IsRequired();

                entity.Property(e => e.ClaveTutorAspirante)
                      .HasColumnName("claveTutorAspirante")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.FechaInicio)
                      .HasColumnName("fecha_inicio")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FechaExpiracion)
                      .HasColumnName("fecha_expiracion")
                      .IsRequired();

                entity.Property(e => e.IpOrigen)
                      .HasColumnName("ip_origen")
                      .HasMaxLength(45);

                entity.Property(e => e.DispositivoOrigen)
                      .HasColumnName("dispositivo_origen");

                entity.Property(e => e.EstadoSesion)
                      .HasColumnName("estado_sesion")
                      .HasDefaultValue(true);

                entity.HasOne(e => e.TutorAspirante)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveTutorAspirante)
                      .HasConstraintName("fk_token_tutor");
            });

            // CargasDocumentos
            modelBuilder.Entity<CargasDocumentosEntity>(entity =>
            {
                entity.ToTable("CargasDocumentos");
                entity.HasKey(e => e.ClaveCarga);

                entity.Property(e => e.ClaveCarga)
                      .HasColumnName("claveCarga")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("genera_clave_carga()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.ClaveExpediente)
                      .HasColumnName("claveExpediente")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.HasIndex(e => e.ClaveExpediente).IsUnique();

                entity.Property(e => e.ClaveUsuario)
                      .HasColumnName("claveUsuario")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.FechaCarga)
                      .HasColumnName("fecha_carga")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Observaciones)
                      .HasColumnName("observaciones");

                entity.Property(e => e.EstatusValidacion)
                      .HasColumnName("estatus_validacion")
                      .HasMaxLength(20)
                      .HasDefaultValue("En Proceso");

                entity.HasOne(e => e.Expediente)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveExpediente)
                      .HasConstraintName("fk_expediente_carga");

                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveUsuario)
                      .HasConstraintName("fk_usuario_carga");
            });

            // DetalleCarga
            modelBuilder.Entity<DetalleCargaEntity>(entity =>
            {
                entity.ToTable("DetalleCarga");
                entity.HasKey(e => new { e.ClaveCarga, e.ClaveDocumento });

                entity.Property(e => e.ClaveCarga)
                      .HasColumnName("claveCarga")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.ClaveDocumento)
                      .HasColumnName("claveDocumento")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.ArchivoUrl)
                      .HasColumnName("archivo_url")
                      .HasMaxLength(255)
                      .IsRequired();

                entity.Property(e => e.FechaSubida)
                      .HasColumnName("fecha_subida")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.CargaDocumento)
                      .WithMany(c => c.DetalleCarga)
                      .HasForeignKey(e => e.ClaveCarga)
                      .HasConstraintName("fk_carga");

                entity.HasOne(e => e.Documento)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveDocumento)
                      .HasConstraintName("fk_documento");
            });

            // Periodos
            modelBuilder.Entity<PeriodosEntity>(entity =>
            {
                entity.ToTable("Periodos");
                entity.HasKey(e => e.ClavePeriodo);

                entity.Property(e => e.ClavePeriodo)
                      .HasColumnName("clavePeriodo")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_periodo()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.ClaveCiclo)
                      .HasColumnName("claveCiclo")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.HasIndex(e => e.ClaveCiclo).IsUnique();

                entity.Property(e => e.NombrePeriodo)
                      .HasColumnName("nombre_periodo")
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.FechaInicio)
                      .HasColumnName("fecha_inicio")
                      .IsRequired();

                entity.Property(e => e.FechaFin)
                      .HasColumnName("fecha_fin")
                      .IsRequired();

                entity.Property(e => e.EstadoPeriodo)
                      .HasColumnName("estado_periodo")
                      .HasDefaultValue(true);

                entity.Property(e => e.FechaRegistro)
                      .HasColumnName("fecha_registro")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.CicloEscolar)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveCiclo)
                      .HasConstraintName("fk_periodo_ciclo");
            });

            // Pagos
            modelBuilder.Entity<PagosEntity>(entity =>
            {
                entity.ToTable("Pagos");
                entity.HasKey(e => e.ClavePago);

                entity.Property(e => e.ClavePago)
                      .HasColumnName("clavePago")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_pago()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.ClaveTutor)
                      .HasColumnName("claveTutor")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.ClaveUsuario)
                      .HasColumnName("claveUsuario")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.ClaveCiclo)
                      .HasColumnName("claveCiclo")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.Monto)
                      .HasColumnName("monto")
                      .HasPrecision(10, 2)
                      .IsRequired();

                entity.Property(e => e.FechaPago)
                      .HasColumnName("fecha_pago")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.MetodoPago)
                      .HasColumnName("metodo_pago")
                      .HasMaxLength(30)
                      .IsRequired();

                entity.Property(e => e.ComprobantePago)
                      .HasColumnName("comprobante_pago")
                      .HasMaxLength(100);

                entity.Property(e => e.Referencia)
                      .HasColumnName("referencia");

                entity.Property(e => e.EstadoPago)
                      .HasColumnName("estado_pago")
                      .HasDefaultValue(true);

                entity.HasCheckConstraint("chk_metodo_pago",
                    "metodo_pago IN ('Efectivo','Transferencia','Deposito')");

                entity.HasOne(e => e.Tutor)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveTutor)
                      .HasConstraintName("fk_pago_tutor");

                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveUsuario)
                      .HasConstraintName("fk_pago_usuario");

                entity.HasOne(e => e.CicloEscolar)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveCiclo)
                      .HasConstraintName("fk_pago_ciclo");
            });

            // Inscripciones
            modelBuilder.Entity<InscripcionesEntity>(entity =>
            {
                entity.ToTable("Inscripciones");
                entity.HasKey(e => e.ClaveInscripcion);

                entity.Property(e => e.ClaveInscripcion)
                      .HasColumnName("claveInscripcion")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_inscripcion()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.ClaveAlumno)
                      .HasColumnName("claveAlumno")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.ClaveCiclo)
                      .HasColumnName("claveCiclo")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.ClavePeriodo)
                      .HasColumnName("clavePeriodo")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.ClaveGrupo)
                      .HasColumnName("claveGrupo")
                      .HasMaxLength(18);

                entity.Property(e => e.ClaveUsuario)
                      .HasColumnName("claveUsuario")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.ClavePago)
                      .HasColumnName("clavePago")
                      .HasMaxLength(18);

                entity.Property(e => e.FechaInscripcion)
                      .HasColumnName("fecha_inscripcion")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.EstatusInscripcion)
                      .HasColumnName("estatus_inscripcion")
                      .HasMaxLength(30)
                      .HasDefaultValue("PENDIENTE");

                entity.Property(e => e.Observaciones)
                      .HasColumnName("observaciones");

                entity.HasCheckConstraint("chk_estatus_ins",
                    "estatus_inscripcion IN ('INSCRITO','CANCELADA')");

                entity.HasIndex(e => new { e.ClaveAlumno, e.ClavePeriodo })
                      .IsUnique()
                      .HasDatabaseName("uk_ins_alumno_periodo");

                entity.HasOne(e => e.Alumno)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveAlumno)
                      .HasConstraintName("fk_ins_alumno");

                entity.HasOne(e => e.CicloEscolar)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveCiclo)
                      .HasConstraintName("fk_ins_ciclo");

                entity.HasOne(e => e.Periodo)
                      .WithMany()
                      .HasForeignKey(e => e.ClavePeriodo)
                      .HasConstraintName("fk_ins_periodo");

                entity.HasOne(e => e.Grupo)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveGrupo)
                      .HasConstraintName("fk_ins_grupo")
                      .IsRequired(false);

                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveUsuario)
                      .HasConstraintName("fk_ins_usuario");

                entity.HasOne(e => e.Pago)
                      .WithMany()
                      .HasForeignKey(e => e.ClavePago)
                      .HasConstraintName("fk_ins_pago")
                      .IsRequired(false);
            });

            modelBuilder.Entity<RutaRechazadaEntity>(entity =>
            {
                entity.ToTable("RutasRechazadas");
                entity.HasKey(e => e.ClaveRuta);

                entity.Property(e => e.ClaveRuta)
                      .HasColumnName("claveRuta")
                      .HasMaxLength(18)
                      .HasDefaultValueSql("generar_clave_ruta_rechazada()")
                      .ValueGeneratedOnAdd()
                      .IsRequired();

                entity.Property(e => e.ClaveAdjuncion)
                      .HasColumnName("claveAdjuncion")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.ClaveDocAspirante)
                      .HasColumnName("claveDocAspirante")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.ClaveRevision)
                      .HasColumnName("claveRevision")
                      .HasMaxLength(18)
                      .IsRequired();

                entity.Property(e => e.RutaArchivoRechazado)
                      .HasColumnName("ruta_archivo_rechazado")
                      .HasMaxLength(255)
                      .IsRequired();

                entity.Property(e => e.FechaRegistro)
                      .HasColumnName("fecha_registro")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Adjuncion)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveAdjuncion)
                      .HasConstraintName("fk_rutas_adjuncion");

                entity.HasOne(e => e.DocumentoAspirante)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveDocAspirante)
                      .HasConstraintName("fk_rutas_documento");

                entity.HasOne(e => e.Revision)
                      .WithMany()
                      .HasForeignKey(e => e.ClaveRevision)
                      .HasConstraintName("fk_rutas_revision");
            });
        }
    }
}