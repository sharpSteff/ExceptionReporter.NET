using ExceptionReporting.Network.Events;
using ExceptionReporting.Network.Senders;

namespace ExceptionReporting.Network
{
	internal class SenderFactory
	{
		private readonly ReportBag _bag;
		private readonly IReportSendEvent _sendEvent;

		public SenderFactory(ReportBag bag, IReportSendEvent sendEvent)
		{
			_bag = bag;
			_sendEvent = sendEvent;
		}

		public IReportSender Get()
		{
			switch (_bag.Config.SendMethod)
			{
				case ReportSendMethod.WebService:
					return new WebServiceSender(_bag, _sendEvent);
				case ReportSendMethod.SMTP:
					return new SmtpMailSender(_bag, _sendEvent);
				case ReportSendMethod.SimpleMAPI:
					return new MapiMailSender(_bag, _sendEvent);
				case ReportSendMethod.None:
					return new GhostSender();
				default:
					return new GhostSender();
			}
		}
	}
}