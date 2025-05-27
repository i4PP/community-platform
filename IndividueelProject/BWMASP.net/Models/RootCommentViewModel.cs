namespace BMW.ASP.Models;

public class RootCommentViewModel
{
    public int CommentId { get; set; }
    
    public string? OwnerName { get; set; }
    

    public int Id { get; set; }
    

    public string? Text { get; set; }
    

    public int OwnerId { get; set; }
    

    public int ThreadId { get; set; }
    

    public string? DateCreated { get; set; }
    
    public List<CommentViewModel> ChildComments { get; set; } = new List<CommentViewModel>();
    
}