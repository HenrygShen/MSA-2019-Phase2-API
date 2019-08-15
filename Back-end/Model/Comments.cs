using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Back_end.Model
{
    public class CommentsDTO
    {
        public int? VideoId { get; set; }
        public string Comment { get; set; }
        public int Likes { get; set; }
        public string LikesList { get; set; }
        public string TimeStamp { get; set; }
        public bool Edited { get; set; }
        public string Username { get; set; }
    }

    public class LikesDTO
    {
        public int CommentId { get; set; }
        public int UserId { get; set; }
        public bool Like { get; set; }
    }
    public class Comments
    {
        [Key]
        public int CommentId { get; set; }
        public int? VideoId { get; set; }
        public string Comment { get; set; }
        public int Likes { get; set; }
        public string LikesList { get; set; }
        public string TimeStamp { get; set; }
        public bool Edited { get; set; }
        public string Username { get; set; }
    }
}
