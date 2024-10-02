using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicWebAppBackend.Infrastructure.ViewModels.Comment;
using MusicWebAppBackend.Services;

namespace MusicWebAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [Route(nameof(AddComment))]
        [HttpPost]
        public async Task<ActionResult> AddComment(AddCommentDto request)
        {
            var data = await _commentService.AddComment(request);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(GetAllCommentBySongId))]
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> GetAllCommentBySongId(string id)
        {
            var data = await _commentService.GetAllCommentBySongId(id);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(GetAllApprovedComment))]
        [HttpGet]
        public async Task<ActionResult> GetAllApprovedComment(int pageIndex, int pageSize)
        {
            var data = await _commentService.GetAllApprovedComment(pageIndex, pageSize);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(GetAllUnApprovedComment))]
        [HttpGet]
        public async Task<ActionResult> GetAllUnApprovedComment(int pageIndex, int pageSize)
        {
            var data = await _commentService.GetAllunApprovedComment(pageIndex, pageSize);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(DeleteCommentById))]
        [HttpDelete]
        public async Task<ActionResult> DeleteCommentById(string id)
        {
            var data = await _commentService.DeleteCommentById(id);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(ToggleApproveCommentById))]
        [HttpPut]
        public async Task<ActionResult> ToggleApproveCommentById(string id)
        {
            var data = await _commentService.ToggleApproveCommentById(id);
            return StatusCode((int)data.ErrorCode, data);
        }
    }
}
