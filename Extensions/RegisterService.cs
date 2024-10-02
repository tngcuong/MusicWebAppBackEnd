using AutoMapper;
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
            services.AddHttpContextAccessor();
            services.AddDistributedMemoryCache();
            services.AddScoped(typeof(IRepository<>), typeof(MongoRepository<>));
            services.AddScoped<ConfigEmail>();
            services.AddScoped<IPlayListService, PlayListService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ISongService, SongService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ILikedSongService, LikedSongService>();
            services.AddScoped<ILikedPlayListService, LikedPlayList>();
            services.AddScoped<ICommentService, CommentService>();
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

            AutoMapperConfig.Init(config);
            return services;
        }


    }
}
