using Avalonia.Controls.Platform.Surfaces;
using Avalonia.Diagnostics;
using LiteDB;
using NetSend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace NetSend.Core {
	public class Database {

		private readonly string _settingsDb = "settings.litedb"; // Настройки приложения, фильтры
		private readonly string _pseudoNamesDb = "pseudo.litedb"; // Все псевдоимена получателей
		private readonly string _dataDb = "data.litedb"; // Отправленные сообщения

		public void WriteSetting(Setting setting) {
			using (var db = new LiteDatabase(_settingsDb)) {
				var col = db.GetCollection<Setting>("settings");
				var newSetting = new List<Setting>();
				col.Upsert(setting);
			}
		}

		public void WriteSettings(List<Setting> settings) {
			using (var db = new LiteDatabase(_settingsDb)) {
				var col = db.GetCollection<Setting>("settings");
				col.Upsert(settings);
			}
		}

		public Setting ReadSetting(string name) {
			using (var db = new LiteDatabase(_settingsDb)) {
				var col = db.GetCollection<Setting>("settings");
				var result = col.Find(name).FirstOrDefault();
				if (result == null) throw new Exception("Setting not found");
				return result;
			}
		}

		public List<Setting> ReadSettings() {
			using (var db = new LiteDatabase(_settingsDb)) {
				var col = db.GetCollection<Setting>("settings");
				var result = col.FindAll().ToList();
				return result;
			}
		}

		#region Messages
		public void WriteMessage(string message) { 
			using (var db = new LiteDatabase(_dataDb)) {
				var col = db.GetCollection<Message>("messages");
				var messages = new List<Message> {
					new Message(message)
				};

				col.Insert(messages);
			}
		}

		public List<Message> AllMessages() {
			using (var db = new LiteDatabase(_dataDb)) {
				var col = db.GetCollection<Message>("messages");
				var messages = col.FindAll().OrderByDescending(a => a.SendDate).ToList();
				return messages;
			}
		}

		public void ClearMessages() {
			using (var db = new LiteDatabase(_dataDb)) {
				var col = db.GetCollection<Message>("messages");
				col.DeleteAll();
			}
		}

		public void DeleteMessage(int id) {
			using (var db = new LiteDatabase(_dataDb)) {
				var col = db.GetCollection<Message>("messages");
				col.Delete(id);
			}
		}
		#endregion

		#region FilterHistory

		public void WriteFilter(string filter_string) {
			var filter = new Filter(filter_string);
			using (var db = new LiteDatabase(_settingsDb)) {
				var col = db.GetCollection<Filter>("filters");
				var filters = new List<Filter>() { filter };

				var existingFilter = col.Find(f => f.filter == filter_string).FirstOrDefault();
				if (existingFilter == null) col.Insert(filters);
			}
		}

		public List<string> GetAllFilters() {
			using (var db = new LiteDatabase(_settingsDb)) {
				var col = db.GetCollection<Filter>("filters");
				var result = new List<string>();
				foreach (var filter in col.FindAll()) { 
					result.Add(filter.filter);
				}
				return result;
			}
		}

		#endregion

		#region Recipients

		public void WriteRecipients(List<Recipient> recipients) {
			if (recipients.Count == 0) return;

			using (var db = new LiteDatabase(_dataDb)) {
				var col = db.GetCollection<Recipient>("recipients");
				col.Insert(recipients);
			}
		}

		public List<Recipient> ReadRecipients() {

			var pseudonames = ReadAllPseudoNames();

			using (var db = new LiteDatabase(_dataDb)) {
				var col = db.GetCollection<Recipient>("recipients");
				var result = col.FindAll().ToList();

				foreach (var recipient in result) {
					var foundedPseudoname = pseudonames.Find(e => e.Address.ToString() == recipient.Address.ToString());
					if (foundedPseudoname != null) recipient.PseudoName = foundedPseudoname.Name;
				}

				return result;
			}
		}

		#endregion

		#region PseudoNames

		public void WritePseudoName(IPAddress address, string pseudoName) {
			using (var db = new LiteDatabase(_pseudoNamesDb)) {
				var col = db.GetCollection<Pseudoname>("pseudonames");
				var newName = new List<Pseudoname> {
					new Pseudoname(address, pseudoName)
				};
				col.Upsert(newName);
			}
		}

		public string ReadPseudoName(IPAddress address) { 
			using (var db = new LiteDatabase(_pseudoNamesDb)) {
				var col = db.GetCollection<Pseudoname>("pseudonames");
				var foundedName = col.FindAll().FirstOrDefault(e => e.Address == address);
				return foundedName?.Name ?? "";
			}
		}

		public List<Pseudoname> ReadAllPseudoNames() { 
			using (var db = new LiteDatabase(_pseudoNamesDb)) {
				var col = db.GetCollection<Pseudoname>("pseudonames");
				var result = col.FindAll().ToList();
				return result;
			}
		}

		#endregion
	}
}
