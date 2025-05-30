﻿using NetSend.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NetSend.Core {
	public class IPScanner {

		public void Scan(string ip_filter, Action<string> log) {
			Global.Recipients.Clear();
			var temp_list = new ConcurrentBag<Recipient>();
			var errors = new List<string>();

			var poolSize = 255;
			var stringIp = IPAddress.Parse(ip_filter);
			var db = new Database();

			var rangePartitioner = Partitioner.Create(0, poolSize, poolSize / 8);
			Parallel.ForEach(rangePartitioner, (range, state) => {
				var byteIp = stringIp.GetAddressBytes();
				for (int i = range.Item1; i < range.Item2; i++) {
					byteIp[3] = (byte)i;
					var addr = new IPAddress(byteIp);
					var isIgnored = Global.IgnoredRecipients.FirstOrDefault(e => e.Address.ToString() == addr.ToString()) != null;
					if (isIgnored) continue;

					try {
						var ping = new Ping();
						var reply = ping.Send(addr, 2);
						if (reply.Status == IPStatus.Success) {
							var hostentry = Dns.GetHostEntry(addr);
							temp_list.Add(new Recipient(hostentry.HostName, addr));
							log($"Найден: {hostentry.HostName} : {addr}");
						}
					} catch (SocketException e) {
						errors.Add($"{addr} : {e.Message}");
						temp_list.Add(new Recipient("UNKNOWN", addr));
						log($"Найден: UNKNOWN : {addr}");
					} catch (Exception e) {
						errors.Add($"{addr} : {e.Message}");
					}
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
