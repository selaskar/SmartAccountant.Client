namespace SmartAccountant.Client.Core.Abstract;

public interface IDateTimeService
{
    DateTimeOffset UtcNow { get; }
}
