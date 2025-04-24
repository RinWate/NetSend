using Avalonia;
using Avalonia.Controls;
using NetSend.ViewModels;
using Ursa.Controls;

namespace NetSend.Views;

public partial class SettingsWindow : Window {

    private SettingsWindowViewModel viewModel;

    public SettingsWindow()
    {
        InitializeComponent();
        viewModel = new SettingsWindowViewModel(this);
        DataContext = viewModel;
    }
}