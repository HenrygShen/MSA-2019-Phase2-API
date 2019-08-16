using Back_end.Model;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Back_end.DAL
{
    public interface ITranscriptionsRepository
    {
        Task<IEnumerable<Transcription>> GetTranscriptions();
        Task<Transcription> GetTranscription(int id);
        Task<bool> AddTranscription(Transcription transcription);
        Task<bool> DeleteTranscription(int transcriptionId);
        Task<bool> UpdateTranscription(Transcription transcription);
    }
    public class TranscriptionsRepository : ITranscriptionsRepository
    {

        private readonly string connectionString;

        public TranscriptionsRepository (){
            this.connectionString = "Server=tcp:scriber-hgs.database.windows.net,1433;Initial Catalog=scriber;Persist Security Info=False;User ID=admin-hgs;Password=scriber-7890;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        }

        public async Task<IEnumerable<Transcription>> GetTranscriptions()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var transcriptions = await connection.QueryAsync<Transcription>("Select * from Transcription");
                    return transcriptions;
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<Transcription> GetTranscription(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var transcription = await connection.QuerySingleOrDefaultAsync<Transcription>("Select * from Transcription where TranscriptionId=@id", new { id });
                    return transcription;
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateTranscription(Transcription transcription)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    await connection.ExecuteAsync("UPDATE Transcription SET VideoId = @VideoId, StartTime = @StartTime, Phrase = @Phrase WHERE TranscriptionId=@TranscriptionId", transcription );
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AddTranscription(Transcription transcription)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    await connection.ExecuteAsync(@"insert into Transcription (VideoId, StartTime, Phrase) 
                                                        values(@VideoId, @StartTime, @Phrase)",
                                                        transcription);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteTranscription(int transcriptionId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    await connection.ExecuteAsync("Delete Transcription where TranscriptionId=@transcriptionId",
                                                        new { transcriptionId });
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
