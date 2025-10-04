using System.Globalization;
using SmartAccountant.Models;

namespace SmartAccountant.Client.MAUI.Converters;

internal class MonetaryValueConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not MonetaryValue monetaryValue)
            return null;

        string amountString = monetaryValue.Amount.ToString("F2");

        string? currencySymbol = monetaryValue.Currency switch
        {
            Currency.USD => "$",
            Currency.EUR => "€",
            Currency.TRY => "₺",
            _ => null
        };

        return currencySymbol != null ? $"{currencySymbol} {amountString}"
            : $"{amountString} {monetaryValue.Currency}";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}