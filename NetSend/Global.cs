using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using NetSend.Models;
using System;
using System.Collections.ObjectModel;

namespace NetSend {
	public static class Global {

		public static readonly string[] TITLES = { @"Режим ""Глашатый""", "Наведём суету?", "Орём во весь голос?", "Устроим спам?", "У меня окошко вылезло..." };
		public static readonly string VERSION = "v 1.3.1 Native";
		public static ObservableCollection<Recipient> Recipients { get; set; } = new ObservableCollection<Recipient>();
		public static ObservableCollection<IgnoredRecipient> IgnoredRecipients { get; set; } = new ObservableCollection<IgnoredRecipient>();
		public static string StatusString { get; set; } = string.Empty;

		public static Window GetMainWindow() {
			var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;
			return mainWindow ?? throw new NullReferenceException();
		}

		public static string GetRandomTitle() {
			var randomIndex = new Random().Next(TITLES.Length - 1);
			return TITLES[randomIndex];
		}

	}
}
