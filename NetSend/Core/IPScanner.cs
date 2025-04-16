using Avalonia.Threading;
using NetSend.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace NetSend.Core {
	public class IPScanner {

		public void Scan(string ip_filter, int threads, Action<string> log) {
			if (ip_filter == null) {
				log("Не указан фильтр!");
				return;
			}
			Global.Recipients.Clear();
			var temp_list = new ConcurrentBag<Recipient>();
			var stringIp = IPAddress.Parse(ip_filter);
			var byteIp = stringIp.GetAddressBytes();

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
				} catch { }
			});
			var result = temp_list.OrderBy(a => a.Hostname).Distinct().ToList();
			foreach (var temp in result) { 
				Global.Recipients.Add(temp);
			}
		}
	}
}
