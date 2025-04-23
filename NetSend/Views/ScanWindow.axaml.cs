using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using NetSend.ViewModels;

namespace NetSend;

public partial class ScanWindow : Window
{
    public ScanWindow()
    {
        InitializeComponent();
        DataContext = new ScanWindowViewModel(this);
    }
}