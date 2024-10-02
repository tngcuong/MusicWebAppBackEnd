using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicWebAppBackend.Infrastructure.ViewModels.Category;
using MusicWebAppBackend.Services;

namespace MusicWebAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [Route(nameof(GetAllCategory))]
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> GetAllCategory(int pageIndex, int pageSize)
        {
            var data = await _categoryService.GetCategory(pageIndex, pageSize);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(GetCategoryById))]
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> GetCategoryById(string id)
        {
            var data = await _categoryService.GetCategoryById(id);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(RemoveCategoryById))]
        [HttpDelete]
        public async Task<ActionResult> RemoveCategoryById(string id)
        {
            var data = await _categoryService.RemoveCategoryById(id);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(Update))]
        [HttpPut]
        public async Task<ActionResult> Update(string id, UpdateCategoryDto cate)
        {
            var data = await _categoryService.Update(id, cate);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(InsertACategory))]
        [HttpPost]
        public async Task<ActionResult> InsertACategory([FromForm] InsertCategoryDto cate)
        {
            var data = await _categoryService.InsertACategory(cate);
            return StatusCode((int)data.ErrorCode, data);
        }

    }
}
