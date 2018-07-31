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
		private readonly IEmailSendEvent _sendEvent;

		internal WebServiceSender(ExceptionReportInfo reportInfo, IEmailSendEvent sendEvent)
		{
			_reportInfo = reportInfo;
			_sendEvent = sendEvent;
		}

		public void Send(string report)
		{
			using (var client = new WebClient())
			{
				client.Encoding = Encoding.UTF8;
				client.UploadValuesCompleted += (sender, e) =>
				{
					if (e.Error != null)
					{
						_sendEvent.Completed(success: false);
						_sendEvent.ShowError(e.Error.Message, e.Error);
					}
					else
					{
						_sendEvent.Completed(success: true);
					}
				};
				client.UploadValuesAsync(new Uri(_reportInfo.WebServiceUrl), "POST", new NameValueCollection
				{
					{"ExceptionReport", report},
					{"ExceptionMessage", _reportInfo.MainException.Message},
					{"AppName", _reportInfo.AppName},
					{"AppVersion", _reportInfo.AppVersion}
				});
			}
		}
	}
}
