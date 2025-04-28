using System.Net;

namespace NetSend.Models {
	public class Pseudoname : Model {
		public IPAddress Address { get; set; } = IPAddress.None;
		public string Name { get; set; } = string.Empty;
		public Pseudoname(IPAddress address, string name) {
			Address = address;
			Name = name;
		}

	}
}
