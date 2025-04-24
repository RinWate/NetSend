using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace NetSend.ViewModels {
	public partial class ViewModelBase : ObservableObject {
		protected Window? window;

		public ViewModelBase(Window? parent = null) {
			window = parent;
		}
	}
}
