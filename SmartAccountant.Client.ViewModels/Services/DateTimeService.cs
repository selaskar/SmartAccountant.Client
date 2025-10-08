using SmartAccountant.Client.Core.Abstract;

namespace SmartAccountant.Client.ViewModels.Services;

internal class DateTimeService : IDateTimeService
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
