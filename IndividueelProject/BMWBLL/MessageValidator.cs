using FluentValidation;
using BMWDomain.Entities;

namespace BMW.BLL;

public class MessageValidator : AbstractValidator<Message>
{
    public MessageValidator()
    {
        RuleFor(x => x.Content).NotEmpty().WithMessage("Content is required");
        RuleFor(x => x.Content).MaximumLength(350).WithMessage("Content is too long");
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
        RuleFor(x => x.ClubId).NotEmpty().WithMessage("ClubId is required");
    }
    
}