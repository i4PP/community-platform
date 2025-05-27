using System.ComponentModel.DataAnnotations;

namespace BMW.ASP.Models
{
    public class InviteViewModel
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public int ClubId { get; set; }
        
        [Required(ErrorMessage = "Please provide an expiration date.")]
        public DateOnly ExpirationDate { get; set; }
        
        
        [Required(ErrorMessage = "Please provide an expiration time.")]
        public TimeOnly ExpirationTime { get; set; }
        
        [Required(ErrorMessage = "Please specify the maximum uses.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please specify the maximum uses.")]
        public int MaxUses { get; set; }
    }
}