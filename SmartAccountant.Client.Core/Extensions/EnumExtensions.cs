using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SmartAccountant.Client.Core.Extensions;

public static class EnumExtensions
{
    public static IEnumerable<EnumMember<T>> ToEnumMembers<T>(this IEnumerable<T> source) where T : Enum
    {
        foreach (T item in source)
        {
            FieldInfo? fi = item.GetType().GetField(item.ToString()!);
            DisplayAttribute? displayAttribute = fi?.GetCustomAttribute<DisplayAttribute>(false);

            yield return new EnumMember<T>
            {
                Value = item,
                DisplayName = displayAttribute?.GetName() ?? item.ToString()
            };
        }
    }
}
public readonly record struct EnumMember<T> where T : Enum
{
    public readonly T Value { get; init; }

    public readonly string DisplayName { get; init; }
}
