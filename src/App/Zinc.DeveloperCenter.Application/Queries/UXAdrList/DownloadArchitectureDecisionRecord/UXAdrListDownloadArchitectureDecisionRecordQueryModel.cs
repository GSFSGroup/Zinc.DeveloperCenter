namespace Zinc.DeveloperCenter.Application.Queries.UXAdrList.DownloadArchitectureDecisionRecord
{
    /// <summary>
    /// The model returned by the <see cref="UXAdrListDownloadArchitectureDecisionRecordQuery"/>.
    /// </summary>
    public class UXAdrListDownloadArchitectureDecisionRecordQueryModel
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fileName">The file name to return to the client.</param>
        /// <param name="mimeType">The file mime type (markdown or html).</param>
        /// <param name="content">The file contents as string.</param>
        /// <param name="contentUrl">The content url.</param>
        public UXAdrListDownloadArchitectureDecisionRecordQueryModel(
            string fileName,
            string mimeType,
            string content,
            string contentUrl)
        {
            FileName = fileName;
            MimeType = mimeType;
            Content = content;
            ContentUrl = contentUrl;
        }

        /// <summary>
        /// Gets the file name to return to the client.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets the mime type of the file (text/markdown or text/html).
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Gets the file contents as string.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets the file content url.
        /// </summary>
        public string ContentUrl { get; set; }
    }
}
