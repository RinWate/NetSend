using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSend.Core;
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
		private List<string> _filters;
		[ObservableProperty]
		private string _filter = string.Empty;

		public CancellationTokenSource _cts;

		public ScanWindowViewModel(Window? parent = null) {
			var db = new Database();
			Filters = db.GetAllFilters();

			Settings.GetValue("DefaultFilter", out string value);
			Filter = value;

			if (parent != null) window = parent;

			ScanCommand.ExecuteAsync(Filter);
		}

		[RelayCommand]
		private async Task Scan(string ipAddress) {
			_cts = new CancellationTokenSource();
			var token = _cts.Token;
			if (string.IsNullOrWhiteSpace(ipAddress)) {
				Log += "Не введен фильтр!" + Environment.NewLine;
				return;
			}

			try {
				IPAddress.Parse(ipAddress);
			} catch {
				Log += "Некорректный формат фильтра!" + Environment.NewLine;
				return;
			}

			IsScanning = true;
			var scanner = new IPScanner();

			Log += "Сканирование начато..." + Environment.NewLine;
			await Task.Run(() => {
				scanner.Scan(ipAddress, (message) => {
					if (token.IsCancellationRequested) return;
					Avalonia.Threading.Dispatcher.UIThread.Post(() => {
						Log += message + Environment.NewLine;
					});
				});
			}, token);
			IsScanning = false;

			new Database().WriteFilter(ipAddress);
			window?.Close(true);
		}

		public void CancelScan() {
			_cts?.Cancel();
		}
	}
}
