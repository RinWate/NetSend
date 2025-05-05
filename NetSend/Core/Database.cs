using LiteDB;
using NetSend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace NetSend.Core;

public class Database {

	/*
     * Описание баз данных:
     * Персональные:
     *      - favourites: избранные получатели
     *      - settings: настройки приложения
     * Общие:
     *      - CommonBase: список получателей, история сообщений и задания на отложенные рассылки
     *      - IgnoredBase: игнорируемые получатели (исключены из сканирования и не отображаются в общем списке)
     *      - PseudonamesBase: список установленных псевдонимов
     *      - TemplatesBase: шаблоны сообщений
     */

	private readonly string _favouritesBase = "favourites.litedb";
	private readonly string _settingsBase = "settings.litedb";

	private string GetDatabasePath(Databases database) {
		Settings.GetValue(database.ToString(), out var value);
		if (string.IsNullOrWhiteSpace(value)) return $"{database.ToString()}.litedb";
		return value;
	}

	public void WriteSettings(List<Setting> settings) {
		using (var db = new LiteDatabase(_settingsBase)) {
			var col = db.GetCollection<Setting>(Tables.Settings);
			col.DeleteAll();
			col.Insert(settings);
		}
	}

	public List<Setting> ReadSettings() {
		using (var db = new LiteDatabase(_settingsBase)) {
			var col = db.GetCollection<Setting>(Tables.Settings);
			var result = col.FindAll().ToList();
			return result;
		}
	}

	private enum Databases : byte {
		CommonBase,
		PseudonamesBase,
		TemplatesBase,
		IgnoredBase
	}

	#region Scheduler

	public void WriteToScheduler(List<Message> messages) {
		using (var db = new LiteDatabase(GetDatabasePath(Databases.CommonBase))) {
			var col = db.GetCollection<Schedule>(Tables.Schedules);
		}
	}

	public void ReadScheduleById(int id) {
		using (var db = new LiteDatabase(GetDatabasePath(Databases.CommonBase))) {
			var col = db.GetCollection<Schedule>(Tables.Schedules);
		}
	}

	public void RemoveAllSchedules() {
		using (var db = new LiteDatabase(GetDatabasePath(Databases.CommonBase))) {
			var col = db.GetCollection<Schedule>(Tables.Schedules);
		}
	}

	#endregion

	#region Templates

	public void WriteTemplate(Template template) {
		using (var db = new LiteDatabase(GetDatabasePath(Databases.TemplatesBase))) {
			var col = db.GetCollection<Template>("templates");
		}
	}

	public List<Template> ReadTemplates() {
		using (var db = new LiteDatabase(GetDatabasePath(Databases.TemplatesBase))) {
			var col = db.GetCollection<Template>("templates");
		}

		return new List<Template>();
	}

	#endregion

	#region Favourites

	public void SetFavourite(List<Recipient> recipients) {
		using (var db = new LiteDatabase(_favouritesBase)) {
			var col = db.GetCollection<Favourite>(Tables.Favourites);
			var favourites = new List<Favourite>();
			foreach (var recipient in recipients) favourites.Add(new Favourite(recipient.Address));
			col.Insert(favourites);
		}
	}

	public void ClearFavourite(List<Recipient> recipients) {
		using (var db = new LiteDatabase(_favouritesBase)) {
			var col = db.GetCollection<Favourite>(Tables.Favourites);
			var addresses = recipients.Select(e => e.Address).ToHashSet();
			col.DeleteMany(e => addresses.Contains(e.Address));
		}
	}

	public List<Favourite> ReadAllFavourites() {
		using (var db = new LiteDatabase(_favouritesBase)) {
			var col = db.GetCollection<Favourite>(Tables.Favourites);
			return col.FindAll().ToList();
		}
	}

	#endregion

	#region Messages

	public void WriteMessage(string message) {
		using (var db = new LiteDatabase(GetDatabasePath(Databases.CommonBase))) {
			var col = db.GetCollection<Message>(Tables.Messages);
			var messages = new List<Message> {
				new(message)
			};

			col.Insert(messages);
		}
	}

	public List<Message> AllMessages() {
		using (var db = new LiteDatabase(GetDatabasePath(Databases.CommonBase))) {
			var col = db.GetCollection<Message>(Tables.Messages);
			var messages = col.FindAll().OrderByDescending(a => a.SendDate).ToList();
			return messages;
		}
	}

	public void ClearMessages() {
		using (var db = new LiteDatabase(GetDatabasePath(Databases.CommonBase))) {
			var col = db.GetCollection<Message>(Tables.Messages);
			col.DeleteAll();
		}
	}

	public void DeleteMessage(int id) {
		using (var db = new LiteDatabase(GetDatabasePath(Databases.CommonBase))) {
			var col = db.GetCollection<Message>(Tables.Messages);
			col.Delete(id);
		}
	}

	#endregion

	#region FilterHistory

	public void WriteFilter(string filter_string) {
		var filter = new Filter(filter_string);
		using (var db = new LiteDatabase(_settingsBase)) {
			var col = db.GetCollection<Filter>(Tables.Filters);
			var filters = new List<Filter> { filter };

			var existingFilter = col.Find(f => f.filter == filter_string).FirstOrDefault();
			if (existingFilter == null) col.Insert(filters);
		}
	}

	public List<string> GetAllFilters() {
		using (var db = new LiteDatabase(_settingsBase)) {
			var col = db.GetCollection<Filter>(Tables.Filters);
			var result = new List<string>();
			foreach (var filter in col.FindAll()) result.Add(filter.filter);
			return result;
		}
	}

