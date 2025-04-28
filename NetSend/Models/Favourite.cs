using System.Net;

namespace NetSend.Models {
	public sealed class Favourite(IPAddress address) : Model {

		public IPAddress Address { get; set; } = address;
	}
}
