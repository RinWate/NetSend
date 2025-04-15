using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSend.ViewModels {
	public partial class LongProcessWindowViewModel : ViewModelBase {

		[ObservableProperty]
		private string title = string.Empty;

		public LongProcessWindowViewModel(string title) {
			this.title = title;
		}

	}
}
