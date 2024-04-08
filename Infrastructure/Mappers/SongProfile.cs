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
                .ForMember(x => x.UserId, o => o.MapFrom(u => u.UserId))
                .ForMember(x => x.DurationTime, o => o.MapFrom(u => u.DurationTime));

            CreateMap<Song, SongProfileDto>()
                .ForMember(x => x.Source, o => o.MapFrom(u => u.Source))
                .ForMember(x => x.Image, o => o.MapFrom(u => u.Img))
               .ForMember(x => x.Name, o => o.MapFrom(u => u.Name))
               .ForMember(x => x.User, o => o.MapFrom(u => u.UserId))
               .ForMember(x => x.Id, o => o.MapFrom(u => u.Id))
                .ForMember(x => x.DurationTime, o => o.MapFrom(u => u.DurationTime));
        }
    }
}
