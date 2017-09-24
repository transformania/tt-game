using FluentValidation;
using MediatR;
using TT.Domain.Identity.CommandRequests;
using TT.Domain.Identity.PropertyValidators;
using TT.Domain.Statics;

namespace TT.Domain.Identity.RequestValidators
{
    public class ResetSecurityStampValidator : AbstractValidator<ResetSecurityStamp>
    {
        public ResetSecurityStampValidator(IMediator medator)
        {
            RuleFor(m => m.UserNameId).MustHaveCorrectRole(medator, PvPStatics.Permissions_Admin)
                .WithMessage("User not in proper role.")
                .DependentRules(av =>
                {
                    av.RuleFor(m => m.TargetUserNameId).MustBeValidUser(medator).WithMessage("The user id given does not match any user.");
                });            
        }
    }
}
