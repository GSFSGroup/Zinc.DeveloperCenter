using System;
using RedLine.Application.Queries;
using Zinc.DeveloperCenter.Domain.Model.GitHub;

namespace Zinc.DeveloperCenter.Application.Queries.UXAdrList.DownloadArchitectureDecisionRecord
{
    /// <summary>
    /// The query used to download the content of an ADR.
    /// </summary>
    public class DownloadArchitectureDecisionRecordQuery : QueryBase<string>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="applicationName">The application name.</param>
        /// <param name="filePath">The ADR file path.</param>
        /// <param name="fileFormat">The format to return (raw markdown or html).</param>
        public DownloadArchitectureDecisionRecordQuery(
            string tenantId,
            Guid correlationId,
            string applicationName,
            string filePath,
            FileFormat fileFormat)
            : base(tenantId, correlationId)
        {
            ApplicationName = applicationName;
            FilePath = filePath;
            FileFormat = fileFormat;
        }

        /// <summary>
        /// Gets the application name.
        /// </summary>
        public string ApplicationName { get; }

        /// <summary>
        /// Gets the ADR file path.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Gets the format to return (raw markdown or html).
        /// </summary>
        public FileFormat FileFormat { get; }
    }
}
