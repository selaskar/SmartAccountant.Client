using System.Collections.ObjectModel;

namespace SmartAccountant.Client.Core.Extensions;

public static class IEnumerableExtensions
{
    public static ObservableCollection<T> ToObservable<T>(this IEnumerable<T> collection)
    {
        return new ObservableCollection<T>(collection);
    }
}
