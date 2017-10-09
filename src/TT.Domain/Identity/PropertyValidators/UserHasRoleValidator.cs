using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using FluentValidation.Resources;
using FluentValidation;
using MediatR;
using TT.Domain.Identity.QueryRequests;

namespace TT.Domain.Identity.PropertyValidators
{
    public class UserHasCorrectValidator : AsyncValidatorBase
    {
        private readonly IEnumerable<string> role;
        private readonly IMediator mediator;

        public UserHasCorrectValidator(IEnumerable<string> role, IMediator mediator) : base(new StaticStringSource(""))
        {
            this.role = role;
            this.mediator = mediator;

            ErrorCodeSource = new StaticStringSource(nameof(UserHasCorrectValidator));
        }

        protected override async Task<bool> IsValidAsync(PropertyValidatorContext context, CancellationToken cancellation)
        {
            return await mediator.Send(new UserHasAnyRoleRequest() { UserNameId = context.PropertyValue as string, Role = role }, cancellation);
        }
    }

    public static class UserIsInRoleValidatorExtension
    {
        public static IRuleBuilderOptions<T, string> MustHaveCorrectRole<T>(this IRuleBuilder<T, string> ruleBuilder, IMediator mediator, params string[] roles)
        {
            return ruleBuilder.SetValidator(new UserHasCorrectValidator(roles, mediator));
        }
    }
}
