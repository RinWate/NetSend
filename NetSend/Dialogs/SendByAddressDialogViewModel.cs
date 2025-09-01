using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Avalonia.Shared.Contracts;
using NetSend.Core;
using NetSend.Models;
using NetSend.ViewModels;

namespace NetSend.Dialogs;

public partial class SendByAddressDialogViewModel : ViewModelBase, IDialogContext {
    [ObservableProperty]
    private IPAddress _address = IPAddress.None;

    [ObservableProperty]
    private string _message = string.Empty;

    public SendByAddressDialogViewModel(string message) {
        Message = message;
        Address = GetIpAddress();
    }

    public event EventHandler<object?>? RequestClose;

    public void Close() {
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    public async Task Send() {
        var recipients = new List<Recipient> {
            new("", Address)
        };

        var mainWindow = Global.GetMainWindow();

        var sender = new Sender();
        await sender.Send(Message, mainWindow, recipients);

        Close();
    }

    public IPAddress GetIpAddress() {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
            if (ip.AddressFamily == AddressFamily.InterNetwork)
                return ip;
        return IPAddress.None;
    }
}