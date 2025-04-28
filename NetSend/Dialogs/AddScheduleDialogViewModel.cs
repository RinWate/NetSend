using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Avalonia.Shared.Contracts;
using NetSend.Core.Enums;
using NetSend.ViewModels;
using System;

namespace NetSend.Dialogs {
	public partial class AddScheduleDialogViewModel : ViewModelBase, IDialogContext {
		public event EventHandler<object?>? RequestClose;

		[ObservableProperty] private DateTime _sendTime;
		[ObservableProperty] private SendingMode _sendingMode;
		[ObservableProperty] private string _message = string.Empty;

		[RelayCommand]
		private void Create() {

		}

		[RelayCommand]
		public void Cancel() {
			RequestClose?.Invoke(this, false);
		}

		public void Close() {
			RequestClose?.Invoke(this, false);
		}
	}
}
