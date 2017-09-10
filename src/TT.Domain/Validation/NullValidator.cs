using FluentValidation;
using MediatR;

namespace TT.Domain.Validation
{
    /// <summary>
    /// An empty <see cref="IValidator{T}"/> used if an <see cref="IRequest"/> does not have an <see cref="IValidator{T}"/>.
    /// </summary>
    public class NullValidator : AbstractValidator<IRequest>
    {
    }

    /// <summary>
    /// An empty <see cref="IValidator{T}"/> used if an <see cref="IRequest{TResponse}"/> does not have an <see cref="IValidator{T}"/>.
    /// </summary>
    public class NullValidatorWithResponse<TResponse> : AbstractValidator<IRequest<TResponse>>
    {
    }
}
