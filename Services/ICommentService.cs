using MusicWebAppBackend.Infrastructure.Mappers.Config;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.Models.Const;
using MusicWebAppBackend.Infrastructure.Models.Data;
using MusicWebAppBackend.Infrastructure.Models.Paging;
using MusicWebAppBackend.Infrastructure.ViewModels;
using MusicWebAppBackend.Infrastructure.ViewModels.Comment;

namespace MusicWebAppBackend.Services
{
    public interface ICommentService
    {
        Task<Payload<Comment>> AddComment(AddCommentDto request);
        Task<Payload<IList<CommentDto>>> GetAllCommentBySongId(string id);
        Task<Payload<CommentDto>> ToggleApproveCommentById(string id);
        Task<Payload<Object>> GetAllApprovedComment(int pageIndex, int pageSize);
        Task<Payload<Object>> GetAllunApprovedComment(int pageIndex, int pageSize);
        Task<Payload<Comment>> DeleteCommentById(string id);
    }
    public class CommentService : ICommentService
    {
        private readonly IRepository<Comment> _commentRepositoty;
        private readonly IRepository<User> _userRepositoty;
        private readonly IRepository<Song> _songRepositoty;
        private readonly IUserService _userService;
        public CommentService(IRepository<Comment> commentRepositoty,
            IRepository<Song> songRepositoty,
            IRepository<User> userRepositoty,
        IUserService userService)
        {
            _commentRepositoty = commentRepositoty;
            _songRepositoty = songRepositoty;
            _userRepositoty = userRepositoty;
            _userService = userService;
        }

        public async Task<Payload<Comment>> AddComment(AddCommentDto request)
        {
            try
            {
                var comment = request.MapTo<AddCommentDto, Comment>();
                var result = await _commentRepositoty.InsertAsync(comment);
                return Payload<Comment>.Successfully(result, CommentResource.CREATESUCCESS);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public async Task<Payload<Comment>> DeleteCommentById(string id)
        {
            try
            {
                var comment = await _commentRepositoty.GetByIdAsync(id);
                if (comment == null)
                    return Payload<Comment>.NoContent();

                comment.IsDeleted = true;
                await _commentRepositoty.UpdateAsync(comment);

                return Payload<Comment>.Successfully(comment);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<Payload<Object>> GetAllApprovedComment(int pageIndex, int pageSize)
        {

            var listComment = (from c in _commentRepositoty.Table
                               join u in _userRepositoty.Table on c.UserId equals u.Id
                               where c.IsDeleted == false && c.IsApproved == true && u.IsDeleted == false
                               orderby c.CreateAt descending
                               select new CommentDto
                               {
                                   Id = c.Id,
                                   SongId = c.SongId,
                                   IsDeleted = c.IsDeleted,
                                   Content = c.Content,
                                   CreateAt = c.CreateAt,
                                   IsApproved = c.IsApproved,
                                   UserId = c.UserId,
                               }).ToList();

            foreach (var comment in listComment)
            {
                var user = await _userService.GetUserById(comment.UserId);
                if (user.Content != null)
                {
                    comment.User = user.Content;
                }
            }

            var pageList = await PageList<CommentDto>.Create(listComment.AsQueryable(), pageIndex, pageSize);

            if (pageList.Count == 0)
            {
                return Payload<Object>.NoContent();
            }

            return Payload<Object>.Successfully(new
            {
                Data = pageList,
                PageIndex = pageIndex,
                Total = listComment.Count(),
                TotalPages = pageList.totalPages
            });
        }

        public async Task<Payload<IList<CommentDto>>> GetAllCommentBySongId(string id)
        {
            var song = await _songRepositoty.GetByIdAsync(id);
            if (song == null)
                return Payload<IList<CommentDto>>.NoContent();

            var listComment = (from c in _commentRepositoty.Table
                               join u in _userRepositoty.Table on c.UserId equals u.Id
                               where c.IsDeleted == false && c.SongId == id && u.IsDeleted == false
                               orderby c.CreateAt descending
                               select new CommentDto
                               {
                                   Id = c.Id,
                                   SongId = c.SongId,
                                   IsDeleted = c.IsDeleted,
                                   Content = c.Content,
                                   CreateAt = c.CreateAt,
                                   IsApproved = c.IsApproved,
                                   UserId = c.UserId,
                               }).ToList();

            foreach (var comment in listComment)
            {
                var user = await _userService.GetUserById(comment.UserId);
                if (user.Content != null)
                {
                    comment.User = user.Content;
                }
            }
            return Payload<IList<CommentDto>>.Successfully(listComment);
        }

        public async Task<Payload<Object>> GetAllunApprovedComment(int pageIndex, int pageSize)
        {
            var listComment = (from c in _commentRepositoty.Table
                               join u in _userRepositoty.Table on c.UserId equals u.Id
                               where c.IsDeleted == false && c.IsApproved == false && u.IsDeleted == false
                               orderby c.CreateAt descending
                               select new CommentDto
                               {
                                   Id = c.Id,
                                   SongId = c.SongId,
                                   IsDeleted = c.IsDeleted,
                                   Content = c.Content,
                                   CreateAt = c.CreateAt,
                                   IsApproved = c.IsApproved,
                                   UserId = c.UserId,
                               }).ToList();

            foreach (var comment in listComment)
            {
                var user = await _userService.GetUserById(comment.UserId);
                if (user.Content != null)
                {
                    comment.User = user.Content;
                }
            }

            var pageList = await PageList<CommentDto>.Create(listComment.AsQueryable(), pageIndex, pageSize);

            if (pageList.Count == 0)
            {
                return Payload<Object>.NoContent();
            }

            return Payload<Object>.Successfully(new
            {
                Data = pageList,
                PageIndex = pageIndex,
                Total = listComment.Count(),
                TotalPages = pageList.totalPages
            });
        }

        public async Task<Payload<CommentDto>> ToggleApproveCommentById(string id)
        {
            var comment = await _commentRepositoty.GetByIdAsync(id);
            if (comment == null)
            {
                return Payload<CommentDto>.NoContent();
            }

            if (comment.IsApproved == false)
            {
                comment.IsApproved = true;
                await _commentRepositoty.UpdateAsync(comment);
            }
            else
            {
                comment.IsApproved = false;
                await _commentRepositoty.UpdateAsync(comment);
            }

            var result = comment.MapTo<Comment, CommentDto>();
            return Payload<CommentDto>.Successfully(result);
        }
    }
}