	#endregion

	#region Recipients

	public void WriteRecipients(List<Recipient> recipients) {
		if (recipients.Count == 0) return;

		using (var db = new LiteDatabase(GetDatabasePath(Databases.CommonBase))) {
			var col = db.GetCollection<Recipient>(Tables.Recipients);
			col.DeleteAll();
			col.Insert(recipients);
		}
	}

	public void WriteRecipient(Recipient recipient) {
		if (recipient == null) return;

		using (var db = new LiteDatabase(GetDatabasePath(Databases.CommonBase))) {
			var col = db.GetCollection<Recipient>(Tables.Recipients);
			var founded = col.FindOne(r => r.Address == recipient.Address);
			if (founded == null) {
				col.Insert(recipient);
			} else {
				recipient.Id = founded.Id;
				col.Update(recipient);
			}
		}
	}

	public List<Recipient> ReadRecipients() {
		var pseudonames = ReadAllPseudoNames();
		var favourites = ReadAllFavourites();
		var ignored = ReadAllIgnoredRecipients();

		using (var db = new LiteDatabase(GetDatabasePath(Databases.CommonBase))) {
			var col = db.GetCollection<Recipient>(Tables.Recipients);
			var result = col.FindAll().ToList();

			foreach (var recipient in result) {
				var recipientIp = recipient.Address.ToString();
				var foundedPseudoname = pseudonames.Find(e => e.Address.ToString() == recipientIp);
				var foundedFavourite = favourites.Find(e => e.Address.ToString() == recipientIp);
				var isIgnored = ignored.Find(e => e.Address.ToString() == recipientIp) != null;

				recipient.IsIgnored = isIgnored;

				if (foundedPseudoname != null) recipient.PseudoName = foundedPseudoname.Name ?? "";
				else recipient.PseudoName = "";

				if (foundedFavourite != null) recipient.IsFavourite = true;
				else recipient.IsFavourite = false;
			}

			result.RemoveAll(e => e.IsIgnored);

			return result;
		}
	}

	#endregion

	#region Pseudonames

	public void WritePseudoName(IPAddress address, string pseudoName) {
		using (var db = new LiteDatabase(GetDatabasePath(Databases.PseudonamesBase))) {
			var col = db.GetCollection<Pseudoname>(Tables.Pseudonames);
			var foundedValue = col.FindOne(e => e.Address == address);
			if (foundedValue != null) {
				foundedValue.Name = pseudoName;
				col.Update(foundedValue);
			} else {
				col.Insert(new Pseudoname(address, pseudoName));
			}
		}
	}

	public string ReadPseudoName(IPAddress address) {
		using (var db = new LiteDatabase(GetDatabasePath(Databases.PseudonamesBase))) {
			var col = db.GetCollection<Pseudoname>(Tables.Pseudonames);
			var foundedName = col.FindAll().FirstOrDefault(e => e.Address == address);
			return foundedName?.Name ?? "";
		}
	}

	public List<Pseudoname> ReadAllPseudoNames() {
		using (var db = new LiteDatabase(GetDatabasePath(Databases.PseudonamesBase))) {
			var col = db.GetCollection<Pseudoname>(Tables.Pseudonames);
			var result = col.FindAll().ToList();
			return result;
		}
	}

	#endregion

	#region Ignored

	public void AddRecipientToIgnore(List<IgnoredRecipient> ignoredRecipients) {
		using (var db = new LiteDatabase(GetDatabasePath(Databases.IgnoredBase))) {
			var col = db.GetCollection<IgnoredRecipient>(Tables.IgnoredRecipients);
			col.Upsert(ignoredRecipients);
		}
	}

	public void RemoveRecipientsFromIgnore(List<IgnoredRecipient> ignoredRecipients) {
		if (ignoredRecipients == null) return;
		using (var db = new LiteDatabase(GetDatabasePath(Databases.IgnoredBase))) {
			var col = db.GetCollection<IgnoredRecipient>(Tables.IgnoredRecipients);
			var idsToRemove = ignoredRecipients.Select(e => e.Id).ToHashSet();
			col.DeleteMany(e => idsToRemove.Contains(e.Id));
		}
	}

	public void UpdateIgnoredRecipient(IgnoredRecipient recipient) {
		using (var db = new LiteDatabase(GetDatabasePath(Databases.IgnoredBase))) {
			var col = db.GetCollection<IgnoredRecipient>(Tables.IgnoredRecipients);
			col.Update(recipient);
		}
	}

	public List<IgnoredRecipient> ReadAllIgnoredRecipients() {
		using (var db = new LiteDatabase(GetDatabasePath(Databases.IgnoredBase))) {
			var col = db.GetCollection<IgnoredRecipient>(Tables.IgnoredRecipients);
			var result = col.FindAll().ToList();
			return result;
		}
	}

	public void RemoveAllIgnoredRecipients() {
		using (var db = new LiteDatabase(GetDatabasePath(Databases.IgnoredBase))) {
			var col = db.GetCollection<IgnoredRecipient>(Tables.IgnoredRecipients);
			col.DeleteAll();
		}
	}

	#endregion
}

internal static class Tables {
	public static string Recipients { get; } = "recipients";
	public static string IgnoredRecipients { get; } = "ignored";
	public static string Settings { get; } = "settings";
	public static string Schedules { get; } = "schedules";
	public static string Messages { get; } = "messages";
	public static string Favourites { get; } = "favourites";
	public static string Pseudonames { get; } = "pseudonames";
	public static string Filters { get; } = "filters";
}