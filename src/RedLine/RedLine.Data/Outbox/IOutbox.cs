using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RedLine.Domain.Model;

namespace RedLine.Data.Outbox
{
    /// <summary>
    /// The interface that defines a contract for an Outbox, which is used to persist domain events.
    /// </summary>
    public interface IOutbox
    {
        /// <summary>
        /// Dispatches the events to the service bus.
        /// </summary>
        /// <param name="dispatcherId">The id of the dispatcher, in case multiple dispatchers are running in-process.</param>
        /// <param name="dispatch">The function used to dispatch the events.</param>
        /// <returns>The number of events dispatched.</returns>
        Task<int> DispatchEvents(string dispatcherId, Func<OutboxRecord, Task<int>> dispatch);

        /// <summary>
        /// Saves the domain events to persistent storage.
        /// </summary>
        /// <param name="aggregate">The <see cref="IAggregateRoot"/> being saved.</param>
        /// <returns>The number of events saved.</returns>
        Task<int> SaveEvents(IAggregateRoot aggregate);

        /// <summary>
        /// Saves the domain events to persistent storage.
        /// </summary>
        /// <param name="messages">The <see cref="OutboxMessage"/>s to save.</param>
        /// <returns>The number of events saved.</returns>
        /// <remarks>
        /// This method should only be called by the AuthorizationBehavior to make it more resilient to RabbitMQ failures.
        /// </remarks>
        Task<int> SaveMessages(IEnumerable<OutboxMessage> messages);
    }
}
