using FluentValidation;
using BMWDomain.Entities;


namespace BMW.BLL;

public class ThreadValidator : AbstractValidator<DiscussionThread>
{
    public ThreadValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required");
        RuleFor(x => x.Title).MaximumLength(35).WithMessage("Title cannot be more than 35 characters");
        RuleFor(x => x.Text).NotEmpty().WithMessage("Description is required");
        RuleFor(x => x.Text).MaximumLength(5000).WithMessage("Description cannot be more than 5000 characters");
        RuleFor(x => x.OwnerId).NotEmpty().WithMessage("UserId is required");
        RuleFor(x => x.TopicId).NotEmpty().WithMessage("TopicId is required");

    }



    
}