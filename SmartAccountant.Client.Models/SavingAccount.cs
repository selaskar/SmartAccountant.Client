using System.ComponentModel.DataAnnotations;
using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Client.Models;

public class SavingAccount : Account
{
    public override BalanceType NormalBalance => BalanceType.Debit;

    public Currency Currency { get; set; }

    [Required]
    public required string AccountNumber { get; init; }
}
