
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSend.Core;
using NetSend.Models;
using System.Collections.Generic;
using Ursa.Controls;

namespace NetSend.ViewModels {
	public partial class MainWindowViewModel : ViewModelBase {

		[ObservableProperty]
		private string _message;
		[ObservableProperty]
		private List<Recipient> _selectedRecipients;
		public MainWindowViewModel() {
			Message = string.Empty;
			SelectedRecipients = new List<Recipient>();

			Global.StatusString = "Сканирование не выполнялось";
		}


		[RelayCommand]
		public void Scan() {
			var newScan = new ScanWindow();
			newScan.DataContext = new ScanWindowViewModel(newScan);

			var mainWindow = Global.GetMainWindow();
			newScan.ShowDialog(mainWindow);
		}

		[RelayCommand]
		public void SendAll() {

			bool isFilled = CheckFill();
			if (!isFilled) {
				MessageBox.ShowAsync("Не заполнено сообщение или отсутствуют адресаты", "Отказ", MessageBoxIcon.Error, MessageBoxButton.OK);
				return;
			}

			var sender = new Sender();
			sender.Send(Message, Global.GetMainWindow());
			Global.StatusString = "Сообщение отправлено";
		}

		[RelayCommand]
		public void Send() {
			bool isFilled = CheckFill();
			if (!isFilled) {
				MessageBox.ShowAsync("Не заполнено сообщение или отсутствуют адресаты", "Отказ", MessageBoxIcon.Error, MessageBoxButton.OK);
				return;
			}

			var sender = new Sender();
			sender.Send(Message, Global.GetMainWindow(), SelectedRecipients);
		}

		[RelayCommand]
		public void SendToAddress() {
			bool isFilled = !string.IsNullOrEmpty(Message);
			if (!isFilled) {
				MessageBox.ShowAsync("Не заполнено сообщение!", "Отказ", MessageBoxIcon.Error, MessageBoxButton.OK);
				return;
			}
		}

		[RelayCommand]
		public void OpenHistory() {
			var newWindow = new MessageHistoryWindow();
			newWindow.DataContext = new MessageHistoryWindowViewModel(this);
			var mainWindow = Global.GetMainWindow();
			newWindow.Show(mainWindow);
		}

		[RelayCommand]
		public void OpenSettings() {
			var newWindow = new SettingsWindow();
			newWindow.DataContext = new SettingsWindowViewModel();
			var mainWindow = Global.GetMainWindow();
			newWindow.ShowDialog(mainWindow);
		}

		[RelayCommand]
		public void Exit() {
			var mainWindow = Global.GetMainWindow();
			mainWindow.Close();
		}

		private bool CheckFill() {
			bool MessageIsNull = string.IsNullOrWhiteSpace(Message);
			bool RecipientsIsNull = Global.Recipients.Count == 0;
			return !MessageIsNull && !RecipientsIsNull;
		}
    }
}
