using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace NetSend.ViewModels;

public partial class AboutWindowViewModel : ViewModelBase {
	public AboutWindowViewModel(Window? parent) {
		if (parent != null) window = parent;
	}

	[RelayCommand]
	public void OpenGitHub() {
		Process.Start(new ProcessStartInfo {
			FileName = "https://github.com/RinWate",
			UseShellExecute = true
		});
	}

	[RelayCommand]
	public void CloseWindow() {
		window?.Close();
	}
}