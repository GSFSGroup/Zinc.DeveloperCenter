using Newtonsoft.Json;

namespace Zinc.DeveloperCenter.Application.Services
{
    /// <summary>
    /// A GitHub Repository Record.
    /// </summary>
    public class GitHubAdrRecord
    {
        /// <summary>
        /// Name. ex: Platinum.Products.
        /// </summary>
        /// <value>name.</value>
        [JsonProperty("name")]
        public string Name { get; set; } = default!;

        /// <summary>
        /// Most recent date of changes to Adr.
        /// </summary>
        /// <value>name.</value>
        [JsonProperty("html_url")]
        public string LastUpdatedDate { get; set; } = default!;

        /// <summary>
        /// Url provided by GitHub API to download Adr.
        /// </summary>
        /// <value>name.</value>
        [JsonProperty("download_url")]
        public string DownloadUrl { get; set; } = default!;
    }
}