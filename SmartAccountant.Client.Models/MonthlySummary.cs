namespace SmartAccountant.Client.Models;

public class MonthlySummary : BaseModel
{
    public DateOnly Month { get; init; }

    public IList<CurrencySummary> Currencies { get; init; } = [];
}
