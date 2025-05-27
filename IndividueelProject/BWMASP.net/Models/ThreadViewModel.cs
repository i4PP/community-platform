using System.ComponentModel.DataAnnotations;
using BMWDomain.Entities;


namespace BMW.ASP.Models
{
    public class ThreadViewModel
    {
        public int? ThreadId { get; set; }

        [Required]
        [MaxLength(40, ErrorMessage = "Title must be less than 35 characters.")]
        public string? Title { get; set; }

        [Required]
        [StringLength(5000)]
        public string? Text { get; set; }

        [Required]
        public int OwnerId { get; set; }

        public string? UserName { get; set; }

        public string? CreatedAt { get; set; }

        [Required]
        public int TopicId { get; set; }

        public List<Topic>? Topics { get; set; }

        public string? TopicName { get; set; }

        public bool? IsEdited { get; set; }





    }
}
