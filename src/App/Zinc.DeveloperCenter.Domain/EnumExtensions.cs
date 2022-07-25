using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Zinc.DeveloperCenter.Domain
{
    /// <summary>
    /// Extension methods for enums.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the value of the [Description] attribute applied to the enum.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <returns>The description as a string.</returns>
        public static string ToDescription(this Enum value)
        {
            return value
                .GetType()
                .GetMember(value.ToString())
                .First()
                .GetCustomAttribute<DescriptionAttribute>()?.Description ?? value.ToString();
        }
    }
}
