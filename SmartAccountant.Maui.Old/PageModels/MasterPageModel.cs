using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using MAUI.MSALClient;
using SmartAccountant.ApiClient.Abstract;
using SmartAccountant.Client.Core.Extensions;
using SmartAccountant.Maui.Models;
using SmartAccountant.Models;

namespace SmartAccountant.Maui.PageModels;

public partial class MasterPageModel : ObservableObject
{
    private readonly ICoreServiceClient serviceClient;
    private readonly ICurrentUser currentUser;

    public MasterPageModel(ICurrentUser currentUser, ICoreServiceClient serviceClient)
    {
        this.serviceClient = serviceClient;
        this.currentUser = currentUser;

        FetchUserDetails();


        Months = Enumerable.Range(0, 12).Select(static x =>
            new DashboardMonth
            {
                Label = DateTime.Today.AddMonths(-x).ToString("MMM-yy"),
                Value = DateOnly.FromDateTime(DateTime.Today).AddMonths(-x)
            }).ToObservableCollection();

        SelectedMonth = Months.First();
    }

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string? userName;

    private async void FetchUserDetails()
    {
        IsBusy = true;

        try
        {
            UserName = (await currentUser.Account)?.GetDisplayName();
        }
        catch (Exception)
        {
            await PublicClientSingleton.Instance.SignOutAsync();
            UserName = null;
            //TODO: log, redirect to sign-in page
        }
        finally
        {
            IsBusy = false;
        }
    }


    [ObservableProperty]
    private ObservableCollection<DashboardMonth> months;

    [ObservableProperty]
    private DashboardMonth selectedMonth;

    partial void OnSelectedMonthChanged(DashboardMonth value)
    {
        _ = FetchSummary(value.Value);
    }

    private async Task FetchSummary(DateOnly month)
    {
        IsBusy = true;

        try
        {
            Summary = await serviceClient.GetMonthlySummary(month);
        }
        catch(Exception ex)
        {
            //TODO:
        }
        finally
        {
            IsBusy = false;
        }
    }

    [ObservableProperty]
    private MonthlySummary? summary;
}
