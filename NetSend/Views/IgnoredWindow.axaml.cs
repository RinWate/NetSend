using Avalonia.Controls;
using NetSend.Models;
using NetSend.ViewModels;

namespace NetSend.Views;

public partial class IgnoredWindow : Window {

	private IgnoredWindowViewModel viewModel;

	public IgnoredWindow() {
		InitializeComponent();
		viewModel = new IgnoredWindowViewModel();
		DataContext = viewModel;
	}

	private void DataGrid_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e) {
		if (sender is DataGrid data) {
			viewModel.SelectedRecipients.Clear();
			foreach (var item in data.SelectedItems) {
				viewModel.SelectedRecipients.Add((IgnoredRecipient)item);
			}
		}
	}
}