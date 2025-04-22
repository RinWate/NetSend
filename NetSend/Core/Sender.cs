using Avalonia.Controls;
using NetSend.Models;
using NetSend.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ursa.Controls;

namespace NetSend.Core {
	public class Sender {
		
		public Sender() { }

		public async Task Send(string message, Window parent, List<Recipient>? recipients = null) {

			var errors = new List<string>();
			var tasks = new List<Task>();
			var long_operation = new LongProcessWindow();
			long_operation.DataContext = new LongProcessWindowViewModel("Отправка...");
			_ = long_operation.ShowDialog(parent);

			List<Recipient> target_recipients;
			if (recipients != null) {
				target_recipients = recipients;
			} else { 
				target_recipients = Global.Recipients.ToList();
			}

			foreach (var recipient in target_recipients) {
				var addr = recipient.Address;

				var info = new ProcessStartInfo();
				info.WorkingDirectory = "C:/Windows/System32/";
				info.CreateNoWindow = true;
				info.FileName = "msg.exe";
				info.Arguments = $"* /SERVER:{addr} {message}";

				tasks.Add(Process.Start(info)!.WaitForExitAsync());
			}

			await Task.WhenAll(tasks);

			long_operation.Close();
			var db = new Database();
			db.WriteMessage(message);
		}

	}
}
