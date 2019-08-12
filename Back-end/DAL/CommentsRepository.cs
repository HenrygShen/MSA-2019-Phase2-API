using Back_end.Model;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Back_end.DAL
{
    public interface ICommentsRepository
    {
        Task<IEnumerable<Comments>> GetComments(int videoId);
        Task<bool> AddComment(CommentsDTO commentInfo);
        Task<bool> DeleteComment(int commentId);
        Task<bool> UpdateComment(int commentId, string comment);
        Task<bool> UpdateLikes(int commentId, int userId, int numLikes, bool like, string likesList);
    }
    public class CommentsRepository : ICommentsRepository
    {
        private readonly string connectionString;

        public CommentsRepository()
        {
            this.connectionString = "Server=tcp:scriber-hgs.database.windows.net,1433;Initial Catalog=scriber;Persist Security Info=False;User ID=admin-hgs;Password=scriber-7890;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        }

        public async Task<IEnumerable<Comments>> GetComments(int videoId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var comments = connection.QueryAsync<Comments>("Select * from Comments where VideoId=@videoId", new { videoId }).Result;
                    // Check for , returns null if user not found
                    return comments;
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> AddComment(CommentsDTO commentInfo)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    await connection.ExecuteAsync(@"insert into Comments (VideoId, Comment, Likes, LikesList, TimeStamp, Edited) 
                                                        values(@videoId, @comment, @likes, @likesList, @timeStamp, 0)", 
                                                        new { commentInfo.VideoId, commentInfo.Comment, commentInfo.Likes,
                                                        commentInfo.LikesList, commentInfo.TimeStamp });
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteComment(int commentId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    await connection.ExecuteAsync("Delete Comments where CommentId=@commentId)", new { commentId });
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateComment(int commentId, string comment)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var affectedRows = connection.ExecuteAsync("UPDATE Comments SET Comment = @comment, Edited = 1 WHERE CommentId=@commentId", new { comment, commentId }).Result;
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateLikes(int commentId, int userId, int numLikes, bool like, string likesList)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    if (like)
                    {
                        // Add to likes
                        numLikes++;

                        // Add userId to likesList
                        string newUser = userId.ToString() + "-";
                        likesList += newUser;
                    }
                    else
                    {
                        // Remove from likes
                        numLikes--;

                        // Remove userId from likesList
                        string oldUser = userId.ToString() + "-";
                        likesList = likesList.Replace(oldUser,"");
                    }
                    await connection.ExecuteAsync("UPDATE Comments SET Likes = @likes, LikesList = @likesList WHERE CommentId=@commentId", new { likes = numLikes, likesList, commentId });

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
