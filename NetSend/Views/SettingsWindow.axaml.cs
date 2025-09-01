using Avalonia.Controls;
using NetSend.ViewModels;

namespace NetSend.Views;

public partial class SettingsWindow : Window {
    private readonly SettingsWindowViewModel viewModel;

    public SettingsWindow() {
        InitializeComponent();
        viewModel = new SettingsWindowViewModel(this);
        DataContext = viewModel;
    }
}