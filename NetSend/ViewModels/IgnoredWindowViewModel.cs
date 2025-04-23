using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSend.Core;
using NetSend.Models;

namespace NetSend.ViewModels;

public partial class IgnoredWindowViewModel : ViewModelBase {

    [ObservableProperty]
    private ObservableCollection<IgnoredRecipient> _ignoredRecipients = new ObservableCollection<IgnoredRecipient>();
    [ObservableProperty]
    private IgnoredRecipient _selectedRecipient = new IgnoredRecipient();
    
    public IgnoredWindowViewModel() {
        IgnoredRecipients = Global.IgnoredRecipients;
    }

    partial void OnSelectedRecipientChanged(IgnoredRecipient value) {
        new Database().UpdateIgnoredRecipient(value);
    }

    [RelayCommand]
    private void ClearIgnoredRecipients() {
        new Database().RemoveAllIgnoredRecipients();
    }

    [RelayCommand]
    private void RemoveFromIgnore() {
        new Database().RemoveRecipientFromIgnore(SelectedRecipient.Id);
        IgnoredRecipients.Remove(SelectedRecipient);
    }
    
}