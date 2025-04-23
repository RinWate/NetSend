using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using NetSend.ViewModels;

namespace NetSend;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
        DataContext = new AboutWindowViewModel(this);
    }
}