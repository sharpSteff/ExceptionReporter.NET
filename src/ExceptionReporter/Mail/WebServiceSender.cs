using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using ExceptionReporting.Core;

namespace ExceptionReporting.Mail
{
	internal class WebServiceSender
	{
		private readonly ExceptionReportInfo _reportInfo;
		private readonly IReportSendEvent _sendEvent;

		internal WebServiceSender(ExceptionReportInfo reportInfo, IReportSendEvent sendEvent)
		{
			_reportInfo = reportInfo;
			_sendEvent = sendEvent;
		}

		public void Send(string report)
		{
			var client = new ExceptionReporterWebClient(_reportInfo.WebServiceTimeout)
			{
				Encoding = Encoding.UTF8,
			};

			client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
			client.Headers.Add(HttpRequestHeader.Accept, "application/json");
			client.UploadStringCompleted += (sender, e) =>
			{
				try
				{
					if (e.Error == null)
					{
						_sendEvent.Completed(success: true);
					}
					else
					{
						_sendEvent.Completed(success: false);
						_sendEvent.ShowError(e.Error.Message, e.Error);
					}
				}
				finally
				{
					client.Dispose();
				}
			};

			using (var json = new MemoryStream())
			{
				var sz = new DataContractJsonSerializer(typeof(ExceptionReportItem));
				sz.WriteObject(json, new ExceptionReportItem()
				{
					AppName = _reportInfo.AppName,
					AppVersion = _reportInfo.AppVersion,
					ExceptionMessage = _reportInfo.MainException.Message,
					ExceptionReport = report
				});
				client.UploadStringAsync(new Uri(_reportInfo.WebServiceUrl), json.ToString());
			}
		}
	}

	internal class ExceptionReporterWebClient : WebClient
	{
		private readonly int _timeout;

		public ExceptionReporterWebClient(int timeout)
		{
			_timeout = timeout;
		}

		protected override WebRequest GetWebRequest(Uri uri)
		{
			var w = base.GetWebRequest(uri);
			w.Timeout = _timeout * 1000;
			return w;
		}
	}
}
