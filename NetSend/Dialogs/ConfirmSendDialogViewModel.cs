using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Avalonia.Shared.Contracts;
using NetSend.ViewModels;
using System;

namespace NetSend.Dialogs {
	public partial class ConfirmSendDialogViewModel : ViewModelBase, IDialogContext {
		public event EventHandler<object?>? RequestClose;

		[ObservableProperty]
		private string _message;

		private bool _isConfirmed;

		public ConfirmSendDialogViewModel(string message) {
			Message = message;
		}

		[RelayCommand]
		public void Confirm() {
			_isConfirmed = true;
			Close();
		}

		[RelayCommand]
		public void Cancel() {
			Close();
		}

		public void Close() {
			RequestClose?.Invoke(this, _isConfirmed);
		}
	}
}
