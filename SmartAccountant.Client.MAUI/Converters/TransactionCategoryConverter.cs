using System.Globalization;
using SmartAccountant.Models;

namespace SmartAccountant.Client.MAUI.Converters;

internal class TransactionCategoryConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not TransactionCategory category)
            return null;

        return category.Category switch
        {
            MainCategory.None => Colors.Navy,
            MainCategory.Expense => Colors.LightCyan,
            MainCategory.Income => Colors.Tan,
            MainCategory.Saving => Colors.Snow,
            MainCategory.InterestOrFee => Colors.Tomato,
            _ => Colors.Black
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
