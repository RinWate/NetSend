using LiteDB;
using NetSend.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;

namespace NetSend.Core {
	public class Database {

		private readonly string _settingsBase = "settings.litedb";
		private readonly string _favouritesBase = "favourites.litedb";

		private enum Databases : byte {
			CommonBase,
			PseudonamesBase,
			TemplatesBase,
			IgnoredBase,
		}

		private string GetDatabase(Databases database) {
			Settings.GetValue(database.ToString(), out string value);
			if (string.IsNullOrWhiteSpace(value)) return $"{database.ToString()}.litedb";
			else return value;
		}
		public void WriteSetting(Setting setting) {
			using (var db = new LiteDatabase(_settingsBase)) {
				var col = db.GetCollection<Setting>("settings");
				var newSetting = new List<Setting>();
				col.Upsert(setting);
			}
		}

		public void WriteSettings(List<Setting> settings) {
			using (var db = new LiteDatabase(_settingsBase)) {
				var col = db.GetCollection<Setting>("settings");
				col.Upsert(settings);
			}
		}

		public Setting ReadSetting(string name) {
			using (var db = new LiteDatabase(_settingsBase)) {
				var col = db.GetCollection<Setting>("settings");
				var result = col.Find(name).FirstOrDefault();
				if (result == null) throw new Exception("Setting not found");
				return result;
			}
		}

		public List<Setting> ReadSettings() {
			using (var db = new LiteDatabase(_settingsBase)) {
				var col = db.GetCollection<Setting>("settings");
				var result = col.FindAll().ToList();
				return result;
			}
		}

		#region Templates

		public void WriteTemplate(Template template) {
			using (var db = new LiteDatabase(GetDatabase(Databases.TemplatesBase))) {
				var col = db.GetCollection<Template>("templates");
			}
		}

		public List<Template> ReadTemplates() {
			using (var db = new LiteDatabase(GetDatabase(Databases.TemplatesBase))) {
				var col = db.GetCollection<Template>("templates");
			}

			return new List<Template>();
		}

		#endregion

		#region Favourites

		public void SetFavourite(IPAddress address) {
			using (var db = new LiteDatabase(_favouritesBase)) {
				var col = db.GetCollection<Favourite>("favourites");

				var foundedValue = col.FindOne(x => x.Address == address);
				if (foundedValue == null) col.Insert(new Favourite(address));
			}
		}

		public void ClearFavourite(IPAddress address) {
			using (var db = new LiteDatabase(_favouritesBase)) {
				var col = db.GetCollection<Favourite>("favourites");

				var foundedValue = col.FindOne(x => x.Address == address);
				if (foundedValue != null) col.Delete(foundedValue.Id);
			}
		}

		public List<Favourite> ReadAllFavourites() {
			using (var db = new LiteDatabase(_favouritesBase)) {
				var col = db.GetCollection<Favourite>("favourites");
				return col.FindAll().ToList();
			}
		}

		#endregion

		#region Messages
		public void WriteMessage(string message) { 
			using (var db = new LiteDatabase(GetDatabase(Databases.CommonBase))) {
				var col = db.GetCollection<Message>("messages");
				var messages = new List<Message> {
					new Message(message)
				};

				col.Insert(messages);
			}
		}

		public List<Message> AllMessages() {
			using (var db = new LiteDatabase(GetDatabase(Databases.CommonBase))) {
				var col = db.GetCollection<Message>("messages");
				var messages = col.FindAll().OrderByDescending(a => a.SendDate).ToList();
				return messages;
			}
		}

		public void ClearMessages() {
			using (var db = new LiteDatabase(GetDatabase(Databases.CommonBase))) {
				var col = db.GetCollection<Message>("messages");
				col.DeleteAll();
			}
		}

		public void DeleteMessage(int id) {
			using (var db = new LiteDatabase(GetDatabase(Databases.CommonBase))) {
				var col = db.GetCollection<Message>("messages");
				col.Delete(id);
			}
		}
		#endregion

		#region FilterHistory

		public void WriteFilter(string filter_string) {
			var filter = new Filter(filter_string);
			using (var db = new LiteDatabase(_settingsBase)) {
				var col = db.GetCollection<Filter>("filters");
				var filters = new List<Filter>() { filter };

				var existingFilter = col.Find(f => f.filter == filter_string).FirstOrDefault();
				if (existingFilter == null) col.Insert(filters);
			}
		}

