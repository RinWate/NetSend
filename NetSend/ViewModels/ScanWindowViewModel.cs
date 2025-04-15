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

		[RelayCommand]
		public async Task Scan(string ipAddress) {
			IsScanning = true;
			var scanner = new IPScanner();

			Log += "Сканирование начато..." + Environment.NewLine;
			await Task.Run(() => {
				scanner.Scan(ipAddress, (message) => {
					Avalonia.Threading.Dispatcher.UIThread.Post(() => {
						Log += message + Environment.NewLine;
					});
				});
			});
			Log += "Сканирование завершено." + Environment.NewLine;
			IsScanning = false;
		}

	}
}
