using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using SmartAccountant.ApiClient.Abstract;
using SmartAccountant.ApiClient.Exceptions;
using SmartAccountant.Client.Core.Extensions;
using SmartAccountant.Client.Models;
using SmartAccountant.Client.ViewModels.Services;

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
                Label = DateTime.Today.AddMonths(-x).ToString("MMM-yy", CultureInfo.CurrentCulture),
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
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

            Summary = await _serviceClient.GetMonthlySummary(month, cts.Token);
        }
        catch (CoreServiceException ex)
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
