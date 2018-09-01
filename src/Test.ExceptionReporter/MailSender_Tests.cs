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
			var exception = new Exception("hello");
			var config = new ReportConfig { TitleText = "test" };
			var error = new ErrorData();
			error.SetExceptions(new[] { exception });
			var mailSender = new MapiMailSender(config, error, null);

			Assert.That(mailSender.EmailSubject, Is.EqualTo("hello"));
		}

		/// <summary>
		/// Logically and because the MailMessage class will throw an exception, we don't want CR/LF
		/// </summary>
		[Test]
		public void Can_Create_Subject_Without_CrLf()
		{
			var config = new ReportConfig();
			var error = new ErrorData();
			error.SetExceptions(new [] {new Exception("hello\r\nagain")});
			
			var mailSender = new MapiMailSender(config, error, null);

			Assert.That(mailSender.EmailSubject, Does.Not.Contain("\r"));
			Assert.That(mailSender.EmailSubject, Does.Not.Contain("\n"));
		}

		[Test]
		public void Can_Create_Subject_If_Exception_Is_Null()
		{
			var mailSender = new MapiMailSender(new ReportConfig(), new ErrorData(), null);		// no exceptions set, so message will be null, does mail cater for it?

			Assert.That(mailSender.EmailSubject, Is.EqualTo("ExceptionReporter given 0 exceptions"));		// reverts to a default message
		}
	}
}