using Avalonia.Controls.Platform.Surfaces;
using LiteDB;
using NetSend.Models;
using System.Collections.Generic;
using System.Linq;

namespace NetSend.Core {
	public class Database {

		private readonly string _settingsDb = "settings.litedb";
		private readonly string _pseudoNamesDb = "pseudo.litedb";
		private readonly string _dataDb = "data.litedb";

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

		public void WriteNew(string filter_string) {
			var filter = new Filter(filter_string);
			using (var db = new LiteDatabase(_dataDb)) {
				var col = db.GetCollection<Filter>("filters");
				var filters = new List<Filter>() { filter };

				var existingFilter = col.Find(f => f.filter == filter_string).FirstOrDefault();
				if (existingFilter == null) col.Insert(filters);
			}
		}

		public List<string> GetAll() {
			using (var db = new LiteDatabase(_dataDb)) {
				var col = db.GetCollection<Filter>("filters");
				var result = new List<string>();
				foreach (var filter in col.FindAll()) { 
					result.Add(filter.filter);
				}
				return result;
			}
		}

		#endregion
	}
}
