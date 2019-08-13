using Back_end.Model;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Back_end.DAL
{
    public interface IVideoRepository : IDisposable
    {

        Task<IEnumerable<Video>> GetVideos();
        Task<Video> GetVideoByID(int VideoId);
        void InsertVideo(Video video);
        Task<bool> DeleteVideo(int VideoId);
        Task<bool> UpdateVideo(Video video);
        void Save();
    }
    public class VideoRepository : IVideoRepository, IDisposable
    {
        private scriberContext context;
        private readonly string connectionString;

        public VideoRepository(scriberContext context)
        {
            this.context = context;
            this.connectionString = "Server=tcp:scriber-hgs.database.windows.net,1433;Initial Catalog=scriber;Persist Security Info=False;User ID=admin-hgs;Password=scriber-7890;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        }

        public async Task<IEnumerable<Video>> GetVideos()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var video = connection.QueryAsync<Video>("Select * from Video").Result;
                    return video;
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<Video> GetVideoByID(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var video = connection.QuerySingleOrDefaultAsync<Video>("Select * from Video where videoId=@id", new { id }).Result;
                    return video;
                }
            }
            catch
            {
                return null;
            }
            //return context.Video.Find(id);
        }

        public void InsertVideo(Video video)
        {
            context.Video.Add(video);
        }

        public async Task<bool> DeleteVideo(int videoId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    await connection.ExecuteAsync("Delete Video where VideoId=@videoId)", new { videoId });
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateVideo(Video video)
        {

            context.Entry(video).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
