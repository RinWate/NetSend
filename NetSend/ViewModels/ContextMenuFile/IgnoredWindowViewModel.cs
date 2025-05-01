using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSend.Core;
using NetSend.Models;

namespace NetSend.ViewModels;

public partial class IgnoredWindowViewModel : ViewModelBase {
    [ObservableProperty] private ObservableCollection<IgnoredRecipient> _ignoredRecipients = new();

    [ObservableProperty] private List<IgnoredRecipient> _selectedRecipients = new();

    public IgnoredWindowViewModel() {
        IgnoredRecipients = Global.IgnoredRecipients;
    }

    partial void OnIgnoredRecipientsChanged(ObservableCollection<IgnoredRecipient> value) {
        IgnoredRecipients = Global.IgnoredRecipients;
    }

    [RelayCommand]
    private void ClearIgnoredRecipients() {
        new Database().RemoveAllIgnoredRecipients();
    }

    [RelayCommand]
    private void RemoveFromIgnore() {
        new Database().RemoveRecipientsFromIgnore(SelectedRecipients);
        Settings.LoadIgnoredRecipients();
    }
}