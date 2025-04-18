using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSend.ViewModels {
	public partial class SettingsWindowViewModel : ViewModelBase {

		[ObservableProperty]
		private string _selectedCommonBase = string.Empty;

		[ObservableProperty]
		private string _selectedSettingsBase = string.Empty;

		[ObservableProperty]
		private string _defaultFilter = string.Empty;

		[RelayCommand]
		public void SaveSettings() {

		}

		protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
			if (e.PropertyName == nameof(SelectedSettingsBase)) { 
				
			}
			base.OnPropertyChanged(e);
		}
	}
}
