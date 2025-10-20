using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartAccountant.Models;
using SmartAccountant.Models.Validators;

namespace SmartAccountant.Client.ViewModels;

public partial class TransactionDetailsPageModel : ViewModelBase, IQueryAttributable
{
    public const string TransactionObjectKey = "Transaction";

    /// <exception cref="ArgumentNullException"/>
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        ArgumentNullException.ThrowIfNull(query);

        Transaction = (Transaction)query[TransactionObjectKey];
        Transaction2 = new Models.Transaction();

        Transaction.BeginEdit();
    }

    [ObservableProperty]
    public partial Transaction Transaction { get; set; }

    [ObservableProperty]
    public partial Models.Transaction? Transaction2 { get; set; }

    partial void OnTransactionChanged(Transaction value)
    {
        Amount = value.Amount;

        Date = DateOnly.FromDateTime(value.Timestamp.Date);
        Time = TimeOnly.FromDateTime(value.Timestamp.DateTime);
        Offset = value.Timestamp.Offset;

        SelectedMainCategory = value.Category.Category;

        if (value.Category.Category == MainCategory.Expense)
            SelectedSubCategory = value.Category.SubCategory;
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
    [NotifyPropertyChangedFor(nameof(ExpenseCategorySelected))]
    public partial MainCategory SelectedMainCategory { get; set; }

    partial void OnSelectedMainCategoryChanged(MainCategory value)
    {
        SelectedSubCategory = -1;
    }

    public bool ExpenseCategorySelected => SelectedMainCategory == MainCategory.Expense;

    public IEnumerable<ExpenseSubCategories> ExpenseSubCategories { get; } = Enum.GetValues<ExpenseSubCategories>();

    [ObservableProperty]
    public partial short SelectedSubCategory { get; set; } = -1;

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task Save()
    {
        Transaction.Category = new TransactionCategory(SelectedMainCategory, SelectedSubCategory != -1 ? (byte)SelectedSubCategory : (byte)0);

        if (!CanSave())
            return;

        Transaction.EndEdit();
    }

    private bool CanSave() => !Transaction2?.HasErrors ?? false;
}
