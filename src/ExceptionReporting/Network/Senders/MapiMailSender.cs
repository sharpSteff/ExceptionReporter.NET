using System.Configuration;
using ExceptionReporting.Core;
using ExceptionReporting.Mail;
using ExceptionReporting.Network.Events;
using Win32Mapi;

namespace ExceptionReporting.Network.Senders
{
	internal class MapiMailSender : MailSender, IReportSender
	{
		public MapiMailSender(ExceptionReportInfo reportInfo, IReportSendEvent sendEvent) : 
			base(reportInfo, sendEvent)
		{ }

		public override string Description
		{
			get { return "Email Client"; }
		}
		
		public override string ConnectingMessage
		{
			get { return string.Format("Launching {0}...", Description); }
		}
		
		/// <summary>
		/// Try send via installed Email client
		/// Uses Simple-MAPI.NET library - https://github.com/PandaWood/Simple-MAPI.NET
		/// </summary>
		public void Send(string report)
		{
			if (_config.EmailReportAddress.IsEmpty())
			{
				_sendEvent.ShowError("EmailReportAddress not set", new ConfigurationErrorsException("EmailReportAddress"));
				return;
			}
			
			var mapi = new SimpleMapi();
			//mapi.Logon(System.IntPtr.Zero);		// this code is strictly correct, but won't work with Outlook 64-bit and is needed in the most common usage

			mapi.AddRecipient(_config.EmailReportAddress, _config.EmailReportAddress, false);
			//_attacher.AttachFiles(new AttachAdapter(mapi));
			mapi.Send(EmailSubject, report);
		}
	}
}