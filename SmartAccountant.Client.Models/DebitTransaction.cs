using SmartAccountant.Shared.Structs;

namespace SmartAccountant.Client.Models;

public class DebitTransaction : Transaction
{
    /// <summary>
    /// Balance after the transaction
    /// </summary>
    public MonetaryValue RemainingBalance { get; set; }
}
