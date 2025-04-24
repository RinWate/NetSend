using Avalonia.Controls;
using Avalonia.Interactivity;

namespace NetSend;

public partial class SendByAddressDialog : UserControl {
	public SendByAddressDialog() {
		InitializeComponent();
	}

	protected override void OnLoaded(RoutedEventArgs e) {
		tb_address.Focus();
		base.OnLoaded(e);
	}
}