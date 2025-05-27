using System.ComponentModel.DataAnnotations;

namespace BMW.ASP.Models;

public class CreateCommentViewModel
{

    

    public int Id { get; set; }
    
    

    public string? Text { get; set; }
    

    public int OwnerId { get; set; }
    
    
    public int ThreadId { get; set; }
    

    public string? DateCreated { get; set; }
    
    public int ParentId { get; set; }
    
    
    
}