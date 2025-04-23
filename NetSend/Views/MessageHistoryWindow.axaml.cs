using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using NetSend.ViewModels;

namespace NetSend;

public partial class MessageHistoryWindow : Window
{
    public MessageHistoryWindow(MainWindowViewModel mainViewModel)
    {
        InitializeComponent();
        DataContext = new MessageHistoryWindowViewModel(mainViewModel);
    }
}