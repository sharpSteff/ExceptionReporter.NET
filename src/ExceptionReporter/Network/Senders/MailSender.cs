using ExceptionReporting.Core;
using ExceptionReporting.Mail;
using ExceptionReporting.Network.Events;

namespace ExceptionReporting.Network.Senders
{
	internal abstract class MailSender
	{
		private readonly ReportBag _bag;
		protected readonly IReportSendEvent _sendEvent;
		protected readonly Attacher _attacher;

		protected MailSender(ReportBag bag, IReportSendEvent sendEvent)
		{
			_bag = bag;
			_sendEvent = sendEvent ?? new SilentSendEvent();
			_attacher = new Attacher(bag.Config);
		}

		public abstract string Description { get; }

		public virtual string ConnectingMessage
		{
			get { return string.Format("Connecting {0}...", Description); }
		}

		public string EmailSubject
		{
			get { return _bag.Error.MainException.Message
				             .Replace('\r', ' ')
				             .Replace('\n', ' ')
				             .Truncate(255) ?? "Exception Report"; }
		}
	}
}