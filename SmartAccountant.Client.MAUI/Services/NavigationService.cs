using SmartAccountant.Client.Core.Abstract;

namespace SmartAccountant.Client.MAUI.Services;

internal sealed partial class NavigationService : INavigationService
{
    public void NavigateBack()
    {
        Shell.Current.GoToAsync("..");
    }
}
