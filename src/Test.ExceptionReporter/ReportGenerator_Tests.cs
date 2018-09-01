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
			_reportGenerator = new ReportGenerator(new ReportBag(new ErrorDetail
			{
				MainException = new Exception(),
				AppAssembly = Assembly.GetExecutingAssembly()
			}, new ReportConfig()));
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
			var error = new ErrorDetail
			{
				MainException = new Exception()
			};
			// ReSharper disable once ObjectCreationAsStatement
			new ReportGenerator(new ReportBag(error, new ReportConfig
			{
				ExceptionDateKind = DateTimeKind.Local
			}));
			Assert.That(error.ExceptionDate.Kind, Is.EqualTo(DateTimeKind.Local));
		}
	}
}