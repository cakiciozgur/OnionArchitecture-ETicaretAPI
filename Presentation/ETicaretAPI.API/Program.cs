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

builder.Services.AddHttpContextAccessor(); // clienttan gelen request ile oluþturulan httpcontext nesnesine katmanlardaki classlar üzerinden (business logic) eriþebilmemizi saðlayan bir servistir.

builder.Services.AddPersistenceServices();
builder.Services.AddInfastructureServices();
builder.Services.AddStorage<LocalStorage>(); // => or builder.Services.AddStorage<AzureStorage>(); builder.Services.AddStorage(StorageType.Azure); // enum kullanmak yerine generic yapýlanma kullanma daha mantýklý
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
    columnOptions: new Dictionary<string, ColumnWriterBase> /*columnOptions => eger default olanlardan farklý kolonlar eklemek istiyorsak var olanlarýda ekleyip onun üzerine ÇALIÞMA YAPMALIYIZ*/
    {
        { "message" , new RenderedMessageColumnWriter() }, /*RenderedMessageColumnWriter çalýþmasý sonucu gelen veri message kolonuna yazýlacak*/
        { "message_template" , new MessageTemplateColumnWriter() },
        { "level" , new LevelColumnWriter() },
        { "time_stamp" , new TimestampColumnWriter() },
        { "exception" , new ExceptionColumnWriter() },        
        { "log_event" , new LogEventSerializedColumnWriter() },        
        { "user_name" ,  new UsernameColumnWriter() },        
    })
    .WriteTo.Seq(builder.Configuration["Seq:ServerUrl"])// seq görsel arayüzüne yazdýrýlsýn.
    .Enrich.FromLogContext() // log contextten beslenmesi gerektiðini buraya(context'e) veriler eklediðimizi bildiriyoruz.
    .MinimumLevel.Information()
    .CreateLogger();

builder.Host.UseSerilog(logger);

builder.Services.AddHttpLogging(logging => // YAPILAN HTTP REQUESTLERÝ ÝÇÝN CONFÝGURATÝON AYARLARI => app.userhttplogging; eklediðimizde tüm request ve responslerý logluyor olacaðýz
{
    logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
    logging.RequestHeaders.Add("sec-ch-ua");
    logging.ResponseHeaders.Add("E-Commerce-API");
    logging.MediaTypeOptions.AddText("application/json");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Admin",options =>// bir token ile istek geldiðinde bunu jwt olarak deðerlendir.
    {
        options.TokenValidationParameters = new() // jwt token içerisinde doðrulanmasý gereken verileri belirtiyoruz
        {
            ValidateIssuer = true, // oluþturulacak tokenin kimin / hangi originin/sitelerin kullanacaðýný belirleriz. => www.xyz.com
            ValidateAudience = true, //  oluþturulacak token deðerini kimin daðýttýðýný belirtir. => www.xyzapi.com
            ValidateLifetime = true, // token süresini kontrol eder
            ValidateIssuerSigningKey = true, // üretilecek olan tokenin uygulamamýza ait bir deðer olduðunu ifade eden secury key verisinin doðrulanmasýdýr,

            /*doðrulanacak veriler*/
            ValidAudience = builder.Configuration["Token:Audience"],
            ValidIssuer = builder.Configuration["Token:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
            LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow : false,

            NameClaimType = ClaimTypes.Name /*jwt üzerinde name claimine karþýlýk gelen deðerini  User.Identity.Name  propertysi üzerinden elde edebilmek için eklendi*/,
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
app.UseSerilogRequestLogging(); // UseSerilogRequestLogging koyulan satýrdan önceki iþlemler loglanmaz dolayýsýyla koyduðumuz yer kritik öneme sahiptir.

app.UseHttpLogging(); // tüm http isteklerini loglama için eklendi.
app.UseCors();
app.UseHttpsRedirection();

app.UseAuthentication(); // authorize için eklenmeli

app.UseAuthorization();

app.Use(async (context, next) =>
{
    var username = context.User?.Identity?.IsAuthenticated != null || true ? context.User.Identity.Name : null;

    LogContext.PushProperty("user_name", username); // log context static yapýlanmasý sayesinde log contextine ekliyoruz.

    await next();
});

app.MapControllers();
app.MapHubs();
app.Run();
