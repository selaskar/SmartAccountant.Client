using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentValidation;
using FluentValidation.Results;
using SmartAccountant.Models;

namespace SmartAccountant.Client.ViewModels;

public partial class TransactionDetailsPageModel : ViewModelBase, IQueryAttributable
{
    public const string TransactionObjectKey = "Transaction";

    private static readonly AbstractValidator<Transaction> _validator = new TransactionValidator();

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        Transaction = (Transaction)query[TransactionObjectKey];

        Transaction.BeginEdit();
    }

    [ObservableProperty]
    public partial Transaction Transaction { get; set; }

    [ObservableProperty]
    public partial ValidationResult? ValidationResult { get; set; }

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

        Transaction.EndEdit();
    }

    private bool CanSave() => ValidationResult?.IsValid == true;
}
