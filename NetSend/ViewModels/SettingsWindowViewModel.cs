using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSend.ViewModels {
	public partial class SettingsWindowViewModel : ViewModelBase {

		[ObservableProperty]
		private string _selectedPath;

	}
}
