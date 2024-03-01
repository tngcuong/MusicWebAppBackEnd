using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MusicWebAppBackend.Infrastructure.Helpers;
using MusicWebAppBackend.Infrastructure.Mappers.Config;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.Models.Data;
using MusicWebAppBackend.Services;
using System.Reflection;

namespace MusicWebAppBackend.Extensions
{
    public static class RegisterService
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(IRepository<>), typeof(MongoRepository<>));
            services.AddHttpContextAccessor();
            services.AddScoped<ConfigEmail>();
            services.AddScoped<IFileService,FileService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ISongService, SongService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<DbContext>();
            return services;
        }

        public static IServiceCollection RegisterAutoMapper(this IServiceCollection services, ConfigurationManager configuration)
        {
            var profiles = Assembly.GetExecutingAssembly().GetTypes()
                                  .Where(t => typeof(Profile).IsAssignableFrom(t) && !t.IsAbstract)
                                  .Select(Activator.CreateInstance)
                                  .Cast<Profile>();

            var config = new MapperConfiguration(cfg =>
            {
                foreach (var instance in profiles)
                {
                    cfg.AddProfile(instance.GetType());
                }
            });

            //register automapper
            AutoMapperConfig.Init(config);
            return services;
        }


    }
}
