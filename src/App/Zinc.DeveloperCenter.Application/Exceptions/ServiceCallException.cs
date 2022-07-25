using System;
using System.Runtime.Serialization;
using RedLine.Domain.Exceptions;

namespace Zinc.DeveloperCenter.Application.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a service to service call fails.
    /// </summary>
    [Serializable]
    public class ServiceCallException : RedLineException
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="statusCode">A status code used to further define the exception.</param>
        /// <param name="message">The error message that explains why the exception occurred.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ServiceCallException(
            int statusCode,
            string message,
            Exception? innerException)
            : base(statusCode, message, innerException)
        { }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="statusCode">A status code used to further define the exception.</param>
        /// <param name="message">The error message that explains why the exception occurred.</param>
        /// <param name="callingService">The service making the call.</param>
        /// <param name="calledService">The service receiving the call.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ServiceCallException(
            int statusCode,
            string message,
            string callingService,
            string calledService,
            Exception? innerException)
            : this(statusCode, $"{callingService} received a {statusCode} status code from {calledService}: {message}.", innerException)
        { }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="info">Holds the serialized object data.</param>
        /// <param name="context">Contains contextual information about the serialization.</param>
        protected ServiceCallException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
