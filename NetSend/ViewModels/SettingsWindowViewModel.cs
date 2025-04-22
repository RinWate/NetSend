using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSend.Core;
using NetSend.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSend.ViewModels {
	public partial class SettingsWindowViewModel : ViewModelBase {

		[ObservableProperty]
		private Setting _commonBase = new Setting("CommonBase", string.Empty, false);
		[ObservableProperty]
		private Setting _templatesBase = new Setting("TemplatesBase", string.Empty, false);
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
				TemplatesBase,
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
				TemplatesBase = Settings.FindSetting("SettingsBase") ??		new Setting();
				PseudoNamesBase = Settings.FindSetting("PseudoNamesBase") ??new Setting();
				DefaultFilter = Settings.FindSetting("DefaultFilter") ??	new Setting();
			}
		}

		[RelayCommand]
		public void OpenProgramCatalog() {
			var directory = Directory.GetCurrentDirectory();
			if (Directory.Exists(directory)) { 
				Process.Start("explorer.exe", directory);
			}
		}
	}
}
