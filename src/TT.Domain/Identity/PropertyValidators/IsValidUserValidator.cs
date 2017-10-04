using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using FluentValidation;
using FluentValidation.Resources;
using TT.Domain.Identity.QueryRequests;

namespace TT.Domain.Identity.PropertyValidators
{
    /// <summary>
    /// This property validator checks to see if the given Id refers to a registered user.
    /// </summary>
    public class IsValidUserValidator : AsyncValidatorBase
    {
        private readonly IMediator mediator;

        public IsValidUserValidator(IMediator mediator) : base(new StaticStringSource(""))
        {
            this.mediator = mediator;

            ErrorCodeSource = new StaticStringSource(nameof(IsValidUserValidator));
        }

        protected override async Task<bool> IsValidAsync(PropertyValidatorContext context, CancellationToken cancellation)
        {
            return await mediator.Send(new IsValidUserRequest { UserNameId = context.PropertyValue as string }, cancellation);
        }
    }

    public static class PrincipleIsUserValidatorExtension
    {
        public static IRuleBuilderOptions<T, string> MustBeValidUser<T>(this IRuleBuilder<T, string> ruleBuilder, IMediator mediator)
        {
            return ruleBuilder.SetValidator(new IsValidUserValidator(mediator));
        }
    }
}
