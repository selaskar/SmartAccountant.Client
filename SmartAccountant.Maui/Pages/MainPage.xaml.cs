using SmartAccountant.Maui.Models;
using SmartAccountant.Maui.PageModels;

namespace SmartAccountant.Maui.Pages;

public partial class MainPage : ContentPage
{
	public MainPage(MainPageModel model)
	{
		InitializeComponent();
		BindingContext = model;
	}
}