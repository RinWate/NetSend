using System;
using Avalonia.Controls;
using NetSend.Core;
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

	private void DataGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e) {
		if (sender is DataGrid data) {
			viewModel.SelectedRecipients.Clear();
			foreach (var item in data.SelectedItems) {
				viewModel.SelectedRecipients.Add((IgnoredRecipient)item);
			}
		}
	}

	private void DataGrid_OnCellEditEnded(object? sender, DataGridCellEditEndedEventArgs e) {
		if (e.Row.DataContext is IgnoredRecipient newRecipient) {
			new Database().UpdateIgnoredRecipient(newRecipient);
		}
	}
}