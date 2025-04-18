using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using NetSend.Models;
using System;
using System.Collections.ObjectModel;

namespace NetSend {
	public static class Global {

		public static readonly string VERSION = "v 1.1";
		public static ObservableCollection<Recipient> Recipients { get; set; } = new ObservableCollection<Recipient>();
		public static string StatusString { get; set; } = string.Empty;

		public static Window GetMainWindow() {
			var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;
			return mainWindow ?? throw new NullReferenceException();
		}

	}
}
