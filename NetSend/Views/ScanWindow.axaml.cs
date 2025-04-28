using Avalonia.Controls;
using NetSend.ViewModels;
using System;

namespace NetSend;

public partial class ScanWindow : Window {

	private readonly ScanWindowViewModel _viewModel;

	public ScanWindow() {
		InitializeComponent();
		_viewModel = new ScanWindowViewModel(this);
		DataContext = _viewModel;
	}

	protected override void OnClosed(EventArgs e) {
		_viewModel.CancelScan();
		base.OnClosed(e);
	}
}