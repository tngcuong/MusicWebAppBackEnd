using AutoMapper;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.ViewModels.Account;

namespace MusicWebAppBackend.Infrastructure.Mappers.Config
{
    public static class MappingExtensions
    {
        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return AutoMapperConfig.Mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return AutoMapperConfig.Mapper.Map(source, destination);
        }
    }

    public class AvatarUrlResolver : IValueResolver<UpdateAccountDto, User, string>
    {
        public string Resolve(UpdateAccountDto source, User destination, string destMember, ResolutionContext context)
        {
            if (source.Avatar != null)
            {
                return "https://musicwebapp.blob.core.windows.net/" + source.Id + "/" + source.Avatar.FileName;
            }

            // Return the existing value if Avatar is null
            return destination.Avatar;
        }
    }
}
