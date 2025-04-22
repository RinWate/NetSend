using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetSend.Models {
	public sealed class Favourite(IPAddress address) {

		public int Id { get; set; }
		public IPAddress Address { get; set; } = address;
	}
}
