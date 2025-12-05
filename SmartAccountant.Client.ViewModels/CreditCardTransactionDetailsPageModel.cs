using Microsoft.Extensions.Logging;
using SmartAccountant.ApiClient.Abstract;
using SmartAccountant.Client.Core.Abstract;
using SmartAccountant.Client.Core.Extensions;
using SmartAccountant.Client.ViewModels.Services;
using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Client.ViewModels;

public partial class CreditCardTransactionDetailsPageModel(ILogger<CreditCardTransactionDetailsPageModel> logger,
    IErrorHandler errorHandler,
    INavigationService navigationService,
    ICoreServiceClient coreServiceClient)
    : TransactionDetailsPageModel(logger, errorHandler, navigationService, coreServiceClient)
{
    public IList<EnumMember<ProvisionState>> ProvisionStates { get; } = Enum.GetValues<ProvisionState>().ToEnumMembers();
}
