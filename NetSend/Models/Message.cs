using System;

namespace NetSend.Models {
	public class Message {

		public int Id { get; set; }
		public DateTime SendDate { get; set; }
		public string Sender { get; set; } = string.Empty;
		public string Content { get; set; } = string.Empty;


		public Message(string message) {
			SendDate = DateTime.Now;
			Sender = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
			Content = message;
		}

		public Message() { }
	}
}
