using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartAccountant.ApiClient.Abstract;
using SmartAccountant.Maui.Models;
using SmartAccountant.Models;

namespace SmartAccountant.Maui.PageModels;

public partial class ProjectDetailPageModel(ICoreServiceClient serviceClient, ModalErrorHandler errorHandler) 
    : ObservableObject, IQueryAttributable, IProjectTaskPageModel
{
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private ObservableCollection<Transaction> transactions = [];

    public Account? Account { get; set; }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out object? value))
        {
            var id = Guid.Parse((string)value);
            LoadData(id).FireAndForgetSafeAsync(errorHandler);
        }
        else if (query.ContainsKey("refresh"))
        {
            RefreshData().FireAndForgetSafeAsync(errorHandler);
        }
    }

    private async Task RefreshData()
    {

    }

    private async Task LoadData(Guid accountId)
    {
        try
        {
            IsBusy = true;

            Transactions = (await serviceClient.GetTransactions(accountId))
                .OrderBy(x => x.ReferenceNumber)
                .ThenByDescending(x => Math.Abs(x.Amount.Amount))
                .ToObservableCollection();
        }
        catch (Exception e)
        {
            errorHandler.HandleError(e);
        }
        finally
        {
            IsBusy = false;
        }
    }


    [RelayCommand]
    private async Task Save()
    {
        await Shell.Current.GoToAsync("..");
        await AppShell.DisplayToastAsync("Project saved");
    }

    [RelayCommand]
    private async Task AddTask()
    {
        // Pass the project so if this is a new project we can just add
        // the tasks to the project and then save them all from here.
        await Shell.Current.GoToAsync($"task",
            new ShellNavigationQueryParameters(){
                {TaskDetailPageModel.ProjectQueryKey, Account}
            });
    }

    [RelayCommand]
    private async Task Delete()
    {
        await Shell.Current.GoToAsync("..");
        await AppShell.DisplayToastAsync("Project deleted");
    }

    [RelayCommand]
    private Task NavigateToTask(ProjectTask task) =>
        Shell.Current.GoToAsync($"task?id={task.ID}");

}
