using AutoMapper;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.ViewModels.Song;
using MusicWebAppBackend.Infrastructure.ViewModels.User;

namespace MusicWebAppBackend.Infrastructure.Mappers
{
    public class SongProfile : Profile
    {
        public SongProfile()
        {
            CreateMap<SongInsertDto, Song>()
                 .ForMember(x => x.Source, o => o.MapFrom(u => "https://musicwebapp.blob.core.windows.net/" + u.UserId + "/" + u.Source.FileName))
                 .ForMember(x => x.Img, o => o.MapFrom(u => "https://musicwebapp.blob.core.windows.net/" + u.UserId + "/" + u.Img.FileName))
                .ForMember(x => x.Name, o => o.MapFrom(u => u.Name))
                .ForMember(x => x.UserId, o => o.MapFrom(u => u.UserId));
        }
    }
}
