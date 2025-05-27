using BMWDALInterfacesAndDTOs.DTOs;

namespace BMWDALInterfacesAndDTOs.Interfaces;

public interface ICommentRepository
{


    public void CreateComment(CommentDTO commentDto);
    
    public RootAndChilderenCommentsDTO GetCommentsByThreadId(int threadId);
    


    
}