using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Back_end.Model;
using Back_end.Helper;
using Back_end.DAL;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Cors;

namespace Back_end.Controllers
{
    // DTO (Data Transfer object) inner class to help with Swagger Documentation.
    public class URLDTO
    {
        public String URL { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class VideosController : ControllerBase
    {
        private IVideoRepository _videoRepository;
        private readonly IMapper _mapper;

        public VideosController(IMapper mapper, IVideoRepository videoRepository)
        {
            _mapper = mapper;
            _videoRepository = videoRepository;
        }

        /// <summary>
        /// Retrieves all videos stored in the database.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Video>>> GetVideo()
        {
            var videos = await _videoRepository.GetVideos();
            return Ok(videos);
        }

        /// <summary>
        /// Retrieves a video by its id.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Video>> GetVideo(int id)
        {
            var video = await _videoRepository.GetVideoByID(id);

            if (video == null)
            {
                return NotFound();
            }

            return Ok(video);
        }

        /// <summary>
        /// Retrieves all videos that contain a certain phrase in its transcriptions.
        /// </summary>
        [HttpGet("SearchByTranscriptions/{searchString}")]
        public async Task<ActionResult<IEnumerable<Video>>> Search(string searchString)
        {
            if (String.IsNullOrEmpty(searchString))
            {
                return BadRequest("Search string cannot be null or empty.");
            }

            // Choose transcriptions that has the phrase 
            List<Video> videos = await _videoRepository.GetVideoWithTranscription(searchString);

            if (videos == null)
                return BadRequest("Something went wrong in the GetVideoWithTranscription method");

            // Removes all videos with empty transcription
            videos.RemoveAll(video => video.Transcription.Count == 0);
            return Ok(videos);
        }

        /// <summary>
        /// Update the Video isFavourite value.
        /// </summary>
        [HttpPatch("update/{id}")]
        public async Task<ActionResult<VideoDTO>> Patch(int id, [FromBody]JsonPatchDocument<VideoDTO> videoPatch)
        {
            //get original video object from the database
            Video originVideo = await _videoRepository.GetVideoByID(id);
            //use automapper to map that to DTO object
            VideoDTO videoDTO = _mapper.Map<VideoDTO>(originVideo);
            //apply the patch to that DTO
            videoPatch.ApplyTo(videoDTO);
            //use automapper to map the DTO back ontop of the database object
            _mapper.Map(videoDTO, originVideo);
            //update video in the database
            await _videoRepository.UpdateVideo(originVideo);
            return videoDTO;
        }

        /// <summary>
        /// Adds the Video to the database, then gets the video transcriptions and saves it to Transcriptions.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Video>> PostVideo([FromBody]URLDTO data)
        {
            Video video;
            String videoURL;
            String videoId;
            Video checkVideo = await _videoRepository.GetVideoByURL(data.URL);
            if (checkVideo != null)
            {
                return CreatedAtAction("GetVideo", new { id = checkVideo.VideoId }, checkVideo);
            }
            try
            {
                // Constructing the video object from our helper function
                videoURL = data.URL;
                videoId = YouTubeHelper.GetVideoIdFromURL(videoURL);
                video = YouTubeHelper.GetVideoInfo(videoId);
            }
            catch
            {
                return BadRequest("Invalid YouTube URL");
            }

            // Add this video object to the database
            await _videoRepository.AddVideo(video);

            // Get video for videoId
            Video newVideo = await _videoRepository.GetVideoByURL(data.URL);

            // Get the primary key of the newly created video record
            //video = _videoRepository.GetVideoByID
            int id = newVideo.VideoId;


            ITranscriptionsRepository transcriptionsRepository = new TranscriptionsRepository();
            TranscriptionsController transcriptionsController = new TranscriptionsController(transcriptionsRepository);

            // This will be executed in the background.
            Task addCaptions = Task.Run(async () =>
            {
                // Get a list of captions from YouTubeHelper
                List<Transcription> transcriptions = new List<Transcription>();
                transcriptions = YouTubeHelper.GetTranscriptions(videoId);

                for (int i = 0; i < transcriptions.Count; i++)
                {
                    // Get the transcription objects form transcriptions and assign VideoId to id, the primary key of the newly inserted video
                    Transcription transcription = transcriptions.ElementAt(i);
                    transcription.VideoId = id;
                    // Add this transcription to the database
                    await transcriptionsController.PostTranscription(transcription);
                }
            });

            // Return success code and the info on the video object
            return CreatedAtAction("GetVideo", new { id = video.VideoId }, video);
        }

        /// <summary>
        /// Delete the video by id.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Video>> DeleteVideo(int id)
        {
            var video = await _videoRepository.GetVideoByID(id);
            if (video == null)
            {
                return NotFound();
            }

            bool deleted = await _videoRepository.DeleteVideo(id);
            if (!deleted)
                return BadRequest(new { message = "Failed to delete video" });
            return Ok(new { message = "Video deleted" });
        }
    }
}
