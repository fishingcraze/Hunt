using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hunt.Backend.Functions
{
	public class PushService
	{
		static PushService _instance;
		public static PushService Instance
		{
			get { return _instance ?? (_instance = new PushService()); }
		}

		List<string> _platforms = new List<string> { Keys.MobileCenter.iOSUrl, Keys.MobileCenter.AndroidUrl };

		async public Task<bool> SendNotification(string title, string message, string key, string value, Dictionary<string, string> payload = null)
		{
			var audienceName = $"{key} eq '{value}'";
			var notification = new MobileCenterNotification();
			notification.Target.Type = TargetType.Audience;
			notification.Target.Audiences.Add(audienceName);
			notification.Content.Title = title;
			notification.Content.Body = message;
			notification.Content.CustomData = payload;
			notification.Content.Name = Guid.NewGuid().ToString();

			foreach (var baseUrl in _platforms)
			{
				var url = $"{baseUrl}/push/notifications/";
				var result = await Http.Client.AddMobileCenterToken().Post<JObject>(url, notification);
			}

			return true;
		}

		async public Task<bool> SendNotification(string title, string message, string[] devices, Dictionary<string, string> payload = null)
		{
			var notification = new MobileCenterNotification();
			notification.Target.Type = TargetType.Device;
			notification.Target.Devices = devices;
			notification.Content.Title = title;
			notification.Content.Body = message;
			notification.Content.CustomData = payload;
			notification.Content.Name = devices.First();

			foreach (var baseUrl in _platforms)
			{
				var url = $"{baseUrl}/push/notifications/";
				var result = await Http.Client.AddMobileCenterToken().Post<JObject>(url, notification);
			}

			return true;
		}
	}
}
