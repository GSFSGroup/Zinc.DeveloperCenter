namespace Zinc.DeveloperCenter.Application.Queries.GitHubADR.Models
{
    /// <summary>
    /// Model for an ADR (Architectural Design Record).
    /// </summary>
    public record GitHubAdrSummaryModel
    {
        /// <summary>
        /// Neat title of ADR. ex: Organize Solution into Layers.
        /// </summary>
        public string? NeatTitle { get; init; }

        /// <summary>
        /// Full title of ADR. ex: adr-0001-github-api.md.
        /// </summary>
        public string? AdrTitle { get; init; }

        /// <summary>
        /// Time that Adr was last updated.
        /// </summary>
        public string? LastUpdatedDate { get; init; }

        /// <summary>
        /// Adr number: ex. adr-0012, number is 12.
        /// </summary>
        public int Number { get; init; }

        /// <summary>
        /// Adr number string: ex. adr-0012.
        /// </summary>
        public string? NumberString { get; init; }

        /// <summary>
        /// Url provided by GitHub API to download Adr.
        /// </summary>
        public string? DownloadUrl { get; set; }

        /// <summary>
        /// Url provided by GitHub API to view Adr on GitHub.
        /// </summary>
        public string? ContentUrl { get; set; }
    }
}
