using Avalonia.Controls;
using NetSend.Models;
using NetSend.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NetSend.Core;

public class Sender {

	/// <summary>
	/// Отправить заданным получателям сообщение
	/// </summary>
	/// <param name="message">Сообщение для отправки</param>
	/// <param name="parent">Родительское окно</param>
	/// <param name="recipients">Список получателей</param>
	/// <returns>Task</returns>
	public async Task Send(string message, Window parent, List<Recipient>? recipients = null) {
		var tasks = new List<Task>();
		var long_operation = new LongProcessWindow();
		long_operation.DataContext = new LongProcessWindowViewModel("Отправка...");
		_ = long_operation.ShowDialog(parent);

		var target_recipients = recipients ?? Global.Recipients.ToList();

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
		new Database().WriteMessage(message);
	}
}