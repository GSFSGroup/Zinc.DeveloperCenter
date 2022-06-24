namespace Zinc.DeveloperCenter.Application.Queries.GitHubADR.Models
{
    /// <summary>
    /// Model for an ADR (Architectural Design Record).
    /// </summary>
    public record GitHubAdrModel
    {
        /// <summary>
        /// Full title of ADR. ex: adr-0001-github-api.md.
        /// </summary>
        public string? Title { get; init; }
    }
}
