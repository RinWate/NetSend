using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSend.Core;
using NetSend.Models;

namespace NetSend.ViewModels;

public partial class SettingsWindowViewModel : ViewModelBase {
    private readonly Window? _window;

    [ObservableProperty] private Setting _commonBase = new("CommonBase", string.Empty, false);

    [ObservableProperty] private Setting _defaultFilter = new("DefaultFilter", string.Empty, false);

    [ObservableProperty] private Setting _ignoredBase = new("IgnoredBase", string.Empty, false);

    [ObservableProperty] private Setting _pseudonamesBase = new("PseudonamesBase", string.Empty, false);

    [ObservableProperty] private Setting _templatesBase = new("TemplatesBase", string.Empty, false);

    public SettingsWindowViewModel(Window? parent = null) {
        LoadSettingsCommand.Execute(null);
        _window = parent;
    }

    [RelayCommand]
    private void SaveSettings() {
        var settings = new List<Setting> {
            CommonBase,
            TemplatesBase,
            PseudonamesBase,
            IgnoredBase,
            DefaultFilter
        };
        Settings.WriteSettings(settings);
        _window?.Close(true);
    }

    [RelayCommand]
    private void LoadSettings() {
        var settings = Settings.GetSettings();

        if (settings != null && settings.Count > 0) {
            CommonBase = Settings.FindSetting("CommonBase");
            TemplatesBase = Settings.FindSetting("SettingsBase");
            PseudonamesBase = Settings.FindSetting("PseudonamesBase");
            IgnoredBase = Settings.FindSetting("IgnoredBase");
            DefaultFilter = Settings.FindSetting("DefaultFilter");
        }
    }

    [RelayCommand]
    private void OpenProgramCatalog() {
        var directory = Directory.GetCurrentDirectory();
        if (Directory.Exists(directory)) Process.Start("explorer.exe", directory);
    }
}