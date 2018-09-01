using ExceptionReporting.Core;
using ExceptionReporting.Mail;
using ExceptionReporting.Network.Events;

namespace ExceptionReporting.Network.Senders
{
	internal abstract class MailSender
	{
		protected readonly ReportConfig _config;
		private readonly ErrorData _error;
		protected readonly IReportSendEvent _sendEvent;
		protected readonly Attacher _attacher;

		protected MailSender(ReportConfig reportConfig, ErrorData error, IReportSendEvent sendEvent)
		{
			_config = reportConfig;
			_error = error;
			_sendEvent = sendEvent;
			_attacher = new Attacher(reportConfig);
		}

		public abstract string Description { get; }

		public virtual string ConnectingMessage
		{
			get { return string.Format("Connecting {0}...", Description); }
		}

		public string EmailSubject
		{
			get { return _error.MainException.Message
				             .Replace('\r', ' ')
				             .Replace('\n', ' ')
				             .Truncate(255) ?? "Exception Report"; }
		}
	}
}