using SmartAccountant.Client.ViewModels;

namespace SmartAccountant.Client.MAUI.Pages;

public partial class SummaryPage : ContentPage
{
    public SummaryPage(SummaryPageModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}