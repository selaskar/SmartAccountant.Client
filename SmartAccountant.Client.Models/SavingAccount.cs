using System.ComponentModel.DataAnnotations;
using SmartAccountant.Client.Models.Resources;
using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Client.Models;

public class SavingAccount : Account
{
    public override BalanceType NormalBalance => BalanceType.Debit;

    public Currency Currency { get; set; }

    [Required(ErrorMessageResourceName = nameof(CommonStrings.Required_Field_Missing), ErrorMessageResourceType = typeof(CommonStrings))]
    public required string AccountNumber { get; init; }
}
