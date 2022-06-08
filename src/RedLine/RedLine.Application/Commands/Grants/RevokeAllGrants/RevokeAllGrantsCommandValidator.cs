using FluentValidation;

namespace RedLine.Application.Commands.Grants.RevokeAllGrants
{
    /// <summary>
    /// Validates the <see cref="RevokeAllGrantsCommand"/> command.
    /// </summary>
    public class RevokeAllGrantsCommandValidator : AbstractValidator<RevokeAllGrantsCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RevokeAllGrantsCommandValidator"/> class.
        /// </summary>
        public RevokeAllGrantsCommandValidator()
        {
            RuleFor(x => x.CorrelationId).NotEmpty();
            RuleFor(x => x.TenantId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.FullName).NotEmpty();
        }
    }
}
