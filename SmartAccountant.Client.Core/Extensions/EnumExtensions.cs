using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SmartAccountant.Client.Core.Extensions;

public static class EnumExtensions
{
    public static IList<EnumMember<T>> ToEnumMembers<T>(this IEnumerable<T> source) where T : Enum
    {
        // SfComboBox's SelectedValue binding doesn't work with an IEnumerable<> source.
        // So we use IList<> instead.
        List<EnumMember<T>> result = [];

        foreach (T item in source)
        {
            FieldInfo? fi = item.GetType().GetField(item.ToString()!);
            DisplayAttribute? displayAttribute = fi?.GetCustomAttribute<DisplayAttribute>(false);

            result.Add(new EnumMember<T>
            {
                Value = item,
                DisplayName = displayAttribute?.GetName() ?? item.ToString()
            });
        }

        return result;
    }
}
public readonly record struct EnumMember<T> where T : Enum
{
    public readonly T Value { get; init; }

    public readonly string DisplayName { get; init; }
}
