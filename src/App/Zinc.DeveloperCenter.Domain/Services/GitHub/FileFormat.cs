using System.ComponentModel;

namespace Zinc.DeveloperCenter.Domain.Model.GitHub
{
    /// <summary>
    /// An enum that defines the file format to return.
    /// </summary>
    public enum FileFormat
    {
        /// <summary>
        /// An unknown format.
        /// </summary>
        Unknown,

        /// <summary>
        /// Return the markdown formatted as html.
        /// </summary>
        [Description("application/vnd.github.VERSION.html")]
        Html,

        /// <summary>
        /// Return the raw markdown format.
        /// </summary>
        [Description("application/vnd.github.VERSION.raw")]
        Raw,
    }
}
