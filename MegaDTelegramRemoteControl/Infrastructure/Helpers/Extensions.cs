using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace MegaDTelegramRemoteControl.Infrastructure.Helpers;

public static class Extensions
{
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        return field == null ||
               Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is not DescriptionAttribute attribute
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

    public static bool HasTypeAttribute<T>(this object value) => value.GetType()
                                                                      .GetCustomAttributes(true)
                                                                      .Any(t => t.GetType() == typeof(T));

    public static List<T> GetEnumList<T>() => Enum.GetValues(typeof(T)).Cast<T>().ToList();

    public static bool TryGetAttribute<T>(this object value, [MaybeNullWhen(false)] out T attribute)
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
            ? Array.Empty<object>()
            : memberInfo[0].GetCustomAttributes(typeof(T), false);
    }

    public static string GetWhitespaces(int count = 1) => new(' ', Math.Max(0, count * 4));

    public static string ToDateString(this DateTime date) => date.ToString("dd.MM.yyyy HH:mm:ss");

    public static string ToSignificantDigit(this decimal value, int digits = 8) => digits switch
    {
        0 => value.ToString("0", CultureInfo.InvariantCulture),
        2 => value.ToString("0.##", CultureInfo.InvariantCulture),
        4 => value.ToString("0.####", CultureInfo.InvariantCulture),
        8 => value.ToString("0.########", CultureInfo.InvariantCulture),
        _ => value.ToString($"0.{new string('#', Math.Max(0, digits))}", CultureInfo.InvariantCulture)
    };

    public static string GetDateDurationString(this TimeSpan diff, int? maxDaysForNoDate = null)
    {
        if (maxDaysForNoDate.HasValue && diff.TotalDays > maxDaysForNoDate.Value)
            return "no date";

        var times = new[]
                    {
                        (diff.Days, $"{diff.Days} day{(diff.Days > 1 ? "s" : "")}"),
                        (diff.Hours, $"{diff.Hours} hour{(diff.Hours > 1 ? "s" : "")}"),
                        (diff.Minutes, $"{diff.Minutes} min"),
                        (diff.Seconds, $"{diff.Seconds} sec"),
                    };

        return times.Any(x => x.Item1 > 0)
            ? string.Join(" ", times.Where(x => x.Item1 > 0).Select(x => x.Item2))
            : "-";
    }
}