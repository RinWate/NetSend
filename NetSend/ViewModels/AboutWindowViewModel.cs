using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSend.ViewModels {
	public partial class AboutWindowViewModel : ViewModelBase {

		public AboutWindowViewModel(Window? parent) { 
			if (parent != null) window = parent;
		}

		[RelayCommand]
		public void CloseWindow() {
			window?.Close();
		}

	}
}
