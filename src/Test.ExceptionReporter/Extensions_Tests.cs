using ExceptionReporting.Core;
using NUnit.Framework;

namespace ExceptionReporting.Tests
{
	public class Extensions_Test
	{
		[Test]
		public void Can_Truncate_Over_Arg()
		{
			Assert.That("123456789abc".Truncate(9), Is.EqualTo("123456789"));
		}
		
		[Test]
		public void Can_Not_Truncate_Under_Arg()
		{
			Assert.That("123".Truncate(4), Is.EqualTo("123"));
		}
	}
}
