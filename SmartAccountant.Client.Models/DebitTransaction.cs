using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using SmartAccountant.Client.Core.Attributes;
using SmartAccountant.Client.Models.Resources;
using SmartAccountant.Shared;
using SmartAccountant.Shared.Structs;

namespace SmartAccountant.Client.Models;

public partial class DebitTransaction : Transaction
{
    /// <summary>
    /// Balance after the transaction
    /// </summary
    [ObservableProperty, NotifyDataErrorInfo]
    [MonetaryValue(ApplicationDefinitions.MinTransactionAmount, ApplicationDefinitions.MaxTransactionAmount, ErrorMessageResourceName = nameof(CommonStrings.Min_Max_Value_Error), ErrorMessageResourceType = typeof(CommonStrings))]
    [Display(Name = nameof(ModelStrings.DebitTransaction_RemainingAmount), ResourceType = typeof(ModelStrings))]
    public partial MonetaryValue RemainingBalance { get; set; }

    private protected override void CopyValuesFrom([NotNull] BaseModel other)
    {
        base.CopyValuesFrom(other);

        if (other is not DebitTransaction debitTransaction)
            throw new ArgumentException($"{other.GetType().Name} was expected to be a {typeof(DebitTransaction).Name}");

        RemainingBalance = debitTransaction.RemainingBalance;
    }
}
