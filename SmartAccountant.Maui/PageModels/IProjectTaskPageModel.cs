using CommunityToolkit.Mvvm.Input;
using SmartAccountant.Maui.Models;

namespace SmartAccountant.Maui.PageModels;

public interface IProjectTaskPageModel
{
	IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
	bool IsBusy { get; }
}