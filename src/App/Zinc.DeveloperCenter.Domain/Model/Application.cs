using RedLine.Domain.Model;

namespace Zinc.DeveloperCenter.Domain.Model
{
    /// <summary>
    /// Represents an application stored in a GitHub repository.
    /// </summary>
    public class Application : AggregateRootBase
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="name">The application full name.</param>
        /// <param name="url">The application GitHub url.</param>
        /// <param name="description">The application description.</param>
        public Application(
            string tenantId,
            string name,
            string url,
            string? description)
        {
            var appName = AppName.Parse(name);

            TenantId = tenantId;
            Name = appName.ApplicationName;
            DisplayName = appName.ApplicationDisplayName;
            Element = appName.ApplicationElement;
            Url = url;
            Description = description;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        protected Application()
        { }

        /// <summary>
        /// Gets the tenant identifier.
        /// </summary>
        public string? TenantId { get; protected set; }

        /// <summary>
        /// Gets the application name where the ADR is defined.
        /// </summary>
        public string? Name { get; protected set; }

        /// <summary>
        /// Gets the application display name where the ADR is defined.
        /// </summary>
        public string? DisplayName { get; protected set; }

        /// <summary>
        /// Gets the application element name.
        /// </summary>
        public string? Element { get; protected set; }

        /// <summary>
        /// Gets the application description.
        /// </summary>
        public string? Description { get; protected set; }

        /// <summary>
        /// Gets the application url.
        /// </summary>
        public string? Url { get; protected set; }

        /// <inheritdoc/>
        public override string Key => $"{TenantId}/{Name}";
    }
}