		public List<string> GetAllFilters() {
			using (var db = new LiteDatabase(_settingsBase)) {
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

			using (var db = new LiteDatabase(GetDatabase(Databases.CommonBase))) {
				var col = db.GetCollection<Recipient>("recipients");
				col.DeleteAll();
				col.Insert(recipients);
			}
		}

		public List<Recipient> ReadRecipients() {

			var pseudonames = ReadAllPseudoNames();
			var favourites = ReadAllFavourites();
			var ignored = ReadAllIgnoredRecipients();

			using (var db = new LiteDatabase(GetDatabase(Databases.CommonBase))) {
				var col = db.GetCollection<Recipient>("recipients");
				var result = col.FindAll().ToList();

				foreach (var recipient in result) {
					var recipientIp = recipient.Address.ToString();
					var foundedPseudoname = pseudonames.Find(e => e.Address.ToString() == recipientIp);
					var foundedFavourite = favourites.Find(e => e.Address.ToString() == recipientIp);
					var isIgnored = ignored.Find(e => e.Address.ToString() == recipientIp) != null;

					recipient.IsIgnored = isIgnored;

					if (foundedPseudoname != null) recipient.PseudoName = foundedPseudoname.Name;
					else recipient.PseudoName = "";

					if (foundedFavourite != null) recipient.IsFavourite = true;
					else recipient.IsFavourite = false;
				}
				result.RemoveAll(e => e.IsIgnored);

				return result;
			}
		}

		#endregion

		#region PseudoNames

		public void WritePseudoName(IPAddress address, string pseudoName) {
			using (var db = new LiteDatabase(GetDatabase(Databases.PseudonamesBase))) {
				var col = db.GetCollection<Pseudoname>("pseudonames");
				var foundedValue = col.FindOne(e => e.Address == address);
				if (foundedValue != null) {
					foundedValue.Name = pseudoName;
					col.Update(foundedValue);
				} else col.Insert(new Pseudoname(address, pseudoName));
			}
		}

		public string ReadPseudoName(IPAddress address) { 
			using (var db = new LiteDatabase(GetDatabase(Databases.PseudonamesBase))) {
				var col = db.GetCollection<Pseudoname>("pseudonames");
				var foundedName = col.FindAll().FirstOrDefault(e => e.Address == address);
				return foundedName?.Name ?? "";
			}
		}

		public List<Pseudoname> ReadAllPseudoNames() { 
			using (var db = new LiteDatabase(GetDatabase(Databases.PseudonamesBase))) {
				var col = db.GetCollection<Pseudoname>("pseudonames");
				var result = col.FindAll().ToList();
				return result;
			}
		}

		#endregion

		#region Ignored

		public void AddRecipientToIgnore(IgnoredRecipient ignoredRecipient) {
			using (var db = new LiteDatabase(GetDatabase(Databases.IgnoredBase))) {
				var col = db.GetCollection<IgnoredRecipient>("ignored");
				var foundedValue = col.FindOne(e => e.Address == ignoredRecipient.Address);
				if (foundedValue == null) {
					col.Insert(ignoredRecipient);
				};
			}
		}

		public void RemoveRecipientFromIgnore(int id) {
			using (var db = new LiteDatabase(GetDatabase(Databases.IgnoredBase))) {
				var col = db.GetCollection<IgnoredRecipient>("ignored");
				col.Delete(id);
			}
		}

		public void UpdateIgnoredRecipient(IgnoredRecipient recipient) {
			using (var db = new LiteDatabase(GetDatabase(Databases.IgnoredBase))) {
				var col = db.GetCollection<IgnoredRecipient>("ignored");
				col.Update(recipient);
			}
		}

		public List<IgnoredRecipient> ReadAllIgnoredRecipients() {
			using (var db = new LiteDatabase(GetDatabase(Databases.IgnoredBase))) {
				var col = db.GetCollection<IgnoredRecipient>("ignored");
				var result = col.FindAll().ToList();
				return result;
			}
		}

		public void RemoveAllIgnoredRecipients() {
			using (var db = new LiteDatabase(GetDatabase(Databases.IgnoredBase))) {
				var col = db.GetCollection<IgnoredRecipient>("ignored");
				col.DeleteAll();
			}
		}

		#endregion
	}
}
