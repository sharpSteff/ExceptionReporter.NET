using System;
using System.Collections.Specialized;
using System.Net;
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

			client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

			client.UploadValuesCompleted += (sender, e) =>
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

			client.UploadValuesAsync(new Uri(_reportInfo.WebServiceUrl), "POST", new NameValueCollection
			{
				{"ExceptionReport", report},
				{"ExceptionMessage", _reportInfo.MainException.Message},
				{"AppName", _reportInfo.AppName},
				{"AppVersion", _reportInfo.AppVersion},
			});
			
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
