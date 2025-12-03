using System.ComponentModel.DataAnnotations;
using SmartAccountant.Client.Models.Resources;
using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Client.Models;

public abstract class AbstractCreditCard : Account
{
    public override BalanceType NormalBalance => BalanceType.Credit;

    [Required(ErrorMessageResourceName = nameof(CommonStrings.Required_Field_Missing), ErrorMessageResourceType = typeof(CommonStrings))]
    public required string CardNumber { get; init; }
}
