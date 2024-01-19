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
builder.Services.AddStorage<LocalStorage>(); // => or builder.Services.AddStorage<AzureStorage>(); builder.Services.AddStorage(StorageType.Azure); // enum kullanmak yerine generic yap�lanma kullanma daha mant�kl�
builder.Services.AddApplicationServices();
builder.Services.AddControllers();
builder.Services.AddFluentValidationServices();

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

app.UseAuthentication(); // authorize i�in eklenmeli

app.UseAuthorization();

app.MapControllers();

app.Run();
