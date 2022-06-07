using FluentValidation;

namespace RedLine.Application.Commands.Grants.RevokeGrant
{
    /// <summary>
    /// Validates the <see cref="RevokeGrantCommand"/>.
    /// </summary>
    public class RevokeGrantCommandValidator : AbstractValidator<RevokeGrantCommand>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public RevokeGrantCommandValidator()
        {
            RuleFor(x => x.CorrelationId).NotEmpty();
            RuleFor(x => x.GrantType).NotEmpty();
            RuleFor(x => x.Qualifier).NotEmpty();
            RuleFor(x => x.TenantId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}
