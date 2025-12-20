using System.Globalization;

namespace SmartAccountant.Client.MAUI.Converters;

internal class DateOnlyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
            return null;

        if (value is not DateOnly date)
            return null;

        if (targetType != typeof(DateTime?))
            return null;

        return new DateTime(date, TimeOnly.MinValue, DateTimeKind.Unspecified);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
            return null;

        if (value is DateOnly date)
            return date;

        if (value is DateTime dateTime)
            return DateOnly.FromDateTime(dateTime);

        if (value is DateTimeOffset dateTimeOffset)
            return DateOnly.FromDateTime(dateTimeOffset.Date);

        return null;
    }
}
