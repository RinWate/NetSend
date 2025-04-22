using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Avalonia.Shared.Contracts;
using NetSend.Core;
using NetSend.Models;
using NetSend.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetSend.Dialogs {
	public partial class SendByAddressDialogViewModel : ViewModelBase, IDialogContext {
		public event EventHandler<object?>? RequestClose;

		[ObservableProperty]
		private string _message = string.Empty;

		[ObservableProperty]
		private IPAddress _address = IPAddress.None;

		public SendByAddressDialogViewModel(string message) { 
			Message = message;
			Address = GetIpAddress();
		}

		[RelayCommand]
		public async Task Send() {

			var recipients = new List<Recipient>() {
				new Recipient("", Address)
			};

			var mainWindow = Global.GetMainWindow();

			var sender = new Sender();
			await sender.Send(Message, mainWindow, recipients);

			Close();
		}

		public IPAddress GetIpAddress() {
			var host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (var ip in host.AddressList) {
				if (ip.AddressFamily == AddressFamily.InterNetwork) return ip;
			}
			return IPAddress.None;
		}

		public void Close() {
			RequestClose?.Invoke(this, EventArgs.Empty);
		}
	}
}
