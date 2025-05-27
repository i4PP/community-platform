using BMWDomain.Entities;


namespace BMWDomain.interfaces;

public interface ICommentContainer
{
    
    public RootAndChilderenComments GetCommentsByThreadId(int threadId);

    public void CreateComment(Comment comment);
    
}