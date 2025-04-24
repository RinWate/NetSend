using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Avalonia.Shared.Contracts;
using NetSend.Core;
using System;
using System.Net;

namespace NetSend.Dialogs {
	public partial class PseudoNameSetterDialogViewModel : ObservableObject, IDialogContext {

		public event EventHandler<object?>? RequestClose;
		private IPAddress _address;

		public PseudoNameSetterDialogViewModel(IPAddress address) {
			_address = address;
		}

		[ObservableProperty]
		private string _pseudoname = string.Empty;

		public void Close() {
			RequestClose?.Invoke(this, null);
		}

		[RelayCommand]
		public void Submit() {

			new Database().WritePseudoName(_address, Pseudoname);
			RequestClose?.Invoke(this, Pseudoname);
		}
	}
}
