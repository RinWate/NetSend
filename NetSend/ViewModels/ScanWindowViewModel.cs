using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSend.Core;
using NetSend.Core.Enums;

namespace NetSend.ViewModels;

public partial class ScanWindowViewModel : ViewModelBase {
    private CancellationTokenSource? _cts;

    [ObservableProperty] private string _filter = string.Empty;

    [ObservableProperty] private List<string> _filters;

    [ObservableProperty] private bool _isScanning;

    [ObservableProperty] private string _log = string.Empty;

    public ScanWindowViewModel(Window? parent = null) {
        var db = new Database();
        Filters = db.GetAllFilters();

        Settings.GetValue("DefaultFilter", out var value);
        Filter = value;

        if (parent != null) window = parent;

        ScanCommand.ExecuteAsync(Filter);
    }

    [RelayCommand]
    private async Task Scan(string ipAddress) {
        _cts = new CancellationTokenSource();
        var token = _cts.Token;
        if (string.IsNullOrWhiteSpace(ipAddress)) {
            Log += "Не введен фильтр!" + Environment.NewLine;
            return;
        }

        try {
            IPAddress.Parse(ipAddress);
        } catch {
            Log += "Некорректный формат фильтра!" + Environment.NewLine;
            return;
        }

        IsScanning = true;
        var scanner = new IPScanner();

        Log += "Сканирование начато..." + Environment.NewLine;
        await Task.Run(() => {
            scanner.Scan(ipAddress, message => {
                if (token.IsCancellationRequested) return;
                Dispatcher.UIThread.Post(() => { Log += message + Environment.NewLine; });
            }, token);
        }, token);
        IsScanning = false;

        new Database().WriteFilter(ipAddress);
        window?.Close(ScanResult.Success);
    }

    public void CancelScan() {
        _cts?.Cancel();
    }
}