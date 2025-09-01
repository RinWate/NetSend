using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSend.Core;
using NetSend.Models;

namespace NetSend.ViewModels;

public partial class MessageHistoryWindowViewModel : ViewModelBase {
    private readonly MainWindowViewModel? _mainViewModel;

    [ObservableProperty]
    private ObservableCollection<Message> _messages;

    [ObservableProperty]
    private Message _selectedMessage = new();

    public MessageHistoryWindowViewModel() {
        var db = new Database();
        Messages = new ObservableCollection<Message>(db.AllMessages());
    }

    public MessageHistoryWindowViewModel(MainWindowViewModel mainViewModel) {
        var db = new Database();
        Messages = new ObservableCollection<Message>(db.AllMessages());
        _mainViewModel = mainViewModel;
    }

    [RelayCommand]
    private void ClearHistory() {
        Messages.Clear();
        var db = new Database();
        db.ClearMessages();
    }

    [RelayCommand]
    private async Task CopyMessage() {
        var mainWindow = Global.GetMainWindow();
        var clipboard = mainWindow!.Clipboard;

        await clipboard!.SetTextAsync(SelectedMessage.Content);
    }

    [RelayCommand]
    private void CopyMessageToMainWindow() {
        if (_mainViewModel != null)
            _mainViewModel.Message = SelectedMessage.Content;
    }

    [RelayCommand]
    private void DeleteMessage() {
        var db = new Database();
        db.DeleteMessage(SelectedMessage.Id);
        Messages.Remove(SelectedMessage);
    }
}