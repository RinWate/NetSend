using Avalonia.Controls;
using Avalonia.Interactivity;

namespace NetSend;

public partial class PseudoNameSetterDialog : UserControl {
    public PseudoNameSetterDialog() {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        tb_pseudoname.Focus();
        base.OnLoaded(e);
    }
}