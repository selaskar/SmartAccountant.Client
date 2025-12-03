using CommunityToolkit.Mvvm.ComponentModel;

namespace SmartAccountant.Client.ViewModels;

public partial class ViewModelBase : ObservableValidator
{
    [ObservableProperty]
    public virtual partial bool IsBusy { get; set; }
}
