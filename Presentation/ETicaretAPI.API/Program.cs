using ETicaretAPI.Application;
using ETicaretAPI.Application.Validators.Products;
using ETicaretAPI.Infastructure;
using ETicaretAPI.Infastructure.Enums;
using ETicaretAPI.Infastructure.Services.Storage.Local;
using ETicaretAPI.Persistence;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistenceServices();
builder.Services.AddInfastructureServices();
builder.Services.AddStorage<LocalStorage>(); // => or builder.Services.AddStorage<AzureStorage>(); builder.Services.AddStorage(StorageType.Azure); // enum kullanmak yerine generic yapýlanma kullanma daha mantýklý
builder.Services.AddApplicationServices();
builder.Services.AddControllers();
builder.Services.AddFluentValidationServices();

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
        };
    }); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy=> policy.WithOrigins("http://localhost:4200", "http://localhost:4200").AllowAnyHeader().AllowAnyMethod()));
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseCors();
app.UseHttpsRedirection();

app.UseAuthentication(); // authorize için eklenmeli

app.UseAuthorization();

app.MapControllers();

app.Run();
