using SmartAccountant.Shared.Structs;

namespace SmartAccountant.Client.Models;

public class CreditCardLimit : BaseModel
{
    public Guid CreditCardId { get; init; }

    public MonetaryValue Amount { get; init; }

    public Period Period { get; init; }
}
