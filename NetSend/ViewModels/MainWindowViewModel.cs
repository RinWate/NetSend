
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSend.Core;
using NetSend.Dialogs;
using NetSend.Models;
using NetSend.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Ursa.Controls;

namespace NetSend.ViewModels {
	public partial class MainWindowViewModel : ViewModelBase {

		public WindowToastManager? _toastManager;

		[ObservableProperty]
		private string _message;
		[ObservableProperty]
		private ObservableCollection<Recipient> _selectedRecipients;
		[ObservableProperty]
		private ObservableCollection<Recipient> _filteredItems = new ObservableCollection<Recipient>();
		private string _searchText = string.Empty;

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
				var tempList = Global.Recipients.OrderByDescending(x => x.IsFavourite)
					.ThenBy(x => x.Hostname);
				FilteredItems = new ObservableCollection<Recipient>(tempList);
			} else {
				var lower = SearchText.ToLower();
				try {
					var filtered = Global.Recipients.Where(item => item.PseudoName.ToLower().Contains(lower) || item.Hostname.ToLower().Contains(lower))
						.OrderByDescending(x => x.IsFavourite).ThenBy(x => x.Hostname);
					FilteredItems = new ObservableCollection<Recipient>(filtered);
				} catch {
					Console.WriteLine();
				}
			}
		}

		public MainWindowViewModel() {
			Message = string.Empty;
			SelectedRecipients = new ObservableCollection<Recipient>();

			FilterRecipients();
			Global.StatusString = "Сканирование не выполнялось";
		}

		[RelayCommand]
		public void AddRecipientToIgnoreList() {
			var selected = SelectedRecipients;
			var ignored = new List<IgnoredRecipient>();
			foreach (var recipient in selected) {
				ignored.Add(new IgnoredRecipient(recipient.Hostname, recipient.Address));
			}
			new Database().AddRecipientToIgnore(ignored);
			
			Settings.LoadIgnoredRecipients();
			Settings.ReloadRecipients();
			FilterRecipients();
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
				selectedRecipient.PseudoName = result as string ?? "";
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
			var selectedRecipient = SelectedRecipients;
			foreach (var recipient in selectedRecipient) {
				recipient.IsFavourite = true;
			}
			new Database().SetFavourite(selectedRecipient.ToList());
			Settings.ReloadRecipients();
			FilterRecipients();
		}

		[RelayCommand]
		public void RemoveInFavourite() {
			var selectedRecipient = SelectedRecipients;
			foreach (var recipient in selectedRecipient) { 
				recipient.IsFavourite = false;
			}
			new Database().ClearFavourite(selectedRecipient.ToList());
			Settings.ReloadRecipients();
			FilterRecipients();
		}

		[RelayCommand]
		public async Task Scan() {
			var newScan = new ScanWindow();

			var mainWindow = Global.GetMainWindow();
			await newScan.ShowDialog(mainWindow);

			Settings.ReloadRecipients();
			FilterRecipients();

			_toastManager?.Show(new Toast("Найдено: " + FilteredItems.Count), NotificationType.Information, showIcon: true, showClose: true);
		}

		[RelayCommand]
		public async Task SendAll() {

			bool isFilled = CheckFill();
			if (!isFilled) {
				await MessageBox.ShowAsync("Не заполнено сообщение или отсутствуют адресаты", "Отказ", MessageBoxIcon.Error, MessageBoxButton.OK);
				return;
			}
			var options = new DialogOptions() {
				Title = Global.GetRandomTitle()
			};
			
			var result = await Dialog.ShowCustomModal<ConfirmSendDialog, ConfirmSendDialogViewModel, object>(new ConfirmSendDialogViewModel("Сообщение будет отправлено ВСЕМ получателям. Вы уверены?"), options: options);
			if (result is not bool) return;

			var isConfirmed = (bool) result;
			if (isConfirmed) {
				var sender = new Sender();
				await sender.Send(Message, Global.GetMainWindow());
				_toastManager?.Show(new Toast($"Отправлено {FilteredItems.Count} получателям"), NotificationType.Information, showIcon: true, showClose: true);
			}
		}

		[RelayCommand]
		public async Task Send() {
			bool isFilled = CheckFill();
			if (!isFilled) {
				await MessageBox.ShowAsync("Не заполнено сообщение или отсутствуют адресаты", "Отказ", MessageBoxIcon.Error);
				return;
			}

			var sender = new Sender();
			await sender.Send(Message, Global.GetMainWindow(), SelectedRecipients.ToList());
			_toastManager?.Show(new Toast("Сообщение отправлено"), NotificationType.Information, showIcon: true, showClose: true);
		}

		[RelayCommand]
		public async Task SendToAddress() {
			bool isFilled = !string.IsNullOrEmpty(Message);
			if (!isFilled) {
				await MessageBox.ShowAsync("Не заполнено сообщение!", "Отказ", MessageBoxIcon.Error);
				return;
			}

			var options = new DialogOptions() {
				Title = "Отправить по адресу"
			};

			await Dialog.ShowCustomModal<SendByAddressDialog, SendByAddressDialogViewModel, object>(new SendByAddressDialogViewModel(Message), options: options);
		}

		[RelayCommand]
		public void OpenHistory() {
			var newWindow = new MessageHistoryWindow(this);
			var mainWindow = Global.GetMainWindow();
			newWindow.Show(mainWindow);
		}

		[RelayCommand]
		public async Task OpenAbout() {
			var newWindow = new AboutWindow();

			var mainWindow = Global.GetMainWindow();
			await newWindow.ShowDialog(mainWindow);
		}

		[RelayCommand]
		public async Task OpenSettings() {
			var newWindow = new SettingsWindow();
			
			var mainWindow = Global.GetMainWindow();
			await newWindow.ShowDialog(mainWindow);
		}

		[RelayCommand]
		public async Task OpenIgnoredList() {
			var newWindow = new IgnoredWindow();

			var mainWindow = Global.GetMainWindow();
			await newWindow.ShowDialog(mainWindow);

			Settings.ReloadRecipients();
			FilterRecipients();
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
