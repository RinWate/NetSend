using Avalonia.Controls;
using NetSend.ViewModels;

namespace NetSend;

public partial class AboutWindow : Window {
	public AboutWindow() {
		InitializeComponent();
		DataContext = new AboutWindowViewModel(this);
	}
}