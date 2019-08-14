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
    public interface IVideoRepository
    {

        Task<IEnumerable<Video>> GetVideos();
        Task<Video> GetVideoByID(int VideoId);
        Task<bool> AddVideo(Video video);
        Task<bool> DeleteVideo(int VideoId);
        Task<bool> UpdateVideo(Video video);
        Task<List<Video>> GetVideoWithTranscription(string searchString);
    }
    public class VideoRepository : IVideoRepository
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
        }

        public async Task<bool> AddVideo(Video video)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    await connection.ExecuteAsync(@"insert into Video (VideoTitle, VideoLength, WebUrl, ThumbnailURL, isFavourite) 
                                                        values(@VideoTitle, @VideoLength, @WebUrl, @ThumbnailURL, @isFavourite)", video);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteVideo(int videoId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    await connection.ExecuteAsync("Delete Video where VideoId=@videoId", new { videoId });
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Video>> GetVideoWithTranscription(string searchString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var transcriptions = await connection.QueryAsync<Transcription>("Select * from Transcription where Phrase like @phrase", new { phrase = '%' + searchString + '%' });
                    var videos = await connection.QueryAsync<Video>("Select * from Video");
                    IEnumerable<Video> finalVideos;

                    finalVideos = videos.Select(video => new Video
                    {
                        VideoId = video.VideoId,
                        VideoTitle = video.VideoTitle,
                        VideoLength = video.VideoLength,
                        WebUrl = video.WebUrl,
                        ThumbnailUrl = video.ThumbnailUrl,
                        IsFavourite = video.IsFavourite,
                        Transcription = transcriptions.Where(tran => tran.VideoId == video.VideoId).ToList()
                    });
                    return finalVideos.ToList();
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateVideo(Video video)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    await connection.ExecuteAsync(@"Update Video Set VideoTitle = @VideoTitle,
                                                VideoLength = @VideoLength,
                                                WebUrl = @WebURL,
                                                ThumbnailURL = @ThumbnailURL,
                                                isFavourite = @IsFavourite", video);
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
