namespace Zinc.DeveloperCenter.Application.Queries.UXAdrList.DownloadArchitectureDecisionRecord
{
    /// <summary>
    /// The model returned by the <see cref="DownloadArchitectureDecisionRecordQuery"/>.
    /// </summary>
    public class DownloadArchitectureDecisionRecordQueryModel
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fileName">The file name to return to the client.</param>
        /// <param name="content">The file contents as UTF8 string.</param>
        public DownloadArchitectureDecisionRecordQueryModel(string fileName, byte[] content)
        {
            FileName = fileName;
            Content = content;
        }

        /// <summary>
        /// Gets the file name to return to the client.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets the file contents as UTF8 string.
        /// </summary>
        public byte[] Content { get; set; }
    }
}
