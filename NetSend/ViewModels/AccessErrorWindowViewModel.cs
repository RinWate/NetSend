using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.IO;

namespace NetSend.ViewModels {
	public partial class AccessErrorWindowViewModel : ViewModelBase {

		[RelayCommand]
		public void TryAgain() { 
			var directory = Directory.GetCurrentDirectory();
			if (Directory.Exists(directory)) { 
				Process.Start(directory + "\\NetSend.exe");
			}
			var window = Global.GetMainWindow();
			window?.Close();
		}

		[RelayCommand]
		public void CloseApp() {
			var window = Global.GetMainWindow();
			window?.Close();
		}
	}
}
