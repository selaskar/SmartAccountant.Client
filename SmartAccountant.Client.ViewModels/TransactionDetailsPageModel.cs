
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SmartAccountant.Models;

namespace SmartAccountant.Client.ViewModels;

public partial class TransactionDetailsPageModel : ViewModelBase, IQueryAttributable
{

    public const string TransactionObjectKey = "Transaction";

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        Transaction = (Transaction)query[TransactionObjectKey];
    }

    [ObservableProperty]
    public partial Transaction Transaction { get; set; }

    partial void OnTransactionChanged(Transaction value)
    {
        Amount = value.Amount;

        Date = DateOnly.FromDateTime(value.Timestamp.Date);
        Time = TimeOnly.FromDateTime(value.Timestamp.DateTime);
        Offset = value.Timestamp.Offset;

        SelectedMainCategory = value.Category.Category;
    }

    [ObservableProperty]
    public partial MonetaryValue Amount { get; set; }

    [ObservableProperty]
    public partial DateOnly Date { get; set; }

    [ObservableProperty]
    public partial TimeOnly Time { get; set; }

    [ObservableProperty]
    public partial TimeSpan Offset { get; set; }

    public ReadOnlyCollection<TimeZoneInfo> TimeZones { get; } = TimeZoneInfo.GetSystemTimeZones();

    [ObservableProperty]
    public partial TimeZoneInfo SelectedTimeZone { get; set; }

    partial void OnSelectedTimeZoneChanged(TimeZoneInfo value)
    {
        this.Offset = value.GetUtcOffset(new DateTime(Date, Time));
    }


    public IEnumerable<MainCategory> MainCategories { get; } = Enum.GetValues<MainCategory>();

    [ObservableProperty]
    public partial MainCategory SelectedMainCategory { get; set; }
}

