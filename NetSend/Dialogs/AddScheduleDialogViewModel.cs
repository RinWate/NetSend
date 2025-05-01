using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Avalonia.Shared.Contracts;
using NetSend.Core.Enums;
using NetSend.ViewModels;

namespace NetSend.Dialogs;

public partial class AddScheduleDialogViewModel : ViewModelBase, IDialogContext {
    [ObservableProperty] private string _message = string.Empty;
    [ObservableProperty] private SendingMode _sendingMode;

    [ObservableProperty] private DateTime _sendTime;
    public event EventHandler<object?>? RequestClose;

    public void Close() {
        RequestClose?.Invoke(this, false);
    }

    [RelayCommand]
    private void Create() {
    }

    [RelayCommand]
    public void Cancel() {
        RequestClose?.Invoke(this, false);
    }
}