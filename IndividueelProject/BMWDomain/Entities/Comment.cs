using BMWDALInterfacesAndDTOs.DTOs;

namespace BMWDomain.Entities;

public class Comment
{
    public int CommentId { get; set; }
    
    public string Text { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    
    public int OwnerId { get; set; }
    
    public int ThreadId { get; set; }
    
    public int ParentId { get; set; }
    
    public Comment(int commentId, string text, DateTimeOffset createdAt, int ownerId, int threadId, int parentId)
    {
        CommentId = commentId;
        Text = text ?? throw new ArgumentNullException(nameof(text));
        CreatedAt = createdAt;
        OwnerId = ownerId;
        ThreadId = threadId;
        ParentId = parentId;
    }

    
    public Comment(CommentDTO commentDto)
    {
        CommentId = commentDto.CommentId;
        Text = commentDto.Text ?? throw new ArgumentNullException(nameof(commentDto));
        CreatedAt = commentDto.CreatedAt;
        OwnerId = commentDto.OwnerId;
        ThreadId = commentDto.ThreadId;
        ParentId = commentDto.ParentId;
    }
    
    public CommentDTO ToDTO()
    {
        return new CommentDTO
        {
            CommentId = CommentId,
            Text = Text,
            CreatedAt = CreatedAt,
            OwnerId = OwnerId,
            ThreadId = ThreadId,
            ParentId = ParentId
        };
    }
}