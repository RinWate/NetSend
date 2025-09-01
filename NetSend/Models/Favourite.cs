using System.Net;

namespace NetSend.Models;

public sealed class Favourite(IPAddress address) {
    public int Id { get; set; }
    public IPAddress Address { get; set; } = address;
}