using SmartAccountant.ApiClient.Abstract;
using SmartAccountant.Client.Core.Abstract;
using SmartAccountant.Client.Core.Extensions;
using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Client.ViewModels;

public partial class CreditCardTransactionDetailsPageModel(INavigationService navigationService, ICoreServiceClient coreServiceClient)
    : TransactionDetailsPageModel(navigationService, coreServiceClient)
{
    public IList<EnumMember<ProvisionState>> ProvisionStates { get; } = Enum.GetValues<ProvisionState>().ToEnumMembers();
}
