using Avalonia.Threading;
using NetSend.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace NetSend.Core {
	public class IPScanner {

		public void Scan(string ip_filter, int threads, Action<string> log) {
			Global.Recipients.Clear();
			var temp_list = new ConcurrentBag<Recipient>();
			var stringIp = IPAddress.Parse(ip_filter);
			var byteIp = stringIp.GetAddressBytes();
			var errors = new List<string>();

			var parallelOptions = new ParallelOptions();
			parallelOptions.MaxDegreeOfParallelism = threads;

			var start = 0;
			var end = 255;

			Parallel.For(start, end, parallelOptions, i => {
				byteIp[3] = (byte) i;

				var addr = new IPAddress(byteIp);
				var hostname = new IPHostEntry();

				try {
					var ping = new Ping();
					var pingResult = ping.Send(addr);
					if (pingResult.Status == IPStatus.Success) {
						hostname = Dns.GetHostEntry(addr);
						if (hostname != null) {
							temp_list.Add(new Recipient(hostname.HostName, addr));
							log($"Найден: {hostname.HostName} : {addr}");
						}
					}
				} catch (Exception e) { 
					errors.Add($"{addr.ToString()}: {e.Message}");
				}
			});
			var result = temp_list.OrderBy(a => a.Hostname).Distinct(new AddressComparer()).ToList();
			foreach (var temp in result) { 
				Global.Recipients.Add(temp);
			}

			new Database().WriteRecipients(result);
			Logger.LogList(errors);
		}
	}

	class AddressComparer : IEqualityComparer<Recipient> {
		public bool Equals(Recipient? x, Recipient? y) {
			if (x == null || y == null) return false;

			if (ReferenceEquals(x, y)) return true;

			return x.Address.GetHashCode() == y.Address.GetHashCode();
		}

		public int GetHashCode([DisallowNull] Recipient obj) {
			if (obj == null) return 0;

			return obj.Address.GetHashCode();
		}
	}
}
