
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSend.Core;
using NetSend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ursa.Controls;

namespace NetSend.ViewModels {
	public partial class MainWindowViewModel : ViewModelBase {

		[ObservableProperty]
		private string _message;
		[ObservableProperty]
		private string _resultString;
		[ObservableProperty]
		private List<Recipient> _selectedRecipients;
		public MainWindowViewModel() {
			Message = string.Empty;
			SelectedRecipients = new List<Recipient>();
			ResultString = "Сканирование не выполнялось";
		}


		[RelayCommand]
		public void Scan() {
			var newScan = new ScanWindow();
			newScan.DataContext = new ScanWindowViewModel();

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

		private bool CheckFill() {
			bool MessageIsNull = string.IsNullOrWhiteSpace(Message);
			bool RecipientsIsNull = Global.Recipients.Count == 0;
			return !MessageIsNull && !RecipientsIsNull;
		}
    }
}
