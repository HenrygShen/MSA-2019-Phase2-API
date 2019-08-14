﻿using System;
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

        // GET: api/Videos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Video>>> GetVideo()
        {
            var videos = await _videoRepository.GetVideos();
            return Ok(videos);
        }

        // GET: api/Videos/5
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

        // GET api/Videos/SearchByTranscriptions/HelloWorld
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

        //PUT with PATCH to handle isFavourite
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

        // POST: api/Videos
        [HttpPost]
        public async Task<ActionResult<Video>> PostVideo([FromBody]URLDTO data)
        {
            Video video;
            String videoURL;
            String videoId;
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

            // Get the primary key of the newly created video record
            int id = video.VideoId;

            // This is needed because context are NOT thread safe, therefore we create another context for the following task.
            // We will be using this to insert transcriptions into the database on a separate thread
            // So that it doesn't block the API.
            scriberContext tempContext = new scriberContext();
            TranscriptionsController transcriptionsController = new TranscriptionsController(tempContext);

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

        // DELETE: api/Videos/5
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
