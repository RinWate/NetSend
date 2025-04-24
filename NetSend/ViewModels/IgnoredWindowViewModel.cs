using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSend.Core;
using NetSend.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace NetSend.ViewModels;

public partial class IgnoredWindowViewModel : ViewModelBase {

	[ObservableProperty]
	private ObservableCollection<IgnoredRecipient> _ignoredRecipients = new ObservableCollection<IgnoredRecipient>();
	
	[ObservableProperty]
	private List<IgnoredRecipient> _selectedRecipients = new List<IgnoredRecipient>();

	public IgnoredWindowViewModel() {
		IgnoredRecipients = Global.IgnoredRecipients;
	}

	[RelayCommand]
	private void ClearIgnoredRecipients() {
		new Database().RemoveAllIgnoredRecipients();
	}

	[RelayCommand]
	private void RemoveFromIgnore() {
		new Database().RemoveRecipientsFromIgnore(SelectedRecipients);
		Settings.LoadIgnoredRecipients();
		IgnoredRecipients = Global.IgnoredRecipients;
	}

}