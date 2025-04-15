using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using NetSend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSend {
	public static class Global {

		public static readonly string VERSION = "v 1.0";
		public static readonly List<Recipient> recipients = new List<Recipient>();
		public static Window? GetMainWindow() { 
			var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;
			return mainWindow;
		}

	}
}
