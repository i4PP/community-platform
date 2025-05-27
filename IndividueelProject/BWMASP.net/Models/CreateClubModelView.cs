using System.ComponentModel.DataAnnotations;

namespace BMW.ASP.Models;


public class CreateClubModelView
{
    [Required (ErrorMessage = "Name is required")]
    [MaxLength(50)]
    public string? Name { get; set; }
    
    
    [Required (ErrorMessage = "Description is required")]
    [MaxLength(200)]
    public string? Desc { get; set; }
    
    [Required (ErrorMessage = "Country is required")]
    public string? Land { get; set; }
    
    [Required]
    public int OwnerId { get; set; }
    

}

