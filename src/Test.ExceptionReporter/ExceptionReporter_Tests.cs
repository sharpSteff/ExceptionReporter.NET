using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace ExceptionReporting.Tests
{
	/// <summary>
	/// Testing ExceptionReporter is mostly a case of integration testing (ie using the demo)
	/// We do a little here
	/// </summary>
	public class ExceptionReporter_Tests
	{
		[Test]
		public void Can_Init_App_Assembly()
		{
			var er = new ExceptionReporter();
			Assert.That(er.Config.AppAssembly, Is.Not.Null);
			Assert.That(er.Config.AppAssembly.FullName, Does.Contain("ExceptionReporter.Tests"));
		}

		[TestCase(null,                     ExpectedResult = false)]
		[TestCase(default(List<Exception>), ExpectedResult = false)]
		public bool Can_Prevent_Showing_If_Null_Exception(params Exception[] exceptions)
		{
			var er = new ExceptionReporter();
			return er.Show(exceptions);
		}
	}
}