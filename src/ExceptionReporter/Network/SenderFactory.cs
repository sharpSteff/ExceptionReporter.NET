using ExceptionReporting.Network.Events;
using ExceptionReporting.Network.Senders;

namespace ExceptionReporting.Network
{
	internal class SenderFactory
	{
		private readonly ReportConfig _config;
		private readonly ErrorData _error;
		private readonly IReportSendEvent _sendEvent;

		public SenderFactory(ReportConfig config, ErrorData error, IReportSendEvent sendEvent)
		{
			_config = config;
			_error = error;
			_sendEvent = sendEvent;
		}

		public IReportSender Get()
		{
			switch (_config.SendMethod)
			{
				case ReportSendMethod.WebService:
					return new WebServiceSender(_config, _error, _sendEvent);
				case ReportSendMethod.SMTP:
					return new SmtpMailSender(_config, _error, _sendEvent);
				case ReportSendMethod.SimpleMAPI:
					return new MapiMailSender(_config, _error, _sendEvent);
				case ReportSendMethod.None:
					return new GhostSender();
				default:
					return new GhostSender();
			}
		}
	}
}