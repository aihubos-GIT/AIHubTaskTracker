using System.ComponentModel;
using System.Reflection;

namespace AIHubTaskTracker.Extensions 
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum enumValue)
        {

            var field = enumValue.GetType().GetField(enumValue.ToString());

            if (field != null && Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            {
                return attribute.Description;
            }
            return enumValue.ToString();
        }
    }
}