using NetSend.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NetSend.Core;

public class IPScanner {
	/// <summary>
	/// Выполнить сканирование локальной сети по заданной маске
	/// </summary>
	/// <param name="ip_filter">Маска в формате 255.255.255.0</param>
	/// <param name="log">Делегат для логгирования</param>
	/// <param name="token">Токен отмены</param>
	public void Scan(string ip_filter, Action<string> log, CancellationToken token) {
		Global.Recipients.Clear();
		var temp_list = new ConcurrentBag<Recipient>();
		var errors = new List<string>();

		var poolSize = 255;
		var stringIp = IPAddress.Parse(ip_filter);
		var db = new Database();

		/*
         * 1. Инициализируем нужное количество потоков, согласно количеству кусочков массива
         * 2. Проверяем, не добавлен ли наш адрес в список игнорируемых получателей. Если да, то пропускаем итерацию
         * 3. Пытаемся выполнить команду ping, если успешно, то пытаемся получить доменное имя
         * 4. Если доменное имя получено, записываем имя и адрес в результирующий список.
         * 5. Если доменное имя не было получено, но команда ping отработала успешно, то присваиваем имя "UNKNOWN"
         *      и всё равно пишем в результирующий список.
         * 6. Если по адресу не было получено вообще никакого ответа, то пишем в лог ошибку.
         * 7. На всякий случай, с помощью компаратора и LINQ-выражения убираем дубли из списка и сортируем по доменному имени
         *      в алфавитном порядке
         */
		var rangePartitioner = Partitioner.Create(0, poolSize, poolSize / 8);
		Parallel.ForEach(rangePartitioner, (range, state) => {
			var byteIp = stringIp.GetAddressBytes();
			for (var i = range.Item1; i < range.Item2; i++) {
				if (token.IsCancellationRequested) state.Stop();
				byteIp[3] = (byte)i;
				var addr = new IPAddress(byteIp);

				var isIgnored = Global.IgnoredRecipients.FirstOrDefault(e => e.Address.ToString() == addr.ToString()) !=
								null;
				if (isIgnored) continue;
				try {
					var ping = new Ping();
					var reply = ping.Send(addr, 2);
					if (reply.Status == IPStatus.Success) {
						var hostEntry = Dns.GetHostEntry(addr);
						temp_list.Add(new Recipient(hostEntry.HostName, addr));
						log($"Найден: {hostEntry.HostName} : {addr}");
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
		if (token.IsCancellationRequested) return;
		var result = temp_list.OrderBy(a => a.Hostname).Distinct(new AddressComparer()).ToList();
		foreach (var temp in result) Global.Recipients.Add(temp);

		new Database().WriteRecipients(result);
		Logger.LogList(errors);
	}
}

/// <summary>
/// Компаратор IP-адресов. Необходим для корректного сравнивания адресов в LINQ выражениях
/// </summary>
internal class AddressComparer : IEqualityComparer<Recipient> {
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