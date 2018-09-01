using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ExceptionReporting.Tests
{
	public class ErrorData_Tests
	{
		private ReportConfig _config;
		private Exception _exception;
		private ErrorData _error;

		[SetUp]
		public void SetUp()
		{
			_config = new ReportConfig();
			_error = new ErrorData();
			_exception = new Exception("test");
		}

		[Test]
		public void Can_Get_And_Set_1_Exception_Without_Knowing_There_Can_Be_Many()
		{
			_error.MainException = _exception;
			Assert.That(_error.MainException, Is.EqualTo(_exception));
		}

		[Test]
		public void Can_Show_Same_Exception_Set_By_Main_Exception_Property()
		{
			_error.MainException = _exception;

			Assert.That(_error.Exceptions.Count, Is.EqualTo(1));
			Assert.That(_error.Exceptions.First(), Is.EqualTo(_exception));
		}

		[Test]
		public void Main_Exception_Shows_First_Exception()
		{
			_error.SetExceptions(new List<Exception>
													{
														new Exception("test1"),
														new Exception("test2")
													});

			Assert.That(_error.MainException.Message, Is.EqualTo("test1"));
		}

		[Test]
		public void Can_Set_Multiple_Exceptions()
		{
			_error.SetExceptions(new List<Exception>
													{
														new Exception("test1"),
														new Exception("test2")
													});

			Assert.That(_error.Exceptions.Count, Is.EqualTo(2));
		}

		[Test]
		public void When_Exception_Already_Exists_Other_Exceptions_Are_Cleared()
		{
			_error.MainException = _exception;
			_error.SetExceptions(new List<Exception>
													{
														new Exception("test1"),
														new Exception("test2")
													});

			Assert.That(_error.Exceptions.Count, Is.Not.EqualTo(3));
			Assert.That(_error.Exceptions.Count, Is.EqualTo(2));
		}

		[Test]
		public void When_Multiple_Exceptions_Already_Exist_Other_Exceptions_Are_Cleared()
		{
			_error.SetExceptions(new List<Exception>
													{
														new Exception("test1"),
														new Exception("test2")
													});

			Assert.That(_error.Exceptions.Count, Is.EqualTo(2));
			_error.MainException = _exception;
			Assert.That(_error.Exceptions.Count, Is.EqualTo(1));
		}
	}
}