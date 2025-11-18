using Avalonia.Controls;
using NetSend.ViewModels;

namespace NetSend.Views;

public partial class MessageHistoryWindow : Window {

    public MessageHistoryWindow() {
        InitializeComponent();
        DataContext = new MessageHistoryWindowViewModel();
    }
    
    public MessageHistoryWindow(MainWindowViewModel mainViewModel) {
        InitializeComponent();
        DataContext = new MessageHistoryWindowViewModel(mainViewModel);
    }
}