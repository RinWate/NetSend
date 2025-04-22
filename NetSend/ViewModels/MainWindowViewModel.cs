
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSend.Core;
using NetSend.Dialogs;
using NetSend.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Ursa.Controls;

namespace NetSend.ViewModels {
	public partial class MainWindowViewModel : ViewModelBase {

		[ObservableProperty]
		private string _message;
		[ObservableProperty]
		private List<Recipient> _selectedRecipients;

		private string _searchText = string.Empty;
		[ObservableProperty]
		private ObservableCollection<Recipient> _filteredItems = new ObservableCollection<Recipient>();

		public string SearchText {
			get => _searchText;
			set {
				if (_searchText != value) {
					_searchText = value;
					OnPropertyChanged();
					FilterRecipients();
				}
			}
		}

		private void FilterRecipients() {
			if (string.IsNullOrWhiteSpace(SearchText)) {
				FilteredItems = new ObservableCollection<Recipient>(Global.Recipients);
			} else {
				var lower = SearchText.ToLower();
				try {
					var filtered = Global.Recipients.Where(item => item.PseudoName.ToLower().Contains(lower) || item.Hostname.ToLower().Contains(lower));
					FilteredItems = new ObservableCollection<Recipient>(filtered);
				} catch {
					Console.WriteLine();
				}
			}
		}

		public MainWindowViewModel() {
			Message = string.Empty;
			SelectedRecipients = new List<Recipient>();

			FilterRecipients();

			Global.StatusString = "Сканирование не выполнялось";
		}

		[RelayCommand]
		public async Task SetPseudoName() {

			var selectedRecipient = SelectedRecipients[0];

			var options = new DialogOptions() {
				Title = "Псевдоним для " + selectedRecipient.Hostname,
				Mode = DialogMode.Question
			};
			var result = await Dialog.ShowCustomModal<PseudoNameSetterDialog, PseudoNameSetterDialogViewModel, object>(new PseudoNameSetterDialogViewModel(selectedRecipient.Address), options: options);
			if (result != null && result is string) { 
				selectedRecipient.PseudoName = result as string;
				Settings.ReloadRecipients();
				FilterRecipients();
			}
		}

		[RelayCommand]
		public void ClearPseudoName() {
			var selectedRecipient = SelectedRecipients[0];
			new Database().WritePseudoName(selectedRecipient.Address, string.Empty);
			Settings.ReloadRecipients();
			FilterRecipients();
		}

		[RelayCommand]
		public void AddInFavourite() {
			var selectedRecipient = SelectedRecipients[0];
			selectedRecipient.IsFavourite = true;
			new Database().SetFavourite(selectedRecipient.Address);
			Settings.ReloadRecipients();
			FilterRecipients();
		}

		[RelayCommand]
		public void RemoveInFavourite() {
			var selectedRecipient = SelectedRecipients[0];
			selectedRecipient.IsFavourite = false;
			new Database().ClearFavourite(selectedRecipient.Address);
			Settings.ReloadRecipients();
			FilterRecipients();
		}

		[RelayCommand]
		public async Task Scan() {
			var newScan = new ScanWindow();
			newScan.DataContext = new ScanWindowViewModel(newScan);

			var mainWindow = Global.GetMainWindow();
			await newScan.ShowDialog(mainWindow);

			Settings.ReloadRecipients();
			FilterRecipients();
		}

		[RelayCommand]
		public async Task SendAll() {

			bool isFilled = CheckFill();
			if (!isFilled) {
				await MessageBox.ShowAsync("Не заполнено сообщение или отсутствуют адресаты", "Отказ", MessageBoxIcon.Error, MessageBoxButton.OK);
				return;
			}
			var options = new DialogOptions() {
				Title = "Массовая резня"
			};
			
			var result = await Dialog.ShowCustomModal<ConfirmSendDialog, ConfirmSendDialogViewModel, object>(new ConfirmSendDialogViewModel("Сообщение будет отправлено ВСЕМ получателям. Вы уверены?"), options: options);
			if (result is not bool) return;

			var isConfirmed = (bool) result;
			if (isConfirmed) {
				var sender = new Sender();
				await sender.Send(Message, Global.GetMainWindow());
				Global.StatusString = "Сообщение отправлено";
			}
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
		public async Task SendToAddress() {
			bool isFilled = !string.IsNullOrEmpty(Message);
			if (!isFilled) {
				await MessageBox.ShowAsync("Не заполнено сообщение!", "Отказ", MessageBoxIcon.Error, MessageBoxButton.OK);
				return;
			}

			var options = new DialogOptions() {
				Title = "Отправить по адресу"
			};

			await Dialog.ShowCustomModal<SendByAddressDialog, SendByAddressDialogViewModel, object>(new SendByAddressDialogViewModel(Message), options: options);
		}

		[RelayCommand]
		public void OpenHistory() {
			var newWindow = new MessageHistoryWindow();
			newWindow.DataContext = new MessageHistoryWindowViewModel(this);
			var mainWindow = Global.GetMainWindow();
			newWindow.Show(mainWindow);
		}

		[RelayCommand]
		public async Task OpenAbout() {
			var newWindow = new AboutWindow();
			newWindow.DataContext = new AboutWindowViewModel(newWindow);

			var mainWindow = Global.GetMainWindow();
			await newWindow.ShowDialog(mainWindow);
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
