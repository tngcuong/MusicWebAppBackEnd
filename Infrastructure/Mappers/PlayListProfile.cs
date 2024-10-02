using AutoMapper;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.ViewModels.PlayList;
using MusicWebAppBackend.Infrastructure.ViewModels.Song;

namespace MusicWebAppBackend.Infrastructure.Mappers
{
    public class PlayListProfile : Profile
    {
        public PlayListProfile()
        {
            CreateMap<InsertPlayListDto, PLaylist>()
                .ForMember(r => r.Name, e => e.MapFrom(e => e.Name))
                .ForMember(r => r.UserId, e => e.MapFrom(e => e.UserId))
              .ForMember(x => x.Thumbnail, o => o.MapFrom(u => "https://musicwebapp.blob.core.windows.net/" + u.UserId + "/" + u.Thumbnail.FileName))
                .ForMember(r => r.IsPrivate, e => e.MapFrom(e => e.IsPrivate));

            CreateMap<PLaylist, PlayListProfileDto>()
             .ForMember(x => x.Name, o => o.MapFrom(u => u.Name))
             .ForMember(x => x.Id, o => o.MapFrom(u => u.Id))
              .ForMember(r => r.Thumbnail, e => e.MapFrom(e => e.Thumbnail))
              .ForMember(r => r.SongList, e => e.MapFrom(e => new List<SongProfileDto>()))
                .ForMember(r => r.IsPrivate, e => e.MapFrom(e => e.IsPrivate));
        }
    }
}
