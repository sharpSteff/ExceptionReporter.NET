using System.Collections.Generic;
using System.Linq;
using AutoMoq;
using ExceptionReporting.MVP.Views;
using ExceptionReporting.Report;
using ExceptionReporting.SystemInfo;
using ExceptionReporting.Templates;
using NUnit.Framework;

namespace ExceptionReporting.Tests
{
	public class ReportBuilder_Tests
	{
		private ReportBag _bag;

		[SetUp]
		public void SetUp()
		{
			_bag = new ReportBag(new ErrorDetail
			{
				MainException = new TestException()
			}, new ReportConfig
			{
				AppName = "TestApp",
				AppVersion = "1.0"
			});
		}
		
		[Test]
		public void Can_Build_Basic_Model_Properties()
		{
			var moqer = new AutoMoqer();
			var reportBuilder = moqer.Create<ReportBuilder>();

			var model = reportBuilder.ReportModel(_bag);
			
			Assert.That(model.App.Name, Is.EqualTo("TestApp"));
			Assert.That(model.App.Version, Is.EqualTo("1.0"));
			Assert.That(model.Error.Message, Is.EqualTo(TestException.ErrorMessage));
		}

		[Test]
		public void Can_Invoke_Dependencies()
		{
			var moqer = new AutoMoqer();
			var reportBuilder = moqer.Create<ReportBuilder>();
			
			moqer.GetMock<IAssemblyDigger>().Setup(ad => ad.GetAssemblyRefs()).Returns(new List<AssemblyRef>
			{
				new AssemblyRef
				{
					Name = "Assembly1",
					Version = "2.1"
				}
			});
			moqer.GetMock<ISysInfoResultMapper>().Setup(si => si.SysInfoString()).Returns("fake tree");
			moqer.GetMock<IStackTraceMaker>().Setup(st => st.FullStackTrace()).Returns("fake stack trace");
			
			var model = reportBuilder.ReportModel(_bag);
			
			Assert.That(model.App.AssemblyRefs.First().Name, Is.EqualTo("Assembly1"));
			Assert.That(model.SystemInfo, Is.EqualTo("fake tree"));
			Assert.That(model.Error.FullStackTrace, Is.EqualTo("fake stack trace"));
		}
	}
}