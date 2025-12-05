using Microsoft.Extensions.Logging;
using SmartAccountant.ApiClient.Abstract;
using SmartAccountant.Client.Core.Abstract;
using SmartAccountant.Client.ViewModels.Services;

namespace SmartAccountant.Client.ViewModels;

public partial class DebitTransactionDetailsPageModel(ILogger<DebitTransactionDetailsPageModel> logger,
    IErrorHandler errorHandler, INavigationService navigationService,
    ICoreServiceClient coreServiceClient)
    : TransactionDetailsPageModel(logger, errorHandler, navigationService, coreServiceClient)
{

}
