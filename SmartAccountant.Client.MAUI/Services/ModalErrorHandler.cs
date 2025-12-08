using SmartAccountant.Client.MAUI.Resources.Text;
using SmartAccountant.Client.ViewModels.Services;

namespace SmartAccountant.Client.MAUI.Services;

/// <summary>
/// Modal Error Handler.
/// </summary>
internal sealed partial class ModalErrorHandler : IErrorHandler, IDisposable
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <summary>
    /// Handle error in UI.
    /// </summary>
    /// <param name="ex">Exception.</param>
    public void HandleError(Exception ex)
    {
        _ = DisplayAlert(ex);
    }

    async Task DisplayAlert(Exception ex)
    {
        try
        {
            await _semaphore.WaitAsync();
            if (Shell.Current is Shell shell)
                await shell.DisplayAlert(MessageResources.Error, ex.Message, MessageResources.OK);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Dispose()
    {
        ((IDisposable)_semaphore).Dispose();
    }
}