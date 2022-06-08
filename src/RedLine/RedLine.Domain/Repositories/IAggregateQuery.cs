using RedLine.Domain.Model;

namespace RedLine.Domain.Repositories
{
    /// <summary>
    /// Marker interface for querying aggregates.
    /// </summary>
    /// <typeparam name="TAggregate">The type of aggregate.</typeparam>
    public interface IAggregateQuery<TAggregate>
        where TAggregate : class, IAggregateRoot
    {
    }
}
