using System.ComponentModel;

namespace Binner.SwarmApi.Extensions
{
    internal static class EnumExtensions
    {
        /// <summary>
        /// Get description attribute of an enum
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static string GetDescription<TEnum>(this TEnum value) where TEnum : struct
        {
            // Get the Description attribute value for the enum value
            var fi = value.GetType().GetField(value.ToString() ?? string.Empty);
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString() ?? string.Empty;
        }
    }
}
