using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartAccountant.Client.Models;
using SmartAccountant.Shared.Enums;
using SmartAccountant.Shared.Structs;
using Transaction = SmartAccountant.Client.Models.Transaction;

namespace SmartAccountant.Client.ViewModels;

public partial class TransactionDetailsPageModel : ViewModelBase, IQueryAttributable
{
    public const string TransactionObjectKey = "Transaction";

    /// <exception cref="ArgumentNullException"/>
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        ArgumentNullException.ThrowIfNull(query);

        Transaction = (Transaction)query[TransactionObjectKey];

        Transaction.BeginEdit();
    }

    [ObservableProperty]
    public partial Transaction? Transaction { get; set; }

    partial void OnTransactionChanged(Transaction? oldValue, Transaction? newValue)
    {
        oldValue?.ErrorsChanged -= Transaction_ErrorsChanged;
        newValue?.ErrorsChanged += Transaction_ErrorsChanged;

        Amount = newValue.Amount;

        Date = DateOnly.FromDateTime(newValue.Timestamp.Date);
        Time = TimeOnly.FromDateTime(newValue.Timestamp.DateTime);
        Offset = newValue.Timestamp.Offset;

        SelectedMainCategory = newValue.Category.Category;

        if (newValue.Category.Category == MainCategory.Expense)
            SelectedSubCategory = newValue.Category.SubCategory;
    }

    private void Transaction_ErrorsChanged(object? sender, System.ComponentModel.DataErrorsChangedEventArgs e)
    {
        SaveCommand.NotifyCanExecuteChanged();
    }

    [ObservableProperty]
    public partial MonetaryValue Amount { get; set; }

    [ObservableProperty]
    public partial DateOnly Date { get; set; }

    [ObservableProperty]
    public partial TimeOnly Time { get; set; }

    [ObservableProperty]
    [Range(typeof(TimeSpan), "-14:00:00", "14:00:00")]
    public partial TimeSpan Offset { get; set; }

    public ReadOnlyCollection<TimeZoneInfo> TimeZones { get; } = TimeZoneInfo.GetSystemTimeZones();

    [ObservableProperty]
    public partial TimeZoneInfo SelectedTimeZone { get; set; } = TimeZoneInfo.Local; //TODO: to be inferred from Account time zone

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

    public IEnumerable<EnumMember<ExpenseSubCategories>> ExpenseSubCategories { get; } = Enum.GetValues<ExpenseSubCategories>().ToEnumMembers();

    [ObservableProperty]
    public partial short SelectedSubCategory { get; set; } = -1;

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task Save()
    {
        Transaction!.Category = new TransactionCategory(SelectedMainCategory, SelectedSubCategory != -1 ? (byte)SelectedSubCategory : (byte)0);


        Transaction.EndEdit();
    }

    private bool CanSave() => !Transaction?.HasErrors ?? false;
}

public readonly record struct EnumMember<T> where T : Enum
{
    public readonly T Value { get; init; }

    public readonly string DisplayName { get; init; }
}

public static class EnumExtensions
{
    public static IEnumerable<EnumMember<T>> ToEnumMembers<T>(this IEnumerable<T> source) where T : Enum
    {
        foreach (T item in source)
        {
            FieldInfo? fi = item.GetType().GetField(item.ToString()!);
            DisplayAttribute? displayAttribute = fi?.GetCustomAttribute<DisplayAttribute>(false);

            yield return new EnumMember<T>
            {
                Value = item,
                DisplayName = displayAttribute?.GetName() ?? item.ToString()!
            };
        }
    }
}