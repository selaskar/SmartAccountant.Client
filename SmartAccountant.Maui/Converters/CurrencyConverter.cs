using System.Globalization;
using SmartAccountant.Models;

namespace SmartAccountant.Maui.Converters;

public class CurrencyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if(value is not Currency currency)
            return null;

        return currency switch
        {
            Currency.USD => "$",
            Currency.EUR => "€",
            Currency.TRY => "₺",
            _ => value
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}