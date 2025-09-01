using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSend.Core;

namespace NetSend.ViewModels;

public partial class ScanWindowViewModel : ViewModelBase {
    [ObservableProperty]
    private string _filter = string.Empty;

    [ObservableProperty]
    private List<string> _filters;

    [ObservableProperty]
    private bool _isScanning;

    [ObservableProperty]
    private string _log = string.Empty;

    public ScanWindowViewModel(Window? parent = null) {
        var db = new Database();
        Filters = db.GetAllFilters();

        Settings.GetValue("DefaultFilter", out var value);
        Filter = value;

        if (parent != null) window = parent;
    }

    [RelayCommand]
    public async Task Scan(string ipAddress) {
        if (string.IsNullOrWhiteSpace(ipAddress)) {
            Log += "Не введен фильтр!" + Environment.NewLine;
            return;
        }

        try {
            var address = IPAddress.Parse(ipAddress);
        } catch {
            Log += "Некорректный формат фильтра!" + Environment.NewLine;
            return;
        }

        IsScanning = true;
        var scanner = new IPScanner();

        Log += "Сканирование начато..." + Environment.NewLine;
        var startTime = DateTime.Now;
        await Task.Run(() => {
            scanner.Scan(ipAddress,
                message => { Dispatcher.UIThread.Post(() => { Log += message + Environment.NewLine; }); });
        });
        var endTime = DateTime.Now;
        var timeResult = (endTime - startTime).TotalSeconds;
        IsScanning = false;

        new Database().WriteFilter(ipAddress);
        window?.Close();
    }
}