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
using ETicaretAPI.Domain.Entities.Identity;
using ETicaretAPI.Application.Abstractions.Services.User;
using ETicaretAPI.Persistence.Services;
using ETicaretAPI.Application.Abstractions.Services.Auth;
using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.Abstractions.Services.Order;
using Microsoft.AspNetCore.Identity;
using ETicaretAPI.Application.Abstractions.Services.Role;
using ETicaretAPI.Application.Abstractions.Services.AuthorizationEndpoint;
using ETicaretAPI.Application.Abstractions.Services.Product;

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
            services.AddScoped<IFileReadRepository, FileReadRepository>();
            services.AddScoped<IFileWriteRepository, FileWriteRepository>();
            services.AddScoped<IProductImageFileReadRepository, ProductImageFileReadRepository>();
            services.AddScoped<IProductImageFileWriteRepository, ProductImageFileWriteRepository>();
            services.AddScoped<IInvoiceFileReadRepository, InvoiceFileReadRepository>();
            services.AddScoped<IInvoiceFileWriteRepository, InvoiceFileWriteRepository>();

            services.AddScoped<IBasketItemReadRepository, BasketItemReadRepository>();
            services.AddScoped<IBasketItemWriteRepository, BasketItemWriteRepository>();
            services.AddScoped<IBasketReadRepository, BasketReadRepository>();
            services.AddScoped<IBasketWriteRepository, BasketWriteRepository>();

            services.AddScoped<ICompletedOrderReadRepository, CompletedOrderReadRepository>();
            services.AddScoped<ICompletedOrderWriteRepository, CompletedOrderWriteRepository>();

            services.AddScoped<IMenuReadRepository, MenuReadRepository>();
            services.AddScoped<IMenuWriteRepository, MenuWriteRepository>();

            services.AddScoped<IEndpointReadRepository, EndpointReadRepository>();
            services.AddScoped<IEndpointWriteRepository, EndpointWriteRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IInternalAuth, AuthService>();
            services.AddScoped<IExternalAuth, AuthService>();
            services.AddScoped<IBasketService, BasketService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAuthorizationEndpointService, AuthorizationEndpointService>();
            services.AddScoped<IProductService, ProductService>();

            services.AddIdentity<AppUser,AppRole>(options => {
                options.Password.RequiredLength = 3;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@";
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ETicaretAPIDbContext>() // identity mekanizmasını devreye alıp => ETicaretAPIDbContext üzerinde store işlemlerinin yapılmasını bildirdik
            .AddDefaultTokenProviders(); // reset password token almak için gereken servis
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
