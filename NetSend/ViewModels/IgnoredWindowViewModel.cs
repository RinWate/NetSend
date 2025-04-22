using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSend.Models;

namespace NetSend.ViewModels;

public partial class IgnoredWindowViewModel : ViewModelBase {

    [ObservableProperty]
    private ObservableCollection<IgnoredRecipient> _ignoredRecipients = new ObservableCollection<IgnoredRecipient>();
    [ObservableProperty]
    private ObservableCollection<IgnoredRecipient> _selectedRecipients = new ObservableCollection<IgnoredRecipient>();
    
    public IgnoredWindowViewModel() {
        
    }

    [RelayCommand]
    private void ClearIgnoredRecipients() {
        
    }
    
}