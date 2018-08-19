using System;
using System.Collections.Generic;
using System.Globalization;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

#pragma warning disable 1591

namespace ExceptionReporting.Templates
{
	public class TemplateModel
	{
		public App App { get; set; }
		public Error Error { get; set; }
		public string SystemInfo { get; set; }
	}

	public class App
	{
		public string Name { get; set; }
		public string Version { get; set; }
		public string Region { get; set; } = CultureInfo.CurrentCulture.DisplayName;
		public string User { get; set; }
		public IList<AssemblyRef> Assemblies{ get; set; }
	}

	public class Error
	{
		public Exception Exception { get; set; }
		
		public DateTime When { get; set; } = DateTime.Now;
		
		public string Explanation { get; set; }
		
		public string Message
		{
			get { return Exception.Message; }
		}
		
		public string Type
		{
			get { return Exception.GetType().FullName; }
		}
		
		public string StackTrace
		{
			get { return Exception.StackTrace; }
		}
		
		public string Date { get { return When.ToShortDateString(); } }
		
		public string Time { get { return When.ToShortTimeString(); } }
		
		public string InnerException
		{
			get { return Exception?.InnerException?.ToString(); }
		}
	}

	public class AssemblyRef
	{
		public string Name  { get; set; }
		public string Version { get; set; }
	}
}