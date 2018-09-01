using NUnit.Framework;

namespace ExceptionReporting.Tests
{
	public class ReportConfig_Tests
	{
		private ReportConfig _config;

		[SetUp]
		public void SetUp()
		{
			_config = new ReportConfig();
		}

		[TestCase("test",     ExpectedResult = "test.zip")]
		[TestCase("test.zip", ExpectedResult = "test.zip")]
		public string Can_Determine_AttachmentFilename(string attachment)
		{
			_config.AttachmentFilename = attachment;
			return _config.AttachmentFilename;
		}

		[TestCase(ReportSendMethod.None,       ExpectedResult = false)]
		[TestCase(ReportSendMethod.SimpleMAPI, ExpectedResult = true)]
		[TestCase(ReportSendMethod.SMTP,       ExpectedResult = false)]
		[TestCase(ReportSendMethod.WebService, ExpectedResult = false)]
		public bool Can_Determine_IsSimpleMAPI(ReportSendMethod method)
		{
			_config.SendMethod = method;
			return _config.IsSimpleMAPI();
		}
		
		[TestCase(ReportSendMethod.None, true,       ExpectedResult = false)]
		[TestCase(ReportSendMethod.SMTP, true,       ExpectedResult = true)]
		[TestCase(ReportSendMethod.SMTP, false,      ExpectedResult = false)]
		[TestCase(ReportSendMethod.SimpleMAPI, true, ExpectedResult = true)]
		[TestCase(ReportSendMethod.WebService, true, ExpectedResult = true)]
		public bool Can_Determine_ShowEmailButton(ReportSendMethod method, bool show)
		{
			_config.SendMethod = method;
			_config.ShowEmailButton = show;
			return _config.ShowEmailButton;
		}
	}
}