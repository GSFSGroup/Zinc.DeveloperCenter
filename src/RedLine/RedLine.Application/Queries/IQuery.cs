using MediatR;

namespace RedLine.Application.Queries
{
    /// <summary>
    /// The interface that defines a contract for queries.
    /// </summary>
    /// <typeparam name="TResponse">The type of response returned from the query.</typeparam>
    public interface IQuery<out TResponse> : IActivity, IRequest<TResponse>
    {
    }
}
