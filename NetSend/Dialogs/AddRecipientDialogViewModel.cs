using System;
using System.Collections.Generic;
using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Avalonia.Shared.Contracts;
using NetSend.Core;
using NetSend.Models;
using NetSend.ViewModels;

namespace NetSend.Dialogs;

public partial class AddRecipientDialogViewModel : ViewModelBase, IDialogContext {
    
    [ObservableProperty] private string _pseudoname = string.Empty;
    [ObservableProperty] private string _domainName = string.Empty;
    [ObservableProperty] private string _address = string.Empty;

    [RelayCommand]
    private void Add() {
        var newRecipient = new Recipient();

        if (string.IsNullOrWhiteSpace(DomainName) || string.IsNullOrWhiteSpace(Address)) {
            return;
        }

        IPAddress address;
        try {
            address = IPAddress.Parse(Address);
        } catch {
            return;
        }

        newRecipient.Hostname = DomainName;
        newRecipient.Address = address;

        var db = new Database();
        if (string.IsNullOrWhiteSpace(Pseudoname)) {
            db.WriteRecipient(newRecipient);
        } else {
            db.WriteRecipient(newRecipient);
            db.WritePseudoName(newRecipient.Address, Pseudoname);
        }
        RequestClose?.Invoke(this, true);
    }

    [RelayCommand]
    private void Cancel() {
        RequestClose?.Invoke(this, false);
    }
    
    public void Close() {
        RequestClose?.Invoke(this, false);
    }

    public event EventHandler<object?>? RequestClose;
}