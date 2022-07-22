namespace Zinc.DeveloperCenter.Application.Queries.GitHubRepo.Models
{
    /// <summary>
    /// Model for a GitHub repository.
    /// </summary>
    public record GitHubRepoModel
    {
        /// <summary>
        /// Full repo name. ex: Platinum.Products.
        /// </summary>
        public string? ApplicationName { get; init; }

        /// <summary>
        /// Shortened repo name. ex: Products.
        /// </summary>
        public string? ApplicationDisplayName { get; init; }

        /// <summary>
        /// ApplicationElement of repo. ex: Platinum.
        /// </summary>
        public string? ApplicationElement { get; init; }
    }
}
