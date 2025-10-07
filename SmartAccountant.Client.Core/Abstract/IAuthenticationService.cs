namespace SmartAccountant.Client.Core.Abstract;

public interface IAuthenticationService
{
    /// <exception cref="OperationCanceledException"/>
    Task SignIn(CancellationToken cancellationToken);

    Task SignOut();
}
