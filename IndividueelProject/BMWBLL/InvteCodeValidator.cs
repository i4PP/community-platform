using FluentValidation;
using BMWDomain.Entities;

namespace BMW.BLL;

public class InvteCodeValidator : AbstractValidator<InviteCode>
{
    public InvteCodeValidator()
    {
        RuleFor(x => x.ClubId).NotEmpty().WithMessage("ClubId is required");
        RuleFor(x => x.ExpirationDate).NotEmpty().WithMessage("ExpirationDate is required");
        RuleFor(x => x.MaxUses).NotEmpty().WithMessage("MaxUses is required");

    }
    
}