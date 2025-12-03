using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using SmartAccountant.Client.Core.Attributes;
using SmartAccountant.Client.Models.Resources;
using SmartAccountant.Shared;
using SmartAccountant.Shared.Structs;

namespace SmartAccountant.Client.Models;

public partial class Transaction : BaseModel
{
    public Guid? AccountId { get; set; }

    public Account? Account { get; set; }

    [ObservableProperty, NotifyDataErrorInfo]
    [StringLength(100, ErrorMessageResourceName = nameof(CommonStrings.Max_Length_Error), ErrorMessageResourceType = typeof(CommonStrings))]
    [Display(Name = nameof(ModelStrings.Transaction_ReferenceNumber), Description = nameof(ModelStrings.Transaction_ReferenceNumber_Description), ResourceType = typeof(ModelStrings))]
    public partial string? ReferenceNumber { get; set; }

    [ObservableProperty, NotifyDataErrorInfo]
    [Range(typeof(DateTimeOffset), ApplicationDefinitions.EpochStartString, ApplicationDefinitions.EpochEndString)]
    public partial DateTimeOffset Timestamp { get; set; }

    [ObservableProperty, NotifyDataErrorInfo]
    [MonetaryValue(ApplicationDefinitions.MinTransactionAmount, ApplicationDefinitions.MaxTransactionAmount, ErrorMessageResourceName = nameof(CommonStrings.Min_Max_Value_Error), ErrorMessageResourceType = typeof(CommonStrings))]
    [Display(Name = nameof(ModelStrings.Transaction_Amount), ResourceType = typeof(ModelStrings))]
    public partial MonetaryValue Amount { get; set; }

    [ObservableProperty, NotifyDataErrorInfo]
    [StringLength(500, ErrorMessageResourceName = nameof(CommonStrings.Max_Length_Error), ErrorMessageResourceType = typeof(CommonStrings))]
    [Display(Name = nameof(ModelStrings.Transaction_PersonalNote), ResourceType = typeof(ModelStrings))]
    public partial string? PersonalNote { get; set; }

    [ObservableProperty, NotifyDataErrorInfo]
    [Required(ErrorMessageResourceName = nameof(CommonStrings.Required_Field_Missing), ErrorMessageResourceType = typeof(CommonStrings))]
    [StringLength(500, ErrorMessageResourceName = nameof(CommonStrings.Max_Length_Error), ErrorMessageResourceType = typeof(CommonStrings))]
    public partial string Description { get; set; }

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
