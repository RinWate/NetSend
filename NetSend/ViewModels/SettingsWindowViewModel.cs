using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSend.Core;
using NetSend.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSend.ViewModels {
	public partial class SettingsWindowViewModel : ViewModelBase {

		[ObservableProperty]
		private Setting _commonBase = new Setting("CommonBase", string.Empty, false);
		[ObservableProperty]
		private Setting _settingsBase = new Setting("SettingsBase", string.Empty, false);
		[ObservableProperty]
		private Setting _pseudoNamesBase = new Setting("PseudoNamesBase", string.Empty, false);
		[ObservableProperty]
		private Setting _defaultFilter = new Setting("DefaultFilter", string.Empty, false);

		public SettingsWindowViewModel(Window? parent = null) {
			LoadSettingsCommand.Execute(null);
		}

		[RelayCommand]
		public void SaveSettings() {
			var settings = new List<Setting>() {
				CommonBase,
				SettingsBase,
				PseudoNamesBase,
				DefaultFilter
			};
			Settings.WriteSettings(settings);
		}

		[RelayCommand]
		public void LoadSettings() {
			var settings = Settings.GetSettings();

			if (settings != null && settings.Count > 0) {
				CommonBase = Settings.FindSetting("CommonBase") ??			new Setting();
				SettingsBase = Settings.FindSetting("SettingsBase") ??		new Setting();
				PseudoNamesBase = Settings.FindSetting("PseudoNamesBase") ??new Setting();
				DefaultFilter = Settings.FindSetting("DefaultFilter") ??	new Setting();
			}
		}

		protected override void OnPropertyChanged(PropertyChangedEventArgs e) {

			base.OnPropertyChanged(e);
		}
	}
}
