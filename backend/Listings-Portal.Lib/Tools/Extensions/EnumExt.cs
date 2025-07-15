using System.ComponentModel;
using System.Reflection;

namespace Listings_Portal.Lib.Tools.Extensions
{
    /// <summary> Extension methods for Enums. </summary>
    public static class EnumExt
    {
        /// <summary> Gets description from enums description attribute. </summary>
        /// <param name="value"> Enum </param>
        /// <returns> Description from enums description attribute (enum itself otherwise). </returns>
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attr = field?.GetCustomAttribute<DescriptionAttribute>();
            return attr?.Description ?? value.ToString();
        }
    }
}
