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

        /// <summary>
        /// Contents URL. ex: https://api.github.com/repos/GSFSGroup/Platinum.Products/contents/{+path}.
        /// </summary>
        /// <value>contents_url.</value>
        [JsonProperty("contents_url")]
        public string ContentsUrl { get; set; } = default!;
    }
}