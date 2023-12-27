﻿using ETicaretAPI.Persistence.Contexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ETicaretAPI.Persistence.Repositories;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.Validators.Products;
using FluentValidation.AspNetCore;
using FluentValidation;
using ETicaretAPI.Infastructure.Filters;

namespace ETicaretAPI.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceServices(this IServiceCollection services)
        {
            services.AddDbContext<ETicaretAPIDbContext>(options => options.UseNpgsql(Configurations.ConnectionString));
            services.AddScoped<ICustomerReadRepository, CustomerReadRepository>();
            services.AddScoped<ICustomerWriteRepository, CustomerWriteRepository>();
            services.AddScoped<IProductReadRepository, ProductReadRepository>();
            services.AddScoped<IProductWriteRepository, ProductWriteRepository>();
            services.AddScoped<IOrderReadRepository, OrderReadRepository>();
            services.AddScoped<IOrderWriteRepository, OrderWriteRepository>();
        }

        public static void  AddFluentValidationServices(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            services.AddMvc(x => x.Filters.Add<ValidationFilter>());

            services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();
            /* *****eklenmezse***** yapılan istekler controller'a ulaşmadan doğrulamaları içeren bir body şeklinde mesaj döndürür.*/
            services.AddMvc().ConfigureApiBehaviorOptions(option => option.SuppressModelStateInvalidFilter = true);
        }
    }
}
