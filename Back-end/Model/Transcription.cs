using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Back_end.Model
{
    //public partial class TranscriptionDTO
    //{
    //    public int? VideoId { get; set; }
    //    public int StartTime { get; set; }
    //    public string Phrase { get; set; }
    //}
    public partial class Transcription
    {
        public int TranscriptionId { get; set; }
        public int? VideoId { get; set; }
        public int StartTime { get; set; }
        [Required]
        [StringLength(255)]
        public string Phrase { get; set; }

        [ForeignKey("VideoId")]
        [InverseProperty("Transcription")]
        public virtual Video Video { get; set; }
    }
}
