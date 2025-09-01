using System.Diagnostics;
using System.IO;
using CommunityToolkit.Mvvm.Input;

namespace NetSend.ViewModels;

public partial class AccessErrorWindowViewModel : ViewModelBase {
    [RelayCommand]
    private void TryAgain() {
        var directory = Directory.GetCurrentDirectory();
        if (Directory.Exists(directory)) Process.Start(directory + "\\NetSend.exe");
        var window = Global.GetMainWindow();
        window?.Close();
    }

    [RelayCommand]
    private void CloseApp() {
        var window = Global.GetMainWindow();
        window?.Close();
    }
}