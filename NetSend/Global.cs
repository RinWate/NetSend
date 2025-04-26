using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using NetSend.Models;
using System;
using System.Collections.ObjectModel;

namespace NetSend {
	public static class Global {
		private static readonly string[] TITLES = { @"Режим ""Глашатай""", "Наведём суету?", "Орём во весь голос?", "Устроим спам?", "У меня окошко вылезло..." };
		public static readonly string VERSION = "v 1.3.4";
		public static ObservableCollection<Recipient> Recipients { get; set; } = new ObservableCollection<Recipient>();
		public static ObservableCollection<IgnoredRecipient> IgnoredRecipients { get; set; } = new ObservableCollection<IgnoredRecipient>();

		public static Window GetMainWindow() {
			var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;
			return mainWindow ?? throw new NullReferenceException();
		}

		public static string GetRandomTitle() {
			var randomIndex = new Random().Next(TITLES.Length);
			return TITLES[randomIndex];
		}

	}
}
