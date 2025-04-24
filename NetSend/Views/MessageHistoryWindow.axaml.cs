using Avalonia.Controls;
using NetSend.ViewModels;

namespace NetSend;

public partial class MessageHistoryWindow : Window {
	public MessageHistoryWindow(MainWindowViewModel mainViewModel) {
		InitializeComponent();
		DataContext = new MessageHistoryWindowViewModel(mainViewModel);
	}
}