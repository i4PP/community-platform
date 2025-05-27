using BMWDALInterfacesAndDTOs.Exceptions;
using BMWDomain.Entities;
using BMWDomain.interfaces;
using BMWDALInterfacesAndDTOs.Interfaces;
using BMWDomain.Exceptions;
using FluentValidation;

namespace BMW.BLL;

public class CommentContainer : ICommentContainer
{
    private readonly ICommentRepository _commentRepository;
    private readonly IValidator<Comment> _commentValidator;

    public CommentContainer(ICommentRepository commentRepository, IValidator<Comment> commentValidator)
    {
        this._commentRepository = commentRepository;
        this._commentValidator = commentValidator;
    }


    public void CreateComment(Comment comment)
    {
        _commentValidator.ValidateAndThrow(comment);
        try
        {
            
            var tdo = comment.ToDTO();
            _commentRepository.CreateComment(tdo);
            comment.CommentId = tdo.CommentId;
            
        }
        catch (NetworkException e)
        {
            throw new DataBaseException("Network-related error occurred while connecting to the database. -1", e);
        }
        catch (DalException e)
        {
            throw new DataBaseException("error while getting comment details 501", e);
        }

    }

    public RootAndChilderenComments GetCommentsByThreadId(int threadId)
    {
        try
        {
            var result = _commentRepository.GetCommentsByThreadId(threadId);
            
            return new RootAndChilderenComments(result);
            
        }
        catch (NetworkException e)
        {
            throw new DataBaseException("Network-related error occurred while connecting to the database. -1", e);
        }
        catch (DalException e)
        {
            throw new DataBaseException("error while getting comment details 501", e);
        }
        catch (Exception e)
        {
            throw new BllException("error while getting comment details 500", e);
        }



    }
    
    
}