using System.Net;

namespace NetSend.Models;

public class IgnoredRecipient(string domainName, IPAddress address, string comment) {
    public int Id { get; set; }
    public string DomainName { get; set; } = domainName;
    public IPAddress Address { get; set; } = address;
    public string Comment { get; set; } = comment;
}