using Avalonia.Controls;
using NetSend.ViewModels;

namespace NetSend;

public partial class MessageSchedulerWindow : Window {
	private MessageSchedulerWindowViewModel _viewModel;
	public MessageSchedulerWindow() {
		InitializeComponent();
		_viewModel = new MessageSchedulerWindowViewModel();
		DataContext = _viewModel;
	}
}