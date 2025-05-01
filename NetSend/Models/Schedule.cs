using System;
using NetSend.Core.Enums;

namespace NetSend.Models;

public class Schedule : Model {
    public DateTime SendDate { get; set; } = DateTime.MinValue;
    public SendingMode SendingMode { get; set; }
    public string Author { get; set; } = string.Empty;
    public Message Message { get; set; } = new();
}