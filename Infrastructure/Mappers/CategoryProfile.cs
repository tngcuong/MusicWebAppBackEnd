using AutoMapper;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.ViewModels.Category;

namespace MusicWebAppBackend.Infrastructure.Mappers
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<InsertCategoryDto, Category>()
                .ForMember(r => r.Name, e => e.MapFrom(e => e.Name))
                .ForMember(r => r.UserId, e => e.MapFrom(e => e.UserId))
              .ForMember(x => x.Img, o => o.MapFrom(u => "https://musicwebapp.blob.core.windows.net/" + u.UserId + "/" + u.Thumbnail.FileName));

            CreateMap<Category, CategoryDto>()
             .ForMember(x => x.Name, o => o.MapFrom(u => u.Name))
             .ForMember(x => x.Id, o => o.MapFrom(u => u.Id))
              .ForMember(r => r.Image, e => e.MapFrom(e => e.Img));

            CreateMap<UpdateCategoryDto, Category>()
            .ForMember(x => x.Name, o => o.MapFrom(u => u.Name));
        }
    }
}
