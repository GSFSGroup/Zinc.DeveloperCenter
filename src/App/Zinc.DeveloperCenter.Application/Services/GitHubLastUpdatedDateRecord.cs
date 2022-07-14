using Newtonsoft.Json;

namespace Zinc.DeveloperCenter.Application.Services
{
    /// <summary>
    /// A GitHub Repository Record.
    /// </summary>
    public class GitHubLastUpdatedDateRecord
    {
        /// <summary>
        /// Date. ex: 2021-02-12T03:18:31Z.
        /// </summary>
        /// <value>date.</value>
        [JsonProperty("node_id")]
        public string Date { get; set; } = default!;
    }
}