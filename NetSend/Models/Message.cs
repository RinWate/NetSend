using System;

namespace NetSend.Models {
	public class Message : Model {

		public DateTime SendDate { get; set; } = DateTime.MinValue;
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
