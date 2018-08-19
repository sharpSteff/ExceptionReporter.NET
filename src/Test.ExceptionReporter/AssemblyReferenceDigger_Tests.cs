using System.Reflection;
using ExceptionReporting.Core;
using ExceptionReporting.Report;
using NUnit.Framework;

namespace ExceptionReporting.Tests
{
	public class AssemblyReferenceDigger_Tests
	{
		[Test]
		public void Can_Dig_Assembly_Name()
		{
			var digger = new AssemblyDigger(Assembly.Load("ExceptionReporter.NET"));
			var references = digger.AssemblyRefs();

			Assert.That(references, Does.Contain("System.Windows.Forms, Version="));
		}
	}
}