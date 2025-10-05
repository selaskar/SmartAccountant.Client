using System.Globalization;
using SmartAccountant.Models;

namespace SmartAccountant.Client.MAUI.Converters;

internal class TransactionAmountConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Transaction transaction)
            return null;

        if (transaction.Amount.Amount == 0)
            return Colors.Black;

        if ((transaction.Account?.NormalBalance == BalanceType.Debit && transaction.Amount.Amount < 0)
            || (transaction.Account?.NormalBalance == BalanceType.Credit && transaction.Amount.Amount > 0))
            return Colors.Red;

        return Colors.Green;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
