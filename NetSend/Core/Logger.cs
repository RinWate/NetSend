using System;
using System.Collections.Generic;
using System.IO;

namespace NetSend.Core;

/// <summary>
/// Логгер
/// </summary>
/// <remarks>ДОДЕЛАТЬ. На данном этапе выглядит как тупо заглушка -_-</remarks>
public static class Logger {
	private static string _currentLog = string.Empty;

	public static void Log(string exception) {
		if (string.IsNullOrEmpty(_currentLog)) InitLog();
		using (var sw = new StreamWriter(_currentLog, false)) {
			sw.WriteLine(exception);
		}
	}

	public static void LogList(List<string> exceptions) {
		if (string.IsNullOrEmpty(_currentLog)) InitLog();
		using (var sw = new StreamWriter(_currentLog, false)) {
			foreach (var exception in exceptions) sw.WriteLine(exception);
		}
	}

	private static void InitLog() {
		var dir = new DirectoryInfo("logs");
		if (!dir.Exists) dir.Create();

		var currentDate = DateTime.Now.ToString("dd-MM-yyyy");
		var current_dir = new DirectoryInfo($"{dir.Name}/{currentDate}");
		if (!current_dir.Exists) current_dir.Create();

		_currentLog = $"{dir.Name}/{currentDate}/{DateTime.Now.ToString("HH-mm")}.txt";
	}
}