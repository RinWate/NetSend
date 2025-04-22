using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSend.Core;
using NetSend.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Avalonia.Controls.Notifications;
using Ursa.Controls;

namespace NetSend.ViewModels {
	public partial class SettingsWindowViewModel : ViewModelBase {

		public WindowToastManager? ToastManager { get; set; } 
		
		[ObservableProperty]
		private Setting _commonBase = new Setting("CommonBase", string.Empty, false);
		[ObservableProperty]
		private Setting _templatesBase = new Setting("TemplatesBase", string.Empty, false);
		[ObservableProperty]
		private Setting _pseudoNamesBase = new Setting("PseudoNamesBase", string.Empty, false);
		[ObservableProperty]
		private Setting _ignoredBase = new Setting("IgnoredBase", string.Empty, false);
		[ObservableProperty]
		private Setting _defaultFilter = new Setting("DefaultFilter", string.Empty, false);

		public SettingsWindowViewModel(Window? parent = null) {
			LoadSettingsCommand.Execute(null);
		}

		[RelayCommand]
		private void SaveSettings() {
			var settings = new List<Setting>() {
				CommonBase,
				TemplatesBase,
				PseudoNamesBase,
				IgnoredBase,
				DefaultFilter
			};
			Settings.WriteSettings(settings);
			
			ToastManager?.Show(new Toast("Настройки сохранены", NotificationType.Success));
		}

		[RelayCommand]
		private void LoadSettings() {
			var settings = Settings.GetSettings();

			if (settings != null && settings.Count > 0) {
				CommonBase = Settings.FindSetting("CommonBase");
				TemplatesBase = Settings.FindSetting("SettingsBase");
				PseudoNamesBase = Settings.FindSetting("PseudoNamesBase");
				IgnoredBase = Settings.FindSetting("IgnoredBase");
				DefaultFilter = Settings.FindSetting("DefaultFilter");
			}
		}

		[RelayCommand]
		private void OpenProgramCatalog() {
			var directory = Directory.GetCurrentDirectory();
			if (Directory.Exists(directory)) { 
				Process.Start("explorer.exe", directory);
			}
		}
	}
}
