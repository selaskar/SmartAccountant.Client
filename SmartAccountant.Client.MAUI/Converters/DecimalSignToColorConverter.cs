using System.Globalization;

namespace SmartAccountant.Client.MAUI.Converters;

internal class DecimalSignToColorConverter : IValueConverter
{
    public Color? PositiveColor { get; set; }

    public Color? NegativeColor { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not decimal d)
            return null;

        return d == 0 ? Colors.Black : d >= 0 ? PositiveColor : NegativeColor;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
