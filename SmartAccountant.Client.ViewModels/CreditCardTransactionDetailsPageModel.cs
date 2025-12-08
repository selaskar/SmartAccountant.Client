using SmartAccountant.ApiClient.Abstract;
using SmartAccountant.Client.Core.Abstract;
using SmartAccountant.Client.Core.Extensions;
using SmartAccountant.Client.ViewModels.Services;
using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Client.ViewModels;

public partial class CreditCardTransactionDetailsPageModel(IErrorHandler errorHandler,
    INavigationService navigationService,
    ICoreServiceClient coreServiceClient)
    : TransactionDetailsPageModel(errorHandler, navigationService, coreServiceClient)
{
    public IList<EnumMember<ProvisionState>> ProvisionStates { get; } = Enum.GetValues<ProvisionState>().ToEnumMembers();
}
