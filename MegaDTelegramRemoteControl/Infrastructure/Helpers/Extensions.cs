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
            return !(Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                ? value.ToString()
                : attribute.Description;
        }
        
        public static T GetAttribute<T>(this object value)
        {
            var attributes = value.GetAttributesInternal<T>();
            return (T)attributes[0];
        }

        public static List<T> GetAttributes<T>(this object value)
        {
            var attributes = value.GetAttributesInternal<T>();
            return attributes.Select(x => (T)x).ToList();
        }
        
        public static List<T> GetEnumList<T>()
        {
            var result = new List<T>();
            foreach (T x in Enum.GetValues(typeof(T)))
            {
                result.Add(x);
            }
            return result;
        }
        
        public static bool TryGetAttribute<T>(this object value, out T attribute)
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
            var memberInfo = type.GetMember(value.ToString());
            return memberInfo[0].GetCustomAttributes(typeof(T), false);
        }

        public static string GetWhitespaces(int count = 1) => new string(' ', Math.Max(0, count * 4));
    }
}