using System.Threading;
using FluentValidation;
using MediatR;
using System.Threading.Tasks;
using TT.Domain.RequestInterfaces;
using TT.Domain.Services;
using TT.Domain.ValidatorSelectors;
using FluentValidation.Internal;

namespace TT.Domain.Validation
{
    public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IPrincipalAccessor principalAccessor;
        private readonly IValidator<TRequest> validator;

        public ValidationPipelineBehavior(IPrincipalAccessor principalAccessor, IValidator<TRequest> validator)
        {
            this.principalAccessor = principalAccessor;
            this.validator = validator;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken token, RequestHandlerDelegate<TResponse> next)
        {
            ValidationContext<TRequest> validationContext;
            var ruleSets = RuleSets.None;

            if (request is IRequestWithRuleSet requestWithRuleSet)
            {
                ruleSets = requestWithRuleSet.RuleSets;
            }
            
            if (request is IRequestWithUserNameId requestWithUserNameId && (ruleSets & (RuleSets.Admin | RuleSets.Moderator)) == RuleSets.None)
            {
                // If this request is for a user grab their name Id unless the request was made by an admin or moderator.
                // In which case the name Id is supplied by the admin or moderator.
                requestWithUserNameId.UserNameId = principalAccessor.UserNameId;
            }

            validationContext = new ValidationContext<TRequest>(
                request,
                new PropertyChain(),
                new IntersectRulesetValidatorSelector(ruleSets));

            var result = await validator.ValidateAsync(validationContext, token);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }

            return await next();
        }
    }
}
