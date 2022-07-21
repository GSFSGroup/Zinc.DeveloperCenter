using System;
using System.Collections.Generic;
using System.Text;
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
        /// <param name="applicationName">The application full name.</param>
        /// <param name="applicationDisplayName">The application display name.</param>
        /// <param name="applicationElement">The application element.</param>
        public Application(
            string tenantId,
            string applicationName,
            string applicationDisplayName,
            string? applicationElement)
        {
            TenantId = tenantId;
            ApplicationName = applicationName;
            ApplicationDisplayName = applicationDisplayName;
            ApplicationElement = applicationElement;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="applicationName">The application full name.</param>
        public Application(string tenantId, string applicationName)
        {
            TenantId = tenantId;

            var nameParts = applicationName.Split('.', 2, StringSplitOptions.RemoveEmptyEntries);

            if (nameParts.Length == 1)
            {
                ApplicationName = applicationName;
                ApplicationDisplayName = GetDisplayName(applicationName);
            }
            else
            {
                ApplicationElement = nameParts[0];
                ApplicationName = applicationName;
                ApplicationDisplayName = GetDisplayName(nameParts[1]);
            }
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        protected Application()
        { }

        /// <summary>
        /// Gets the application element name.
        /// </summary>
        public string? ApplicationElement { get; protected set; }

        /// <summary>
        /// Gets the application name where the ADR is defined.
        /// </summary>
        public string? ApplicationName { get; protected set; }

        /// <summary>
        /// Gets the application display name where the ADR is defined.
        /// </summary>
        public string? ApplicationDisplayName { get; protected set; }

        /// <inheritdoc/>
        public override string Key => $"{TenantId}/{ApplicationName}";

        /// <summary>
        /// Gets the tenant identifier.
        /// </summary>
        public string? TenantId { get; protected set; }

        private static string GetDisplayName(string value)
        {
            /* This method returns a display name from an application name. It is assumed
             * the element name has already been removed prior to calling this method.
             *
             * For example:
             * DataCatalog = Data Catalog
             * DataExchange.Api = Data Exchange Api
             * */

            var buffer = new StringBuilder(value);
            buffer.Replace(".", null); // Get rid of any additional dots in the name

            // Find the positions within the buffer where we will add a space (wherever we find an upper case letter)
            var insertSpaceAtIndexes = new List<int>();

            for (int i = 1; i < buffer.Length; i++)
            {
                if (char.IsUpper(buffer[i]))
                {
                    insertSpaceAtIndexes.Add(i);
                }
            }

            int j = 1;

            // Once we add a space, we need to adjust the indexes so they match the new string
            for (int i = 1; i < insertSpaceAtIndexes.Count; i++)
            {
                insertSpaceAtIndexes[i] += j;
                j++;
            }

            foreach (var index in insertSpaceAtIndexes)
            {
                buffer.Insert(index, " ");
            }

            return buffer.ToString();
        }
    }
}
