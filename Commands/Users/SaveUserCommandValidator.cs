using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common.Constants;
using Data;
using Extensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Commands.Users
{
    public class SaveUserCommandValidator : AbstractValidator<SaveUserCommand>
    {
        private readonly DatabaseContext context;

        private static readonly Regex PasswordRegex = new(ApplicationConstants.PasswordRegex, RegexOptions.Compiled);

        public SaveUserCommandValidator(DatabaseContext context)
        {
            this.context = context;

            RuleFor(cmd => cmd.MobileNumber)
                .NotEmpty().WithMessage("Mobile Number is required")
                .Must(IsValidMobileNumber).WithMessage("Mobile Number must be only numeric")
                .Length(10, 10).WithMessage("Mobile Number should have 10 characters");

            RuleFor(x => x).MustAsync((x, cancellation) => IsMobileNumberAlreadyRegistered(x))
                .OverridePropertyName("MobileNumber")
                .WithMessage("Mobile number already registered");

            RuleFor(cmd => cmd.FirstName)
                .NotEmpty().WithMessage("First Name is required")
                .Length(0, 50).WithMessage("First Name should not exceed 50 characters");

            RuleFor(cmd => cmd.LastName)
                .NotEmpty().WithMessage("Last Name is required")
                .Length(0, 50).WithMessage("Last Name should not exceed 50 characters");

            RuleFor(cmd => cmd.EmailAddress)
                .NotEmpty().WithMessage("Email is required");

            RuleFor(cmd => cmd.EmailAddress)
                .EmailAddress().When(i => i.EmailAddress.HasValue()).WithMessage("Invalid Email Address");

            RuleFor(x => x).MustAsync((x, cancellation) => IsEmailAddressAlreadyRegistered(x)).When(i => i.EmailAddress.HasValue())
                .OverridePropertyName("EmailAddress")
                .WithMessage("Email Address already registered");

            RuleFor(cmd => cmd.Password)
                .NotEmpty().When(i => i.Id == 0).WithMessage("Password is required");

            RuleFor(cmd => cmd.ConfirmPassword)
                .NotEmpty().When(i => i.Id == 0).WithMessage("Confirm Password is required");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).When(i => i.Id == 0).WithMessage("Passwords do not match");

            RuleFor(x => x).MustAsync((x, cancellation) => IsValidPassword(x))
                .OverridePropertyName("Password")
                .WithMessage("Password must be 8-25 characters and at least 1 uppercase, 1 lowercase, 1 digit, 1 special character's (@#$%)");

        }

        private static bool IsValidMobileNumber(string mobileNumber)
        {
            return mobileNumber.IsNumeric();
        }

        private async Task<bool> IsMobileNumberAlreadyRegistered(SaveUserCommand command)
        {
            if (command.Id > 0)
                return true;

            var result = await context.User
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.MobileNumber.Equals(command.MobileNumber));
            return result == null;
        }

        private async Task<bool> IsEmailAddressAlreadyRegistered(SaveUserCommand command)
        {
            if (command.Id > 0)
                return true;

            var result = await context.User
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.EmailAddress.Equals(command.EmailAddress));
            return result == null;
        }

        private Task<bool> IsValidPassword(SaveUserCommand command)
        {
            return command.Id > 0 ? Task.FromResult(true) : Task.FromResult(command.Password.IsNullOrEmpty() || PasswordRegex.IsMatch(command.Password));
        }
    }
}
