using _00_Entities;                 // Para SmtpSettings
using _01_DataLogic.Clases;         // ContactDal y EmailService
using _02_BusinessLogic.Clases;     // BL
using _02_BusinessLogic.Interfaces; // Interfaces
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// =========================
// Configurar CORS
// =========================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

// =========================
// Configurar Controllers
// =========================
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.MaxDepth = 64;
});

// =========================
// Registro de dependencias
// =========================
builder.Services.AddScoped<IChatBL, ChatBL>();
builder.Services.AddScoped<IContactBL, ContactBL>();
builder.Services.AddScoped<IPdfBL, PdfBL>();
builder.Services.AddScoped<ISexoBL, SexoBL>();

// Registro del DAL
builder.Services.AddScoped<ContactDal>();

// Bind de configuración SMTP y registro del servicio de email
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

// =========================
// QuestPDF
// =========================
QuestPDF.Settings.License = LicenseType.Community;

// =========================
// Swagger
// =========================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// =========================
// Middleware
// =========================
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
