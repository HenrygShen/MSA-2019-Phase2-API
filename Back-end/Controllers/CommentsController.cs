using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Back_end.DAL;
using Back_end.Model;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Back_end.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowOrigin")]
    public class CommentsController : ControllerBase
    {
        private ICommentsRepository _commentsRepository;

        public CommentsController(ICommentsRepository commentsRepository)
        {
            _commentsRepository = commentsRepository;
        }

        /// <summary>
        /// Retrieves all comments for the specified videoId.
        /// </summary>
        [HttpGet("{videoId}")]
        public async Task<ActionResult<IEnumerable<Comments>>> GetComments(int videoId)
        {
            var comments = await _commentsRepository.GetComments(videoId);
            return Ok(comments);
        }

        /// <summary>
        /// Adds a comment to the specified video.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody]CommentsDTO commentInfo)
        {
            bool added = await _commentsRepository.AddComment(commentInfo);
            if (!added)
                return BadRequest(new { message = "Failed to save comment" });

            return Ok(new { message = "Comment saved" });
        }

        /// <summary>
        /// Updates likes, either adding a like or removing a like from the specified comment.
        /// </summary>
        [HttpPut("UpdateLikes")]
        public async Task<IActionResult> UpdateLikes([FromBody]LikesDTO commentInfo)
        {
            bool updated = await _commentsRepository.UpdateLikes(commentInfo.CommentId, commentInfo.UserId, commentInfo.Like);
            if (!updated)
                return BadRequest(new { message = "Failed to update likes" });

            return Ok(new { message = "Likes updated" });
        }

        /// <summary>
        /// Deletes comment by its commentId. 
        /// </summary>
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            bool deleted = await _commentsRepository.DeleteComment(commentId);
            if (!deleted)
                return BadRequest(new { message = "Failed to delete comment" });

            return Ok(new { message = "Comment deleted" });
        }
    }
}
