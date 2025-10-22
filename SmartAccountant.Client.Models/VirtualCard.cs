namespace SmartAccountant.Client.Models;

public class VirtualCard : AbstractCreditCard
{
    public Guid ParentId { get; init; }

    public Account? Parent { get; init; }
}
