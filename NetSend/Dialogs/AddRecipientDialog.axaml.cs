using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace NetSend.Dialogs;

public partial class AddRecipientDialog : UserControl {
    public AddRecipientDialog() {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        tb_pseudoname.Focus();
        base.OnLoaded(e);
    }
}