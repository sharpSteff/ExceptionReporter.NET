using System;
using ExceptionReporting.Network.Senders;
using NUnit.Framework;

namespace ExceptionReporting.Tests
{
	public class MailSender_Tests
	{
		[Test]
		public void Can_Create_Subject()
		{
			var mailSender = new MapiMailSender(new ReportBag(new ErrorDetail
			{
				MainException = new Exception("hello")
			}, new ReportConfig
			{
				TitleText = "test"
			}));

			Assert.That(mailSender.EmailSubject, Is.EqualTo("hello"));
		}

		/// <summary>
		/// Logically and because the MailMessage class will throw an exception, we don't want CR/LF
		/// </summary>
		[Test]
		public void Can_Create_Subject_Without_CrLf()
		{
			var mailSender = new MapiMailSender(new ReportBag(new ErrorDetail
			{
				MainException = new Exception("hello\r\nagain")
			}, new ReportConfig()));

			Assert.That(mailSender.EmailSubject, Does.Contain("again"));
			Assert.That(mailSender.EmailSubject, Does.Not.Contain("\r"));
			Assert.That(mailSender.EmailSubject, Does.Not.Contain("\n"));
		}
	}
}