using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extension
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddAplicationServices(this IServiceCollection services, IConfiguration config)
        {
                services.AddDbContext<DataContext>(options => {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo  { Title = "API", Version = "V1"});
            });
            services.AddScoped<ITokenService,TokenService>();

            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

            services.AddScoped<IUserRepository,UserRepository>();
            return services;
        }
    }
}