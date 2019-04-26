using ExceptionReporting.Core;
using ExceptionReporting.Network.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionReporting.Network.Senders
{
  internal class MailtoMailSender : MailSender, IReportSender
  {
	public MailtoMailSender(ExceptionReportInfo reportInfo, IReportSendEvent sendEvent) :
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

	  string command = string.Format("mailto:{0}?subject={1}?body={2}", _config.EmailReportAddress, EmailSubject,Uri.EscapeDataString(report));
	  System.Diagnostics.Process.Start(command);
	  //mapi.Send(EmailSubject, report);
	}
  }
}
