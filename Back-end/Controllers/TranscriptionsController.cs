﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Back_end.Model;
using Microsoft.AspNetCore.Cors;
using Back_end.DAL;

namespace Back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class TranscriptionsController : ControllerBase
    {
        private ITranscriptionsRepository _transcriptionsRepository;

        public TranscriptionsController(ITranscriptionsRepository transcriptionsRepository)
        {
            _transcriptionsRepository = transcriptionsRepository;
        }

        /// <summary>
        /// Retrieves all transcriptions stored in the database.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transcription>>> GetTranscription()
        {
            var result = await _transcriptionsRepository.GetTranscriptions();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves transcription with the specified id.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Transcription>> GetTranscription(int id)
        {
            var transcription = await _transcriptionsRepository.GetTranscription(id);

            if (transcription == null)
            {
                return NotFound();
            }

            return transcription;
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutTranscription(int id, Transcription transcription)
        //{
        //    if (id != transcription.TranscriptionId)
        //    {
        //        return BadRequest();
        //    }

        //    bool updated = await _transcriptionsRepository.UpdateTranscription(transcription);

        //    if (!updated)
        //        return NotFound();

        //    return NoContent();
        //}

        /// <summary>
        /// Retrieves all videos stored in the database.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Transcription>> PostTranscription(Transcription transcription)
        {
            await _transcriptionsRepository.AddTranscription(transcription);

            return CreatedAtAction("GetTranscription", new { id = transcription.TranscriptionId }, transcription);
        }

        /// <summary>
        /// Deletes transcription with id.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Transcription>> DeleteTranscription(int id)
        {
            bool deleted = await _transcriptionsRepository.DeleteTranscription(id);
            if (!deleted)
            {
                return NotFound();
            }

            return Ok(new { message = "Transcription Deleted" }) ;
        }
    }
}
