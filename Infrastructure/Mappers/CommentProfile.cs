using AutoMapper;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.ViewModels.Comment;

namespace MusicWebAppBackend.Infrastructure.Mappers
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, AddCommentDto>()
              .ForMember(x => x.UserId, o => o.MapFrom(u => u.UserId))
              .ForMember(x => x.Content, o => o.MapFrom(u => u.Content))
              .ForMember(x => x.SongId, o => o.MapFrom(u => u.SongId));

            CreateMap<AddCommentDto, Comment>()
                .ForMember(x => x.UserId, o => o.MapFrom(u => u.UserId))
                .ForMember(x => x.Content, o => o.MapFrom(u => u.Content))
                .ForMember(x => x.SongId, o => o.MapFrom(u => u.SongId));

            CreateMap<CommentDto, Comment>()
              .ForMember(x => x.Id, o => o.MapFrom(u => u.Id))
              .ForMember(x => x.UserId, o => o.MapFrom(u => u.UserId))
              .ForMember(x => x.Content, o => o.MapFrom(u => u.Content))
              .ForMember(x => x.IsApproved, o => o.MapFrom(u => u.IsApproved))
              .ForMember(x => x.IsDeleted, o => o.MapFrom(u => u.IsDeleted))
              .ForMember(x => x.CreateAt, o => o.MapFrom(u => u.CreateAt))
              .ForMember(x => x.SongId, o => o.MapFrom(u => u.SongId));

            CreateMap<Comment, CommentDto>()
             .ForMember(x => x.Id, o => o.MapFrom(u => u.Id))
             .ForMember(x => x.UserId, o => o.MapFrom(u => u.UserId))
             .ForMember(x => x.Content, o => o.MapFrom(u => u.Content))
             .ForMember(x => x.IsApproved, o => o.MapFrom(u => u.IsApproved))
             .ForMember(x => x.IsDeleted, o => o.MapFrom(u => u.IsDeleted))
             .ForMember(x => x.CreateAt, o => o.MapFrom(u => u.CreateAt))
             .ForMember(x => x.SongId, o => o.MapFrom(u => u.SongId));
        }
    }
}
