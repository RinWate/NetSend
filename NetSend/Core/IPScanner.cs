using Avalonia.Threading;
using NetSend.Models;
using System;
using System.Collections.Generic;
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
						Global.recipients.Add(new Recipient(hostname.HostName, addr));
						Dispatcher.UIThread.Post(() => {
							log($"Найден: {hostname.HostName} : {addr}");
						});
					}
				} catch { }
			});

			//for (byte i = 0; i < 255; i++) {
			//	byteIp[3] = i;

			//	var addr = new IPAddress(byteIp);
			//	var hostname = new IPHostEntry();
			//	try {
			//		var ping = new Ping();
			//		var pingResult = ping.Send(addr, 1);
			//		if (pingResult.Status == IPStatus.Success) {
			//			hostname = Dns.GetHostEntry(addr);
			//			Global.recipients.Add(new Recipient(hostname.HostName, addr));
			//			log($"Найден: {hostname.HostName} : {addr}");
			//		}
			//	} catch {}
			//}
		}
	}
}
