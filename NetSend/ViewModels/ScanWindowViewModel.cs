using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSend.ViewModels {
	public partial class ScanWindowViewModel : ViewModelBase {

		[ObservableProperty]
		private string _log = string.Empty;
		[ObservableProperty]
		private bool _isScanning = false;
		[ObservableProperty]
		private int _threads = 16;

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
			Log += $"Сканирование завершено за {timeResult:F2} сек." + Environment.NewLine;
			IsScanning = false;
		}

	}
}
