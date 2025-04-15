

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Documents;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSend.Core;
using NetSend.Models;
using System;
using System.Collections.Generic;
using Ursa.Controls;

namespace NetSend.ViewModels {
	public partial class MainWindowViewModel : ViewModelBase {

		[ObservableProperty]
		private string _version;
		[ObservableProperty]
		private string _message;
		[ObservableProperty]
		private List<Recipient> _recipients;

		public MainWindowViewModel() {
			Version = Global.VERSION;
			Message = string.Empty;
			Recipients = new List<Recipient>();
		}


		[RelayCommand]
		public void Scan() {
			var newScan = new ScanWindow();
			newScan.DataContext = new ScanWindowViewModel();
			newScan.Show();
		}

		[RelayCommand]
		public void SendAll() {

			bool isFilled = CheckFill();
			if (!isFilled) {
				MessageBox.ShowAsync("Не заполнено сообщение или не выбраны адресаты!", "Отказ", MessageBoxIcon.Error, MessageBoxButton.OK);
				return;
			}

			var wind = new LongProcessWindow();
			wind.DataContext = new LongProcessWindowViewModel("Отправка...");

			var mainWindow = Global.GetMainWindow();
			wind.ShowDialog(mainWindow!);
		}

		[RelayCommand]
		public void Send() {

		}

		private bool CheckFill() {
			bool MessageIsNull = string.IsNullOrWhiteSpace(Message);
			bool RecipientsIsNull = Recipients.Count == 0;
			return !MessageIsNull && !RecipientsIsNull;
		}
    }
}
