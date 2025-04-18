using CommunityToolkit.Mvvm.ComponentModel;
using System.Net;

namespace NetSend.Models {
	public class Recipient : ObservableObject {

		public string PseudoName { get; set; } = string.Empty;
		public string Hostname { get; set; } = string.Empty;
		public IPAddress Address { get; set; } = IPAddress.None;

		public Recipient(string hostname, IPAddress address) { 
			Hostname = hostname;
			Address = address;
		}

		public Recipient(string pseudoname, string hostname, IPAddress address) { 
			PseudoName = pseudoname;
			Hostname = hostname;
			Address = address;
		}

		public override string? ToString() {
			if (string.IsNullOrEmpty(PseudoName)) return $"{Hostname} : {Address}";
			return $"{PseudoName} : {Address}";
		}
	}
}
