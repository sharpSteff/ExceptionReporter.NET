using System;
using System.Reflection;
using NUnit.Framework;

namespace ExceptionReporting.Tests
{
	public class ReportGenerator_Tests
	{
		private ReportGenerator _reportGenerator;

		[SetUp]
		public void SetUp()
		{
			_reportGenerator = new ReportGenerator(new ReportConfig(), new ErrorData
			{
				MainException = new Exception(),
				AppAssembly = Assembly.GetExecutingAssembly()
			});
		}

		[Test]
		public void Can_Deal_With_Null_In_Constructor()
		{
			Assert.Throws<ArgumentNullException>(
				() => _reportGenerator = new ReportGenerator(null, new ErrorData()), "reportInfo cannot be null");
		}

		[Test]
		public void Can_Create_Report_With_A_Couple_Of_Minimal_Bits_That_Should_Exist()
		{
			if (ExceptionReporter.IsRunningMono()) return;
			var report = _reportGenerator.Generate();
			var reportString = report;

			Assert.That(reportString, Does.Contain("Application:"));
			Assert.That(reportString, Does.Contain("Version:"));
			Assert.That(reportString, Does.Contain("TotalPhysicalMemory ="));
		}

		[Test]
		public void Can_Create_Report_With_Local_Datetime()
		{
			var error = new ErrorData
			{
				MainException = new Exception()
			};
			// ReSharper disable once ObjectCreationAsStatement
			new ReportGenerator(new ReportConfig
			{
				ExceptionDateKind = DateTimeKind.Local
			}, error);
			Assert.That(error.ExceptionDate.Kind, Is.EqualTo(DateTimeKind.Local));
		}
	}
}