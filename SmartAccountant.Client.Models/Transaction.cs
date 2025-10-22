using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Security.Principal;
using CommunityToolkit.Mvvm.ComponentModel;
using SmartAccountant.Client.Models.Resources;
using SmartAccountant.Shared.Structs;

namespace SmartAccountant.Client.Models;

public partial class Transaction : BaseModel
{
    public Guid? AccountId { get; set; }

    public Account? Account { get; set; }

    [ObservableProperty, NotifyDataErrorInfo]
    [StringLength(100)]
    [Display(Name = nameof(Strings.Transaction_ReferenceNumber), Description = nameof(Strings.Transaction_ReferenceNumber_Description), ResourceType = typeof(Strings))]
    public partial string? ReferenceNumber { get; set; }

    [ObservableProperty, NotifyDataErrorInfo]
    [Range(typeof(DateTimeOffset), "2000.01.01", "2100.01.01")]
    public partial DateTimeOffset Timestamp { get; set; }

    [ObservableProperty]
    //TODO: maximum and minimum value validation
    public partial MonetaryValue Amount { get; set; }

    [ObservableProperty, NotifyDataErrorInfo]
    [StringLength(500)]
    [Display(Name = nameof(Strings.Transaction_PersonalNote), ResourceType = typeof(Strings))]
    public partial string? PersonalNote { get; set; }

    [ObservableProperty, NotifyDataErrorInfo]
    [StringLength(500)]
    public partial string? Description { get; set; }

    [ObservableProperty]
    public partial TransactionCategory Category { get; set; }

    private protected override void CopyValuesFrom([NotNull] BaseModel other)
    {
        base.CopyValuesFrom(other);

        if (other is not Transaction transaction)
            throw new ArgumentException($"{other.GetType().Name} was expected to be a {typeof(Transaction).Name}");

        AccountId = transaction.AccountId;
        Account = transaction.Account;
        ReferenceNumber = transaction.ReferenceNumber;
        Timestamp = transaction.Timestamp;
        Amount = transaction.Amount;
        Description = transaction.Description;
        PersonalNote = transaction.PersonalNote;
        Category = transaction.Category;
    }
}
