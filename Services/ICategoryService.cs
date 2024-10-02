using MusicWebAppBackend.Infrastructure.Mappers.Config;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.Models.Const;
using MusicWebAppBackend.Infrastructure.Models.Data;
using MusicWebAppBackend.Infrastructure.Models.Paging;
using MusicWebAppBackend.Infrastructure.ViewModels;
using MusicWebAppBackend.Infrastructure.ViewModels.Category;
using NuGet.Packaging;

namespace MusicWebAppBackend.Services
{
    public interface ICategoryService
    {
        Task<Payload<Object>> GetCategory(int pageIndex, int pageSize);
        Task<Payload<CategoryDto>> GetCategoryById(string id);
        Task<Payload<Category>> InsertACategory(InsertCategoryDto request);
        Task<Payload<CategoryDto>> InsertSongToCategory(UpdateSongToCategoryDto request);
        Task<Payload<Category>> Update(string id, UpdateCategoryDto category);
        Task<Payload<Category>> RemoveCategoryById(string id);
    }

    public class CategoryService : ICategoryService
    {
        private IRepository<Category> _categoryRepository;
        private IRepository<User> _userRepository;
        private IRepository<Song> _songRepository;
        private ISongService _songService;
        private IFileService _fileService;
        private IUserService _userService;
        public CategoryService(IRepository<Category> categoryRepository,
            IRepository<User> userRepository,
            IRepository<Song> songRepository,
            ISongService songService,
            IFileService fileService,
            IUserService userService)
        {
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
            _songRepository = songRepository;
            _fileService = fileService;
            _songService = songService;
            _userService = userService;
        }

        public async Task<Payload<object>> GetCategory(int pageIndex, int pageSize)
        {
            var qure = (from s in _categoryRepository.Table
                        where s.IsDeleted == false
                        select new CategoryDto
                        {
                            Id = s.Id,
                            Image = s.Img,
                            Name = s.Name,
                            CreateById = s.UserId,
                            CreateAt = s.CreatedAt,
                        }).ToList();

            if (!qure.Any())
            {
                return Payload<object>.NoContent();
            }

            foreach (var listItem in qure)
            {
                var creator = await _userService.GetUserById(listItem.CreateById);
                if (creator.Content != null)
                {
                    listItem.CreateBy = creator.Content;
                }

                var cate = await _categoryRepository.GetByIdAsync(listItem.Id);
                if (cate != null)
                {
                    foreach (var item in cate.Songs)
                    {
                        var song = await _songService.GetById(item);
                        if (song.Content != null)
                        {
                            listItem.SongList.Add(song.Content);
                        }
                    }
                }
            }

            var pageList = await PageList<CategoryDto>.Create(qure.AsQueryable(), pageIndex, pageSize);

            if (pageList.Count == 0)
            {
                return Payload<Object>.NotFound(PlayListResource.NOALBUMFOUND);
            }

            return Payload<Object>.Successfully(new
            {
                Data = pageList,
                PageIndex = pageIndex,
                Total = qure.Count(),
                TotalPages = pageList.totalPages
            }, PlayListResource.GETSUCCESS);
        }

        public async Task<Payload<CategoryDto>> GetCategoryById(string id)
        {
            try
            {
                var cate = await _categoryRepository.GetByIdAsync(id);
                if (cate == null)
                {
                    return Payload<CategoryDto>.NotFound();
                }

                var dto = cate.MapTo<Category, CategoryDto>();
                return Payload<CategoryDto>.Successfully(dto);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<Payload<Category>> InsertACategory(InsertCategoryDto request)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return Payload<Category>.NotFound(UserResource.NOUSERFOUND);
            }

            var imgFile = await _fileService.SetImage(request.Thumbnail, request.UserId);
            if (imgFile.Length < 1 || imgFile == null || imgFile is EmptyFormFile)
            {
                return Payload<Category>.BadRequest(FileResource.IMAGEFVALID);
            }

            request.Thumbnail = imgFile;

            Category playList = request.MapTo<InsertCategoryDto, Category>();
            await _categoryRepository.InsertAsync(playList);
            return Payload<Category>.Successfully(playList, PlayListResource.CREATESUCCESS);
        }

        public async Task<Payload<CategoryDto>> InsertSongToCategory(UpdateSongToCategoryDto request)
        {
            try
            {
                var cate = await _categoryRepository.GetByIdAsync(request.IdCate);
                cate.Songs.Clear();
                cate.Songs.AddRange(request.IdSong);
                await _categoryRepository.UpdateAsync(cate);

                CategoryDto playListDto = cate.MapTo<Category, CategoryDto>();
                foreach (var item in _categoryRepository.GetByIdAsync(request.IdCate).Result.Songs)
                {
                    playListDto.SongList.Add(_songService.GetById(item).Result.Content);
                }
                return Payload<CategoryDto>.Successfully(playListDto);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Payload<Category>> RemoveCategoryById(string id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    return Payload<Category>.NotFound(SongResource.NOSONGFOUND);
                }

                category.IsDeleted = true;
                await _categoryRepository.UpdateAsync(category);
                return Payload<Category>.Successfully(category, SongResource.DELETESUCCESS);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public async Task<Payload<Category>> Update(string id, UpdateCategoryDto category)
        {
            try
            {
                var entity = await _categoryRepository.GetByIdAsync(id);
                if (entity == null)
                {
                    return Payload<Category>.NotFound();
                }

                var cateUpdate = category.MapTo<UpdateCategoryDto, Category>(entity);

                await _categoryRepository.UpdateAsync(cateUpdate);
                return Payload<Category>.Successfully(cateUpdate);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
