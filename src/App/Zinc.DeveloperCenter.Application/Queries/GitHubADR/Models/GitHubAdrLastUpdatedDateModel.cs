namespace Zinc.DeveloperCenter.Application.Queries.GitHubADR.Models
{
    /// <summary>
    /// Model for an ADR (Architectural Design Record).
    /// </summary>
    public record GitHubAdrLastUpdatedDateModel
    {
        /// <summary>
        /// Date of most recent update to Adr.
        /// </summary>
        public string? LastUpdatedDate { get; init; }
    }
}
