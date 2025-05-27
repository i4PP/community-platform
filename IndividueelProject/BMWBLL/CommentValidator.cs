using FluentValidation;
using BMWDomain.Entities;

namespace BMW.BLL;

public class CommentValidator : AbstractValidator<Comment>
{
    
    public CommentValidator()
    {
        RuleFor(x => x.ThreadId).NotEmpty().WithMessage("ThreadId is required");
        RuleFor(x => x.OwnerId).NotEmpty().WithMessage("CommentText is required");
        RuleFor(x => x.Text).NotEmpty().WithMessage("CommentText is required");

    }
    
}