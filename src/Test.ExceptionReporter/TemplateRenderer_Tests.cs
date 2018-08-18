using System;
using System.Collections.Generic;
using ExceptionReporting.Templates;
using NUnit.Framework;

namespace ExceptionReporting.Tests
{
	public class TemplateRenderer_Tests
	{
		[Test]
		public void Can_Render_Text_Template()
		{
			var renderer = new TemplateRenderer(new ReportModel
			{
				Exception = new Error
				{
					Exception = new TestException(),
					When = DateTime.Today
				},
				App = new App
				{
					Name = "TestApp",
					User = "Bob",
					Version = "4.0.1",
					Assemblies = new List<Assembly>
					{
						new Assembly
						{
							Name = "Assembly 1",
							Version = "1.2.3"
						}
					}
				}
			});
			var result = renderer.Render();
			Assert.That(result.Contains("Message: Test Exception"));
		}
	}
}

public class TestException : Exception
{
	public TestException() : base("Test Exception")
	{ }
}