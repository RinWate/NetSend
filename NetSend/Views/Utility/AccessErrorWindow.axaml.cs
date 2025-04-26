using Avalonia.Controls;
using NetSend.ViewModels;

namespace NetSend;

public partial class AccessErrorWindow : Window {
	public AccessErrorWindow() {
		InitializeComponent();
		DataContext = new AccessErrorWindowViewModel();
	}
}