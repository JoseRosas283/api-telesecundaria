using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Implementations;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Implementations;
using Telesecundaria.Services.Interfaces;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.FileProviders;
using FluentValidation;
using FluentValidation.AspNetCore;
using Telesecundaria.Validators.Auth;

var builder = WebApplication.CreateBuilder(args);

// Uploads path Guardar en cualquier servidor
var uploadsPath = builder.Configuration["Storage:UploadsPath"]!;
if (!Path.IsPathRooted(uploadsPath))
    uploadsPath = Path.Combine(builder.Environment.ContentRootPath, uploadsPath);
Directory.CreateDirectory(uploadsPath);

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddKeyedSingleton("uploadsPath", uploadsPath);

// Base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnectionString"))
           .LogTo(Console.WriteLine, LogLevel.Information)
           .EnableSensitiveDataLogging()
);

// JWT
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Proxy para https 
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Repositorios
builder.Services.AddScoped<IExpedientesRepository, ExpedientesRepository>();
builder.Services.AddScoped<IEmpleadosRepository, EmpleadosRepository>();
builder.Services.AddScoped<IRolesRepository, RolesRepository>();
builder.Services.AddScoped<IEmpleadoRolRepository, EmpleadoRolRepository>();
builder.Services.AddScoped<IDocumentosRepository, DocumentosRepository>();
builder.Services.AddScoped<IUsuariosRepository, UsuariosRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthTutorRepository, AuthTutorRepository>();
builder.Services.AddScoped<IModuloRepository, ModuloRepository>();
builder.Services.AddScoped<IPermisoRepository, PermisosRepository>();
// Dependency Injection
builder.Services.AddScoped<IConvocatoriasRepository, ConvocatoriasRepository>();
builder.Services.AddScoped<IConvocatoriasService, ConvocatoriasService>();
builder.Services.AddScoped<IGaleriaImagenesRepository, GaleriaImagenesRepository>();
builder.Services.AddScoped<IGaleriaImagenesService, GaleriaImagenesService>();
builder.Services.AddScoped<IImagenService, ImagenService>();
builder.Services.AddScoped<IPublicacionesRepository, PublicacionesRepository>();
builder.Services.AddScoped<IPublicacionesService, PublicacionesService>();
builder.Services.AddScoped<IGruposRepository, GruposRepository>();
builder.Services.AddScoped<IGruposService, GruposService>();
builder.Services.AddScoped<IAsignacionGrupoRepository, AsignacionGrupoRepository>();
builder.Services.AddScoped<IAsignacionGrupoService, AsignacionGrupoService>();
builder.Services.AddScoped<ITutorAspiranteRepository, TutorAspiranteRepository>();
builder.Services.AddScoped<ITutorAspiranteService, TutorAspiranteService>();
builder.Services.AddScoped<IAspirantesRepository, AspirantesRepository>();
builder.Services.AddScoped<IAspirantesService, AspirantesService>();
builder.Services.AddScoped<ITipoDocumentosRepository, TipoDocumentosRepository>();
builder.Services.AddScoped<ITipoDocumentosService, TipoDocumentosService>();
builder.Services.AddScoped<IRequisitosRepository, RequisitosRepository>();
builder.Services.AddScoped<IRequisitosService, RequisitosService>();
builder.Services.AddScoped<IAdjuncionesRepository, AdjuncionesRepository>();
builder.Services.AddScoped<IAdjuncionesService, AdjuncionesService>();
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped<IAdjuncionesRepository, AdjuncionesRepository>();
builder.Services.AddScoped<IAdjuncionesService, AdjuncionesService>();
builder.Services.AddScoped<ICiclosEscolaresRepository, CiclosEscolaresRepository>();
builder.Services.AddScoped<ICiclosEscolaresService, CiclosEscolaresService>();
builder.Services.AddScoped<IInscripcionesRepository, InscripcionesRepository>();
builder.Services.AddScoped<IInscripcionesService, InscripcionesService>();
builder.Services.AddScoped<IPagosRepository, PagosRepository>();
builder.Services.AddScoped<IPagosService, PagosService>();
builder.Services.AddScoped<IRevisionesRepository, RevisionesRepository>();
builder.Services.AddScoped<IRevisionesService, RevisionesService>();
builder.Services.AddScoped<IDetalleRevisionRepository, DetalleRevisionRepository>();
builder.Services.AddScoped<IDetalleRevisionService, DetalleRevisionService>();
builder.Services.AddScoped<IRevisionesAceptadasRepository, RevisionesAceptadasRepository>();
builder.Services.AddScoped<IRevisionesAceptadasService, RevisionesAceptadasService>();
builder.Services.AddScoped<ICitasInscripcionRepository, CitasInscripcionRepository>();
builder.Services.AddScoped<ICitasInscripcionService, CitasInscripcionService>();
builder.Services.AddScoped<IAdjuncionesOriginalesRepository, AdjuncionesOriginalesRepository>();
builder.Services.AddScoped<IAdjuncionesOriginalesService, AdjuncionesOriginalesService>();
builder.Services.AddScoped<ITutoresRepository, TutoresRepository>();
builder.Services.AddScoped<ITutoresService, TutoresService>();
builder.Services.AddScoped<ITutoresAlumnosRepository, TutoresAlumnosRepository>();
builder.Services.AddScoped<ITutoresAlumnosService, TutoresAlumnosService>();
builder.Services.AddScoped<ITipoNotificacionesRepository, TipoNotificacionesRepository>();
builder.Services.AddScoped<ITipoNotificacionesService, TipoNotificacionesService>();
builder.Services.AddScoped<IDestinoNotificacionRepository, DestinoNotificacionRepository>();
builder.Services.AddScoped<IDestinoNotificacionService, DestinoNotificacionService>();
builder.Services.AddScoped<IReceptoresRepository, ReceptoresRepository>();
builder.Services.AddScoped<IReceptoresService, ReceptoresService>();
builder.Services.AddScoped<IEnviosRepository, EnviosRepository>();
builder.Services.AddScoped<IEnviosService, EnviosService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHostedService<EnvioCorreoBackgroundService>();

// Servicios
builder.Services.AddScoped<IExpedienteService, ExpedienteService>();
builder.Services.AddScoped<IEmpleadoService, EmpleadoService>();
builder.Services.AddScoped<IRolesService, RolesService>();
builder.Services.AddScoped<IEmpleadoRolService, EmpleadoRolService>();
builder.Services.AddScoped<IDocumentoService, DocumentoService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthTutorService, AuthTutorService>();
builder.Services.AddScoped<IModuloService, ModuloService>();
builder.Services.AddScoped<IPermisoService, PermisoService>();


//FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<LoguinRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Swagger con JWT — solo UNA vez
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = ""
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy => policy.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

var app = builder.Build();

app.UseForwardedHeaders();

app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads/expedientes"
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseCors("AllowAngular");

app.UseAuthorization();

app.MapControllers();

app.Run();
