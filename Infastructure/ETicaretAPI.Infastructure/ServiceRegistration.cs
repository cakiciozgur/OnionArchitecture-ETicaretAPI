using ETicaretAPI.Application.Abstractions.Services.Configurations;
using ETicaretAPI.Application.Abstractions.Services.Mail;
using ETicaretAPI.Application.Abstractions.Services.QRCode;
using ETicaretAPI.Application.Abstractions.Storage;
using ETicaretAPI.Application.Abstractions.Token;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Infastructure.Enums;
using ETicaretAPI.Infastructure.Services;
using ETicaretAPI.Infastructure.Services.Configurations;
using ETicaretAPI.Infastructure.Services.Mail;
using ETicaretAPI.Infastructure.Services.QRCode;
using ETicaretAPI.Infastructure.Services.Storage;
using ETicaretAPI.Infastructure.Services.Storage.Azure;
using ETicaretAPI.Infastructure.Services.Storage.Local;
using ETicaretAPI.Infastructure.Services.Token;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IStorageService, StorageService>();
            services.AddScoped<ITokenHandler, TokenHandler>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IApplicationService, ApplicationService>();
            services.AddScoped<IQRCodeService, QRCodeService>();
        }

        public static void AddStorage<T>(this IServiceCollection services) where T : Storage , IStorage
        {
            services.AddScoped<IStorage, T>();
        }

        //public static void AddStorage(this IServiceCollection services, StorageType storageType)
        //{
        //    switch (storageType)
        //    {
        //        case StorageType.Local:
        //            services.AddScoped<IStorage, LocalStorage>();
        //            break;
        //        case StorageType.Azure:
        //            services.AddScoped<IStorage, AzureStorage>();
        //            break;
        //        case StorageType.AWS:
        //            //aws storage eklenebilir.
        //            break;
        //        default:
        //            services.AddScoped<IStorage, LocalStorage>();
        //            break;
        //    }
        //}
    }
}