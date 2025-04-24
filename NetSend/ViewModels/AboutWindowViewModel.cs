using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;

namespace NetSend.ViewModels {
	public partial class AboutWindowViewModel : ViewModelBase {

		public AboutWindowViewModel(Window? parent) {
			if (parent != null) window = parent;
		}

		[RelayCommand]
		public void CloseWindow() {
			window?.Close();
		}

	}
}
