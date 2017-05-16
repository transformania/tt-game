using FluentValidation;
using MediatR;
using System.Linq;
using System.Threading.Tasks;

namespace TT.Domain.Validation
{
    public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IValidator<TRequest>[] validators;

        public ValidationPipelineBehavior(IValidator<TRequest>[] validators)
        {
            this.validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next)
        {
            var validationContext = new ValidationContext<TRequest>(request);

            var failures = validators.Select(validator => validator.Validate(validationContext))
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Any())
            {
                throw new ValidationException(failures);
            }

            return await next();
        }
    }
}
