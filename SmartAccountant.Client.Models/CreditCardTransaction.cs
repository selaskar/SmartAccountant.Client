using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Client.Models;

public class CreditCardTransaction : Transaction
{
    public ProvisionState ProvisionState { get; set; }
}
