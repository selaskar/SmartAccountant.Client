namespace SmartAccountant.Maui.Models;

public record DashboardMonth
{
    public required string Label { get; init; }

    public DateOnly Value { get; init; }
}