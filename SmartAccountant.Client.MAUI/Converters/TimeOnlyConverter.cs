using System.Globalization;

namespace SmartAccountant.Client.MAUI.Converters;

internal class TimeOnlyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
            return null;

        if (value is not TimeOnly time)
            return null;

        if (targetType != typeof(TimeSpan))
            return null;

        return time.ToTimeSpan();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
            return null;

        if (value is TimeOnly time)
            return time;

        if (value is TimeSpan timeSpan)
            return TimeOnly.FromTimeSpan(timeSpan);

        return null;
    }
}
