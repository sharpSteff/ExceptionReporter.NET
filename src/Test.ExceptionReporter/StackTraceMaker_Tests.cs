using ExceptionReporting.Report;
using NUnit.Framework;

namespace ExceptionReporting.Tests
{
	public class StackTraceMaker_Tests
	{
		[Test]
		public void Can_Create_StackTrace_One_Exception()
		{
			var maker = new StackTraceMaker(new TestException());
			var stackTrace = maker.FullStackTrace();
			
			Assert.That(stackTrace, 
				Is.EqualTo("Top-level Exception\nType:    ExceptionReporting.Tests.TestException\nMessage: NullRef\nSource:\n"));
		}
	}
}