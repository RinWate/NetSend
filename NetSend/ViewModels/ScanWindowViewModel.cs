using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSend.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NetSend.ViewModels {
	public partial class ScanWindowViewModel : ViewModelBase {

		[ObservableProperty]
		private string _log = string.Empty;
		[ObservableProperty]
		private bool _isScanning = false;
		[ObservableProperty]
		private int _threads = 32;
		[ObservableProperty]
		private List<string> _filters;

		public ScanWindowViewModel() { 
			var db = new Database();
			Filters = db.GetAll();
		}

		public ScanWindowViewModel(Window parent) {
			var db = new Database();
			Filters = db.GetAll();
			window = parent;
		}

		[RelayCommand]
		public async Task Scan(string ipAddress) {
			IsScanning = true;
			var scanner = new IPScanner();

			Log += "Сканирование начато..." + Environment.NewLine;
			DateTime startTime = DateTime.Now;
			await Task.Run(() => {
				scanner.Scan(ipAddress, Threads, (message) => {
					Avalonia.Threading.Dispatcher.UIThread.Post(() => {
						Log += message + Environment.NewLine;
					});
				});
			});
			DateTime endTime = DateTime.Now;
			var timeResult = (endTime - startTime).TotalSeconds;
			IsScanning = false;

			var db = new Database();
			db.WriteNew(ipAddress);
			window?.Close();
		}

	}
}
