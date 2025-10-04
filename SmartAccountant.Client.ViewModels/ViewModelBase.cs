using CommunityToolkit.Mvvm.ComponentModel;

namespace SmartAccountant.Client.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    public partial bool IsBusy { get; set; }

}
