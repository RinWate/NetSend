using LiteDB;
using System.Net;

namespace NetSend.Models;

public class Recipient : Model {
	public Recipient(string hostname, IPAddress address) {
		Hostname = hostname;
		Address = address;
	}

	public Recipient(string pseudoname, string hostname, IPAddress address) {
		PseudoName = pseudoname;
		Hostname = hostname;
		Address = address;
	}

	public Recipient(bool isFavourite, string pseudoname, string hostname, IPAddress address) {
		IsFavourite = isFavourite;
		PseudoName = pseudoname;
		Hostname = hostname;
		Address = address;
	}

	public Recipient() {
	}

	[BsonIgnore] public bool IsIgnored { get; set; }

	[BsonIgnore] public bool IsFavourite { get; set; }

	[BsonIgnore] public string PseudoName { get; set; } = string.Empty;

	public string Hostname { get; set; } = string.Empty;
	public IPAddress Address { get; set; } = IPAddress.None;

	public override string? ToString() {
		if (string.IsNullOrWhiteSpace(PseudoName)) return $"{Hostname} : {Address}";
		return $"{PseudoName} : {Address}";
	}
}