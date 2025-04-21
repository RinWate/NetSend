
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
		private ObservableCollection<Recipient> _filteredItems;

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

		public void FilterRecipients() {
			if (string.IsNullOrWhiteSpace(SearchText)) {
				FilteredItems = new ObservableCollection<Recipient>(Global.Recipients);
			} else {
				var lower = SearchText.ToLower();
				try {
					var filtered = Global.Recipients.Where(item => item.Hostname.ToLower().Contains(lower) || item.PseudoName.ToLower().Contains(lower));
					FilteredItems = new ObservableCollection<Recipient>(filtered);
				} catch { }
			}
		}

		public MainWindowViewModel() {
			Message = string.Empty;
			SelectedRecipients = new List<Recipient>();

			FilteredItems = new ObservableCollection<Recipient>(Global.Recipients);

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
				FilteredItems = new ObservableCollection<Recipient>(Global.Recipients);
			}
		}


		[RelayCommand]
		public async Task Scan() {
			var newScan = new ScanWindow();
			newScan.DataContext = new ScanWindowViewModel(newScan);

			var mainWindow = Global.GetMainWindow();
			await newScan.ShowDialog(mainWindow);

			FilteredItems = new ObservableCollection<Recipient>(Global.Recipients);
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
