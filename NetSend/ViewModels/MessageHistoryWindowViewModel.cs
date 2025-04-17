using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSend.Core;
using NetSend.Models;
using System.Windows;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.ComTypes;
using Avalonia.Input.Platform;
using System.Threading.Tasks;

namespace NetSend.ViewModels {
	public partial class MessageHistoryWindowViewModel : ViewModelBase {

		[ObservableProperty]
		private ObservableCollection<Message> _messages;
		[ObservableProperty]
		private Message _selectedMessage = new Message();

		public MessageHistoryWindowViewModel() { 
			var db = new Database();
			Messages = new ObservableCollection<Message>(db.AllMessages());
		}

		[RelayCommand]
		public void ClearHistory() {
			Messages.Clear();
			var db = new Database();
			db.ClearMessages();
		}

		[RelayCommand]
		public async Task CopyMessage() {
			var mainWindow = Global.GetMainWindow();
			var clipboard = mainWindow.Clipboard;

			await clipboard.SetTextAsync(SelectedMessage.Content);
		}

	}
}
