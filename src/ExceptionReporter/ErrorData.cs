using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;

namespace ExceptionReporting
{
	/// <summary>
	/// 
	/// </summary>
	public class ErrorData
	{
		readonly List<Exception> _exceptions = new List<Exception>();

		/// <summary>
		/// The Main (usually the 'only') exception, which is the subject of this exception 'report'
		/// Setting this property will clear any previously set exceptions
		/// <remarks>
		/// If multiple top-level exceptions are required, use <see cref="SetExceptions(IEnumerable{Exception})"/> instead
		/// </remarks>
		/// </summary>
		public Exception MainException
		{
			get { return _exceptions.Count > 0 ? 
				_exceptions[0] : 
				// while we generally don't want to allow our own exceptions, I'll make an exception here - it would be too silly
				// for a user to set a null exception
				new ConfigurationErrorsException("ExceptionReporter given 0 exceptions"); }
			set
			{
				_exceptions.Clear();
				_exceptions.Add(value);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Exception[] Exceptions
		{
			get { return _exceptions.ToArray(); }
		}

		/// <summary>
		/// Add multiple exceptions
		/// <remarks>
		/// Note: Showing multiple exceptions is a special-case requirement
		/// To set only 1 top-level exception use <see cref="MainException"/> property instead
		/// </remarks>
		/// </summary>
		public void SetExceptions(IEnumerable<Exception> exceptions)
		{
			_exceptions.Clear();
			_exceptions.AddRange(exceptions);
		}
		
		/// <summary>
		/// The calling assembly of the running application
		/// If not set, will default to
		/// <see cref="Assembly.GetEntryAssembly()"/> ??
		/// <see cref="Assembly.GetCallingAssembly()"/>
		/// </summary>
		public Assembly AppAssembly { get; set; }
		
		/// <summary>
		/// Date/time of the exception being raised - will be set automatically by <see cref="ReportGenerator"/>
		/// depending on <see cref="ReportConfig.ExceptionDateKind"/>
		/// </summary>
		public DateTime ExceptionDate { get; set; }

	}
}