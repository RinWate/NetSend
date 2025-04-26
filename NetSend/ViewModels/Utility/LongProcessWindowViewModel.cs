using CommunityToolkit.Mvvm.ComponentModel;

namespace NetSend.ViewModels {
	public partial class LongProcessWindowViewModel : ViewModelBase {

		[ObservableProperty]
		private string title = string.Empty;

		public LongProcessWindowViewModel(string title) {
			this.title = title;
		}

	}
}
