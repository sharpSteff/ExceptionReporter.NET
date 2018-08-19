/*
 * https://github.com/PandaWood/ExceptionReporter.NET
 */

using System;
using System.Collections.Generic;
//using System.Deployment.Application;
using System.Reflection;
using System.Windows.Forms;
using ExceptionReporting.Report;
using ExceptionReporting.SystemInfo;

// ReSharper disable MemberCanBePrivate.Global

#pragma warning disable 1591

namespace ExceptionReporting
{
	/// <summary>
	/// ReportGenerator is the entry point to use 'ExceptionReporter.NET' to retrieve the report/data only.
	/// ie if the user only requires the report info but has no need to use the show or send functionality available
	/// </summary>
	public class ReportGenerator : Disposable
	{
		private readonly ExceptionReportInfo _info;
		private readonly List<SysInfoResult> _sysInfoResults = new List<SysInfoResult>();

		/// <summary>
		/// Initialises some ExceptionReportInfo properties related to the application/system
		/// </summary>
		/// <param name="reportInfo">an ExceptionReportInfo, can be pre-populated with config
		/// however 'base' properties such as MachineName</param>
		public ReportGenerator(ExceptionReportInfo reportInfo)
		{
			// this is going to be a dev/learning mistake, so let them now fast and hard
			_info = reportInfo ?? throw new ReportGeneratorException("reportInfo cannot be null");
			
			_info.AppName = string.IsNullOrEmpty(_info.AppName) ? Application.ProductName : string.Empty;
			_info.AppVersion = string.IsNullOrEmpty(_info.AppVersion) ? GetAppVersion() : string.Empty;
			_info.ExceptionDate = _info.ExceptionDateKind != DateTimeKind.Local ? DateTime.UtcNow : DateTime.Now;
			
			if (_info.AppAssembly == null)
				_info.AppAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
		}

//		private string GetAppVersion()
//		{
//			return ApplicationDeployment.IsNetworkDeployed ? 
//				ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString() : Application.ProductVersion;
//		}
		
//		leave commented out for mono to toggle in/out to be able to compile
		private string GetAppVersion()
		{
			return Application.ProductVersion;
		}

		/// <summary>
		/// Generate the exception report
		/// </summary>
		/// <returns><see cref="ReportModel"/>object</returns>
		public ReportModel Generate()
		{
			var sysInfoResults = GetOrFetchSysInfoResults();
			
			var builder = new ReportBuilder(_info,  
				new AssemblyDigger(_info.AppAssembly), 
				new StackTraceMaker(_info.Exceptions),
				new SysInfoResultMapper(sysInfoResults));
			
			return builder.Build();
		}

		internal IEnumerable<SysInfoResult> GetOrFetchSysInfoResults()
		{
			if (ExceptionReporter.IsRunningMono()) return new List<SysInfoResult>();
			if (_sysInfoResults.Count == 0)
				_sysInfoResults.AddRange(CreateSysInfoResults());

			return _sysInfoResults.AsReadOnly();
		}

		private static IEnumerable<SysInfoResult> CreateSysInfoResults()
		{
			var retriever = new SysInfoRetriever();
			var results = new List<SysInfoResult>
			{
			retriever.Retrieve(SysInfoQueries.OperatingSystem).Filter(
				new[]
				{
					"CodeSet", "CurrentTimeZone", "FreePhysicalMemory",
					"OSArchitecture", "OSLanguage", "Version"
				}),
			retriever.Retrieve(SysInfoQueries.Machine).Filter(
				new[]
				{
					"TotalPhysicalMemory", "Manufacturer", "Model"
				}),
			};
			return results;
		}
	}

	/// <summary>
	/// Exception report generator exception.
	/// </summary>
	public class ReportGeneratorException : Exception
	{
		public ReportGeneratorException(string message) : base(message)
		{ }
	}
}