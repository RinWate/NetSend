using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSend.Core;
using NetSend.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
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
		[ObservableProperty]
		private string _filter = string.Empty;

		public ScanWindowViewModel(Window? parent = null) {
			var db = new Database();
			Filters = db.GetAllFilters();

			Settings.GetValue("DefaultFilter", out string value);
			Filter = value;

			if (parent != null) window = parent;
		}

		[RelayCommand]
		public async Task Scan(string ipAddress) {

			if (string.IsNullOrWhiteSpace(ipAddress)) {
				Log += "Не введен фильтр!" + Environment.NewLine;
				return;
			}

			try {
				var address = IPAddress.Parse(ipAddress);
			} catch (Exception ex) {
				Log += "Некорректный формат фильтра!" + Environment.NewLine;
				return;
			}

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

			new Database().WriteFilter(ipAddress);
			window?.Close();
		}

	}
}
