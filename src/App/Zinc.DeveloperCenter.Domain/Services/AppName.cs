using System;
using System.Collections.Generic;
using System.Text;

namespace Zinc.DeveloperCenter.Domain.Services
{
    /// <summary>
    /// A helper class used to parse the application name into various components.
    /// </summary>
    public class AppName
    {
        private AppName(
            string applicationName,
            string applicationDisplayName,
            string? applicationElement)
        {
            ApplicationName = applicationName;
            ApplicationDisplayName = applicationDisplayName;
            ApplicationElement = applicationElement;
        }

        /// <summary>
        /// Gets the application name where the ADR is defined.
        /// </summary>
        public string ApplicationName { get; }

        /// <summary>
        /// Gets the application display name where the ADR is defined.
        /// </summary>
        public string ApplicationDisplayName { get; }

        /// <summary>
        /// Gets the application element name.
        /// </summary>
        public string? ApplicationElement { get; }

        /// <summary>
        /// Parses the application name into it various components.
        /// </summary>
        /// <param name="applicationName">The application name.</param>
        /// <returns><see cref="ApplicationName"/>.</returns>
        public static AppName Parse(string applicationName)
        {
            var nameParts = applicationName.Split('.', 2, StringSplitOptions.RemoveEmptyEntries);

            string name;
            string displayName;
            string? element;

            if (nameParts.Length == 1)
            {
                element = null;
                name = applicationName;
                displayName = GetDisplayName(applicationName);
            }
            else
            {
                element = nameParts[0];
                name = applicationName;
                displayName = GetDisplayName(nameParts[1]);
            }

            return new AppName(name, displayName, element);
        }

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
