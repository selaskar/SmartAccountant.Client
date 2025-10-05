using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SmartAccountant.ApiClient.Abstract;
using SmartAccountant.Client.Core.Extensions;
using SmartAccountant.Client.Models;
using SmartAccountant.Client.ViewModels.Services;
using SmartAccountant.Models;

namespace SmartAccountant.Client.ViewModels;

public partial class SummaryPageModel : ViewModelBase
{
    private readonly ICoreServiceClient _serviceClient;
    private readonly IErrorHandler _errorHandler;

    public SummaryPageModel(ICoreServiceClient serviceClient, IErrorHandler errorHandler)
    {
        _serviceClient = serviceClient;
        _errorHandler = errorHandler;

        Months = Enumerable.Range(0, 12).Select(static x =>
            new DashboardMonth
            {
                Label = DateTime.Today.AddMonths(-x).ToString("MMM-yy"),
                Value = DateOnly.FromDateTime(DateTime.Today).AddMonths(-x)
            }).ToObservable();

        SelectedMonth = Months.First();
    }

    [ObservableProperty]
    public partial ObservableCollection<DashboardMonth> Months { get; set; }

    [ObservableProperty]
    public partial DashboardMonth SelectedMonth { get; set; }

    partial void OnSelectedMonthChanged(DashboardMonth value)
    {
        _ = FetchSummary(value.Value);
    }

    private async Task FetchSummary(DateOnly month)
    {
        IsBusy = true;

        try
        {
            Summary = await _serviceClient.GetMonthlySummary(month);
        }
        catch (Exception ex)
        {
            _errorHandler.HandleError(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [ObservableProperty]
    public partial MonthlySummary? Summary { get; set; }
}
