using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Avalonia.Shared.Contracts;
using NetSend.ViewModels;

namespace NetSend.Dialogs;

public partial class ConfirmSendDialogViewModel : ViewModelBase, IDialogContext {
    private bool _isConfirmed;

    [ObservableProperty] private string _message;

    public ConfirmSendDialogViewModel(string message) {
        Message = message;
    }

    public event EventHandler<object?>? RequestClose;

    public void Close() {
        RequestClose?.Invoke(this, _isConfirmed);
    }

    [RelayCommand]
    public void Confirm() {
        _isConfirmed = true;
        Close();
    }

    [RelayCommand]
    public void Cancel() {
        Close();
    }
}