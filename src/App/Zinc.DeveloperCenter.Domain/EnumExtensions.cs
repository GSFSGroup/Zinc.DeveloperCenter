using System;
using System.ComponentModel;
using System.Linq;

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
            var attribute = value
                .GetType()
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .FirstOrDefault() as DescriptionAttribute;

            if (attribute != null)
            {
                return attribute.Description;
            }

            return value.ToString();
        }
    }
}
