using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace TT.Domain.Validation
{
    public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IValidator<TRequest> validator;

        public ValidationPipelineBehavior(IValidator<TRequest> validator)
        {
            this.validator = validator;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next)
        {
            var validationContext = new ValidationContext<TRequest>(request);

            ValidationResult result = await validator.ValidateAsync(validationContext);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }

            return await next();
        }
    }
}
