using LiteDB;
using NetSend.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NetSend.Core {

	public static class Settings {
		private static List<Setting> settings = new List<Setting>();

		public static void WriteSettings(List<Setting> newSettings) {
			settings = newSettings;
			new Database().WriteSettings(settings);
		}

		public static List<Setting> LoadSettings() {
			settings = new Database().ReadSettings();
			return settings;
		}

		public static void LoadIgnoredRecipients() {
			var ignored = new Database().ReadAllIgnoredRecipients();
			Global.IgnoredRecipients = new ObservableCollection<IgnoredRecipient>(ignored);
		}

		public static List<Setting> GetSettings() {
			return settings;
		}

		public static Setting FindSetting(string name) {
			var foundedSetting = settings.Find(s => s.Name == name);
			if (foundedSetting != null) return foundedSetting;
			return new Setting();
		}

		public static void GetValue(string name, out string value) {
			var foundedSetting = FindSetting(name);
			if (foundedSetting != null && foundedSetting.isEnabled) value = foundedSetting.Value;
			else value = string.Empty;
		}

		public static bool isSet(string name) {
			var db = new Database();

			if (settings.Count == 0) return false;

			var foundedSet = settings.Find(x => x.Name == name);
			if (foundedSet != null) {
				return foundedSet.Value != null;
			} else return false;
		}

		public static void RegisterAdditionalMappings() {
			var mapper = BsonMapper.Global;
			mapper.RegisterType<IPAddress>(
				ip => ip.ToString(),
				str => IPAddress.Parse(str)
			);
		}

		public static void ReloadRecipients() {
			var recipients = new Database().ReadRecipients();
			Global.Recipients = new ObservableCollection<Recipient>(recipients);
		}
	}
}
