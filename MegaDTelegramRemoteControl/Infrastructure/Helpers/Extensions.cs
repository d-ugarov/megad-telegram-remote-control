using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MegaDTelegramRemoteControl.Infrastructure.Helpers
{
    public static class Extensions
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            return field is null || !(Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                ? value.ToString()
                : attribute.Description;
        }
        
        public static T GetAttribute<T>(this object value)
        {
            var attributes = value.GetAttributesInternal<T>();
            return (T)attributes[0];
        }
        
        public static List<T> GetEnumList<T>() => Enum.GetValues(typeof(T)).Cast<T>().ToList();

        public static bool TryGetAttribute<T>(this object value, out T? attribute)
        {
            var attributes = value.GetAttributesInternal<T>();
            
            if (!attributes.Any())
            {
                attribute = default;
                return false;
            }

            attribute = (T)attributes[0];
            return true;
        }

        private static object[] GetAttributesInternal<T>(this object value)
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString() ?? "");
            return !memberInfo.Any()
                ? new object[0]
                : memberInfo[0].GetCustomAttributes(typeof(T), false);
        }

        public static string GetWhitespaces(int count = 1) => new(' ', Math.Max(0, count * 4));
    }
}