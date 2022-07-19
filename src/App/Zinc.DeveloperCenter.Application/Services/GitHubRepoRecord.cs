using Newtonsoft.Json;

namespace Zinc.DeveloperCenter.Application.Services
{
    /// <summary>
    /// A GitHub Repository Record.
    /// </summary>
    public class GitHubRepoRecord
    {
        /// <summary>
        /// Name. ex: Platinum.Products.
        /// </summary>
        /// <value>name.</value>
        [JsonProperty("name")]
        public string Name { get; set; } = default!;
    }
}