using Avalonia.Controls;
using NetSend.ViewModels;
using Ursa.Controls;

namespace NetSend.Views {
	public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();
			var viewModel = new MainWindowViewModel();
			viewModel._toastManager = new WindowToastManager(this);
			DataContext = viewModel;
		}
	}
}