using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetSend.Models {
	public class Recipient {
		public string Hostname { get; set; } = string.Empty;
		public IPAddress Address { get; set; } = IPAddress.None;

		public Recipient(string hostname, IPAddress address) { 
			Hostname = hostname;
			Address = address;
		}

	}
}
