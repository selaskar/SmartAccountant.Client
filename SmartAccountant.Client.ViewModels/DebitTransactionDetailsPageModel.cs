using SmartAccountant.ApiClient.Abstract;
using SmartAccountant.Client.Core.Abstract;

namespace SmartAccountant.Client.ViewModels;

public partial class DebitTransactionDetailsPageModel(INavigationService navigationService, ICoreServiceClient coreServiceClient) 
    : TransactionDetailsPageModel(navigationService, coreServiceClient)
{

}
