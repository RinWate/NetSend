using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetSend.Models {
	public class Pseudoname {
		public int Id { get; set; }
		public IPAddress Address { get; set; } = IPAddress.None;
		public string Name { get; set; } = string.Empty;
		public Pseudoname(IPAddress address, string name) { 
			Address = address;
			Name = name;
		}

	}
}
