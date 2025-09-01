using System;
using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Avalonia.Shared.Contracts;
using NetSend.Core;

namespace NetSend.Dialogs;

public partial class PseudoNameSetterDialogViewModel : ObservableObject, IDialogContext {
    private readonly IPAddress _address;

    [ObservableProperty]
    private string _pseudoname = string.Empty;

    public PseudoNameSetterDialogViewModel(IPAddress address) {
        _address = address;
    }

    public event EventHandler<object?>? RequestClose;

    public void Close() {
        RequestClose?.Invoke(this, null);
    }

    [RelayCommand]
    public void Submit() {
        new Database().WritePseudoName(_address, Pseudoname);
        RequestClose?.Invoke(this, Pseudoname);
    }
}