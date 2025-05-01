using System;
using System.Security.Principal;

namespace NetSend.Models;

public class Message : Model {
    public Message(string message) {
        SendDate = DateTime.Now;
        Sender = WindowsIdentity.GetCurrent().Name;
        Content = message;
    }

    public Message() {
    }

    public DateTime SendDate { get; set; } = DateTime.MinValue;
    public string Sender { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}