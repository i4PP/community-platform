namespace BMWDALInterfacesAndDTOs.DTOs;

public class RootAndChilderenCommentsDTO
{
    public List<CommentDTO> RootComments { get; set; } = new List<CommentDTO>();
    public List<CommentDTO> ChildComments { get; set; } = new List<CommentDTO>();
    
}