using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SmartAccountant.Client.Core.Extensions;
using SmartAccountant.Client.Models;
using SmartAccountant.Shared.Enums;
using SmartAccountant.Shared.Structs;

namespace SmartAccountant.Client.ViewModels;

public partial class TransactionEditorViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial Transaction? Transaction { get; set; }

    partial void OnTransactionChanged(Transaction? oldValue, Transaction? newValue)
    {
        if (newValue == null)
        {
            Date = DateOnly.FromDateTime(DateTime.Today);
            Time = TimeOnly.FromDateTime(DateTime.Today);
            offset = default;
        }
        else
        {
            // Careful: Little hack to solve atomicity problem.
            // When any of these properties are assigned first, they fire a changed callback and overwrite other properties of the source (newValue).
            // Tuple syntax is to avoid defining additional local variables.
            (Date, Time, offset) = (
                DateOnly.FromDateTime(newValue.Timestamp.Date),
                TimeOnly.FromDateTime(newValue.Timestamp.DateTime),
                newValue.Timestamp.Offset);
        }

        // Same hack here.
        (SelectedMainCategory, SelectedSubCategory) = (newValue?.Category.Category ?? default, (sbyte?)newValue?.Category.SubCategory ?? (sbyte)-1);
    }

    #region Date/Time
    [ObservableProperty]
    public partial DateOnly Date { get; set; }

    partial void OnDateChanged(DateOnly value) => TimestampChangedCommon();

    [ObservableProperty]
    public partial TimeOnly Time { get; set; }

    partial void OnTimeChanged(TimeOnly value) => TimestampChangedCommon();

    private TimeSpan offset;

    public ReadOnlyCollection<TimeZoneInfo> TimeZones { get; } = TimeZoneInfo.GetSystemTimeZones();

    [ObservableProperty]
    public partial TimeZoneInfo SelectedTimeZone { get; set; } = TimeZoneInfo.Local; //TODO: to be inferred from Account time zone

    partial void OnSelectedTimeZoneChanged(TimeZoneInfo value)
    {
        offset = value.GetUtcOffset(new DateTime(Date, Time));
    }

    private void TimestampChangedCommon()
    {
        Transaction?.Timestamp = new DateTimeOffset(Date, Time, offset);
    }
    #endregion

    #region Category
    public IEnumerable<MainCategory> MainCategories { get; } = Enum.GetValues<MainCategory>();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ExpenseCategorySelected))]
    public partial MainCategory SelectedMainCategory { get; set; }

    partial void OnSelectedMainCategoryChanged(MainCategory value)
    {
        SelectedSubCategory = -1;

        CategoryChangedCommon();
    }

    public bool ExpenseCategorySelected => SelectedMainCategory == MainCategory.Expense;

    public IEnumerable<EnumMember<ExpenseSubCategories>> ExpenseSubCategories { get; } = Enum.GetValues<ExpenseSubCategories>().ToEnumMembers();

    [ObservableProperty]
    public partial sbyte SelectedSubCategory { get; set; } = -1;

    partial void OnSelectedSubCategoryChanged(sbyte value) => CategoryChangedCommon();

    private void CategoryChangedCommon()
    {
        Transaction?.Category = new TransactionCategory(SelectedMainCategory, SelectedSubCategory != -1 ? (byte)SelectedSubCategory : (byte)0);
    }
    #endregion
}
