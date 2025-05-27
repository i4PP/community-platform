using FluentValidation;
using BMWDomain.Entities;


namespace BMW.BLL;

public class ClubValidator : AbstractValidator<CreateClub>
{
    public ClubValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Name).MaximumLength(50).WithMessage("Name cannot be more than 50 characters");
        RuleFor(x => x.Desc).NotEmpty().WithMessage("Description is required");
        RuleFor(x => x.Desc).MaximumLength(500).WithMessage("Description cannot be more than 500 characters");
        RuleFor(x => x.OwnerId).NotEmpty().WithMessage("OwnerId is required");
        RuleFor(x => x.Land).NotEmpty().WithMessage("Land is required");
    }
    
}