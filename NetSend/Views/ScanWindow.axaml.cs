using Avalonia.Controls;
using NetSend.ViewModels;

namespace NetSend.Views;

public partial class ScanWindow : Window {
    public ScanWindow() {
        InitializeComponent();
        DataContext = new ScanWindowViewModel(this);
    }
}