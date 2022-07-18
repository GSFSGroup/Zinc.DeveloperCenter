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
        public string? DotName { get; init; }

        /// <summary>
        /// Shortened repo name. ex: Products.
        /// </summary>
        public string? NeatName { get; init; }

        /// <summary>
        /// Element of repo. ex: Platinum.
        /// </summary>
        public string? Element { get; init; }
    }
}
