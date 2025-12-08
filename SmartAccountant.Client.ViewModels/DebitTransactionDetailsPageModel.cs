using SmartAccountant.ApiClient.Abstract;
using SmartAccountant.Client.Core.Abstract;
using SmartAccountant.Client.ViewModels.Services;

namespace SmartAccountant.Client.ViewModels;

public partial class DebitTransactionDetailsPageModel(IErrorHandler errorHandler, INavigationService navigationService,
    ICoreServiceClient coreServiceClient)
    : TransactionDetailsPageModel(errorHandler, navigationService, coreServiceClient)
{

}
