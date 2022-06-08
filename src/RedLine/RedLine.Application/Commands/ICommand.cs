using MediatR;

namespace RedLine.Application.Commands
{
    /// <summary>
    /// The interface that defines a contract for commands.
    /// </summary>
    /// <typeparam name="TResponse">The type of response returned from the command.</typeparam>
    public interface ICommand<out TResponse> : IActivity, IRequest<TResponse>
    {
    }
}
