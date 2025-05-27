namespace BMWDALInterfacesAndDTOs.DTOs;

public class CommentDTO
{
    public int CommentId { get; set; }
    
    public string? Text { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    
    public int OwnerId { get; set; }
    
    public int ThreadId { get; set; }
    
    public int ParentId { get; set; }
    
}