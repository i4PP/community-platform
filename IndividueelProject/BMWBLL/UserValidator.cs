using FluentValidation;
using BMWDomain.Entities;
using System.Text.RegularExpressions;

#pragma warning disable SYSLIB1006
namespace BMW.BLL
{
    public class UserValidator : AbstractValidator<Account>
    {
        public UserValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Username is required");
            RuleFor(x => x.Name).MaximumLength(50).WithMessage("Username cannot be more than 50 characters");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Email is not valid");
            RuleFor(x => x.Email).MaximumLength(50).WithMessage("Email cannot be more than 50 characters");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
            RuleFor(x => x.Password).MaximumLength(50).WithMessage("Password cannot be more than 50 characters");
            RuleFor(x => x.Password).MinimumLength(6).WithMessage("Password must be at least 6 characters long");
            RuleFor(x => x.Password).Must(ContainsUppercase).WithMessage("Password must contain at least one uppercase letter");
            RuleFor(x => x.Password).Must(ContainsSpecialCharacter).WithMessage("Password must contain at least one special character");
        }

        private bool ContainsUppercase(string password)
        {
            return !string.IsNullOrEmpty(password) && password.Any(char.IsUpper);
        }

        private bool ContainsSpecialCharacter(string password)
        {
            return !string.IsNullOrEmpty(password) && Regex.IsMatch(password, @"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");
        }
    }
}
#pragma warning restore SYSLIB1006