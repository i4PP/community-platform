using BMWDALInterfacesAndDTOs.DTOs;

namespace BMWDomain.Entities;

public class RootAndChilderenComments
{
    
    public List<Comment> RootComments { get; set; } = new List<Comment>();
    public List<Comment> ChildComments { get; set; } = new List<Comment>();


    public RootAndChilderenComments(RootAndChilderenCommentsDTO rootAndChilderenCommentsDto)
    {
        foreach (var commentDto in rootAndChilderenCommentsDto.RootComments)
        {
            RootComments.Add(new Comment(commentDto));
        }
        foreach (var commentDto in rootAndChilderenCommentsDto.ChildComments)
        {
            ChildComments.Add(new Comment(commentDto));
        }
    }
}