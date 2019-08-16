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
        Task<bool> UpdateLikes(int commentId, int userId, bool like);
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
                    await connection.ExecuteAsync(@"insert into Comments (VideoId, Comment, Likes, LikesList, TimeStamp, Edited, Username) 
                                                        values(@videoId, @comment, @likes, @likesList, @timeStamp, 0, @username)", 
                                                        commentInfo);
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
                    await connection.ExecuteAsync("Delete Comments where CommentId=@commentId", new { commentId });
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }


        public async Task<bool> UpdateLikes(int commentId, int userId, bool like)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string likesList = connection.QueryFirstOrDefault<string>("Select LikesList from Comments where CommentId = @commentId", new { commentId });
                    int likes = connection.QueryFirstOrDefault<int>("Select Likes from Comments where CommentId = @commentId", new { commentId });
                    if (like)
                    {
                        // Add to likes
                        likes++;

                        // Add userId to likesList
                        string newUser = userId.ToString() + "-";
                        likesList += newUser;
                    }
                    else
                    {
                        // Remove from likes
                        likes--;

                        // Remove userId from likesList
                        string oldUser = userId.ToString() + "-";
                        likesList = likesList.Replace(oldUser,"");
                    }
                    await connection.ExecuteAsync("UPDATE Comments SET Likes = @likes, LikesList = @likesList WHERE CommentId=@commentId", new { likes, likesList, commentId });

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
