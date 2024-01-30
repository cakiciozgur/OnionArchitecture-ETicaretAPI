using ETicaretAPI.API.Configurations.ColumnWriters;
using ETicaretAPI.API.Extensions;
using ETicaretAPI.Application;
using ETicaretAPI.Application.Validators.Products;
using ETicaretAPI.Infastructure;
using ETicaretAPI.Infastructure.Enums;
using ETicaretAPI.Infastructure.Services.Storage.Local;
using ETicaretAPI.Persistence;
using ETicaretAPI.SignalR;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Sinks.PostgreSQL;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor(); // clienttan gelen request ile olu�turulan httpcontext nesnesine katmanlardaki classlar �zerinden (business logic) eri�ebilmemizi sa�layan bir servistir.

builder.Services.AddPersistenceServices();
builder.Services.AddInfastructureServices();
builder.Services.AddStorage<LocalStorage>(); // => or builder.Services.AddStorage<AzureStorage>(); builder.Services.AddStorage(StorageType.Azure); // enum kullanmak yerine generic yap�lanma kullanma daha mant�kl�
builder.Services.AddApplicationServices();
builder.Services.AddControllers();
builder.Services.AddFluentValidationServices();
builder.Services.AddSignalRServices();

Logger logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt")
    .WriteTo.PostgreSQL(builder.Configuration.GetConnectionString("PostgreSQL"),
    "logs",
    needAutoCreateTable: true,
    columnOptions: new Dictionary<string, ColumnWriterBase> /*columnOptions => eger default olanlardan farkl� kolonlar eklemek istiyorsak var olanlar�da ekleyip onun �zerine �ALI�MA YAPMALIYIZ*/
    {
        { "message" , new RenderedMessageColumnWriter() }, /*RenderedMessageColumnWriter �al��mas� sonucu gelen veri message kolonuna yaz�lacak*/
        { "message_template" , new MessageTemplateColumnWriter() },
        { "level" , new LevelColumnWriter() },
        { "time_stamp" , new TimestampColumnWriter() },
        { "exception" , new ExceptionColumnWriter() },        
        { "log_event" , new LogEventSerializedColumnWriter() },        
        { "user_name" ,  new UsernameColumnWriter() },        
    })
    .WriteTo.Seq(builder.Configuration["Seq:ServerUrl"])// seq g�rsel aray�z�ne yazd�r�ls�n.
    .Enrich.FromLogContext() // log contextten beslenmesi gerekti�ini buraya(context'e) veriler ekledi�imizi bildiriyoruz.
    .MinimumLevel.Information()
    .CreateLogger();

builder.Host.UseSerilog(logger);

builder.Services.AddHttpLogging(logging => // YAPILAN HTTP REQUESTLER� ���N CONF�GURAT�ON AYARLARI => app.userhttplogging; ekledi�imizde t�m request ve responsler� logluyor olaca��z
{
    logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
    logging.RequestHeaders.Add("sec-ch-ua");
    logging.ResponseHeaders.Add("E-Commerce-API");
    logging.MediaTypeOptions.AddText("application/json");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Admin",options =>// bir token ile istek geldi�inde bunu jwt olarak de�erlendir.
    {
        options.TokenValidationParameters = new() // jwt token i�erisinde do�rulanmas� gereken verileri belirtiyoruz
        {
            ValidateIssuer = true, // olu�turulacak tokenin kimin / hangi originin/sitelerin kullanaca��n� belirleriz. => www.xyz.com
            ValidateAudience = true, //  olu�turulacak token de�erini kimin da��tt���n� belirtir. => www.xyzapi.com
            ValidateLifetime = true, // token s�resini kontrol eder
            ValidateIssuerSigningKey = true, // �retilecek olan tokenin uygulamam�za ait bir de�er oldu�unu ifade eden secury key verisinin do�rulanmas�d�r,

            /*do�rulanacak veriler*/
            ValidAudience = builder.Configuration["Token:Audience"],
            ValidIssuer = builder.Configuration["Token:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
            LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow : false,

            NameClaimType = ClaimTypes.Name /*jwt �zerinde name claimine kar��l�k gelen de�erini  User.Identity.Name  propertysi �zerinden elde edebilmek i�in eklendi*/,
        };
    }); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy=> policy.WithOrigins("http://localhost:4200", "http://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials()));
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.ConfigureExceptionHandler<Program>(app.Services.GetRequiredService<ILogger<Program>>()); // global exception handling
app.UseSerilogRequestLogging(); // UseSerilogRequestLogging koyulan sat�rdan �nceki i�lemler loglanmaz dolay�s�yla koydu�umuz yer kritik �neme sahiptir.

app.UseHttpLogging(); // t�m http isteklerini loglama i�in eklendi.
app.UseCors();
app.UseHttpsRedirection();

app.UseAuthentication(); // authorize i�in eklenmeli

app.UseAuthorization();

app.Use(async (context, next) =>
{
    var username = context.User?.Identity?.IsAuthenticated != null || true ? context.User.Identity.Name : null;

    LogContext.PushProperty("user_name", username); // log context static yap�lanmas� sayesinde log contextine ekliyoruz.

    await next();
});

app.MapControllers();
app.MapHubs();
app.Run();
