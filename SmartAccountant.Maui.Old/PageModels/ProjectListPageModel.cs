using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUI.MSALClient;
using SmartAccountant.ApiClient.Abstract;
using SmartAccountant.Maui.Old.Resources;
using SmartAccountant.Models;

namespace SmartAccountant.Maui.PageModels;

public partial class ProjectListPageModel(ICoreServiceClient serviceClient, ModalErrorHandler errorHandler) : ObservableObject
{
    private readonly IErrorHandler errorHandler = errorHandler;

    [ObservableProperty]
	private List<Account> _projects = [];

    [RelayCommand]
	private async Task Appearing()
	{
		try
		{
            if (PublicClientSingleton.Instance.MSALClientHelper.AuthResult == null)
            {
                await Shell.Current.DisplayAlert(Message.Error, Message.UserNotAuthenticated, Message.Cancel);
                return;
            }

            Projects = [.. await serviceClient.GetAccounts()];
		}
		catch (Exception ex)
		{
			errorHandler.HandleError(ex);
		}
	}

	[RelayCommand]
	Task NavigateToProject(Account account)
		=> Shell.Current.GoToAsync($"project?id={account.Id}");

	[RelayCommand]
	async Task AddProject()
	{
		await Shell.Current.GoToAsync($"project");
	}
}