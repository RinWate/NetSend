using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetSend.Core;
using NetSend.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NetSend.ViewModels;

public partial class IgnoredWindowViewModel : ViewModelBase {

	[ObservableProperty]
	private ObservableCollection<IgnoredRecipient> _ignoredRecipients = new ObservableCollection<IgnoredRecipient>();

	[ObservableProperty]
	private List<IgnoredRecipient> _selectedRecipients = new List<IgnoredRecipient>();

	public IgnoredWindowViewModel() {
		IgnoredRecipients = Global.IgnoredRecipients;
	}

	// TODO Дописать изменения комментарий. По какой-то причине не происходит запись в базу данных
	partial void OnSelectedRecipientsChanged(List<IgnoredRecipient> value) {
		new Database().UpdateIgnoredRecipient(value[0]);
	}

	partial void OnIgnoredRecipientsChanged(ObservableCollection<IgnoredRecipient> value) {
		if (SelectedRecipients.Count != 0) {
			new Database().UpdateIgnoredRecipient(SelectedRecipients[0]);
		}
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