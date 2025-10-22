using System.ComponentModel.DataAnnotations;
using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Client.Models;

public abstract class AbstractCreditCard : Account
{
    public override BalanceType NormalBalance => BalanceType.Credit;

    [Required]
    public required string CardNumber { get; init; }
}
