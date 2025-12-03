using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace SmartAccountant.Client.MAUI.Converters;

internal class EnumDisplayNameConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value?.GetType().IsEnum != true
            || value.ToString() == null)
            return null;

        FieldInfo? fi = value.GetType().GetField(value.ToString()!);
        DisplayAttribute? attribute = fi?.GetCustomAttribute<DisplayAttribute>();

        return attribute?.GetName() ?? value.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
