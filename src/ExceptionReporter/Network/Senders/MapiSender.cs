using ExceptionReporting.Mail;
using ExceptionReporting.Network.Events;
using Win32Mapi;

namespace ExceptionReporting.Network.Senders
{
	internal class MapiSender : MailSender, IReportSender
	{
		public MapiSender(ExceptionReportInfo reportInfo, IReportSendEvent sendEvent) : base(reportInfo, sendEvent)
		{ }
		
		public string Description
		{
			get { return "SimpleMAPI"; }
		}
		
		/// <summary>
		/// Try send via installed Email client
		/// Uses Simple-MAPI.NET library - https://github.com/PandaWood/Simple-MAPI.NET
		/// </summary>
		public void Send(string exceptionReport)
		{
			var mapi = new SimpleMapi();

			mapi.AddRecipient(_config.EmailReportAddress, null, false);
			_attacher.AttachFiles(new AttachAdapter(mapi));

			mapi.Send(EmailSubject, exceptionReport);
		}
	}
}