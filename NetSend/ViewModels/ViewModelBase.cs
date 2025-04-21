using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using NetSend.Models;
using System.Collections.ObjectModel;

namespace NetSend.ViewModels
{
    public partial class ViewModelBase : ObservableObject {
        protected Window? window;

        public ViewModelBase(Window? parent = null) { 
            window = parent;
        }
    }
}
