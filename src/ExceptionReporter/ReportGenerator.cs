/*
 * https://github.com/PandaWood/ExceptionReporter.NET
 */

using System;
using System.Collections.Generic;
//using System.Deployment.Application;
using System.Reflection;
using System.Windows.Forms;
using ExceptionReporting.Core;
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
	internal class ReportGenerator
	{
		private readonly ReportConfig _config;
		private readonly ErrorData _error;
		private readonly List<SysInfoResult> _sysInfoResults = new List<SysInfoResult>();

		/// <summary>
		/// Initialises some ExceptionReportInfo properties related to the application/system
		/// </summary>
		/// <param name="config">an ExceptionReportInfo, can be pre-populated with config</param>
		/// <param name="error">an ExceptionReportInfo, can be pre-populated with config however 'base'
		/// properties such as MachineName</param>
		public ReportGenerator(ReportConfig config, ErrorData error)
		{
			// this is going to be a dev/learning mistake - fail fast and hard
			_config = config ?? throw new ArgumentNullException(nameof(config));

			_error = error;
			_error.ExceptionDate = _config.ExceptionDateKind != DateTimeKind.Local ? DateTime.UtcNow : DateTime.Now;
			
			_config.AppName =    _config.AppName.IsEmpty() ? Application.ProductName : _config.AppName;
			_config.AppVersion = _config.AppVersion.IsEmpty() ? GetAppVersion() : _config.AppVersion;
			
			if (_error.AppAssembly == null)
				_error.AppAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
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
		/// <remarks>
		/// Generate doesn't do a lot beside feed the builder - this is just to keep the builder free of
		/// too many concrete (system-reliant) dependencies
		/// </remarks>
		/// <returns><see cref="ReportModel"/>object</returns>
		public string Generate()
		{
			var sysInfoResults = GetOrFetchSysInfoResults();
			
			var build = new ReportBuilder(_config, _error,
				new AssemblyDigger(_error.AppAssembly), 
				new StackTraceMaker(_error.Exceptions),
				new SysInfoResultMapper(sysInfoResults));
			
			return build.Report();
		}

		/// <summary>
		/// get system information and memoize
		/// </summary>
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
						"CodeSet",
						"CurrentTimeZone",
						"FreePhysicalMemory",
						"OSArchitecture",
						"OSLanguage",
						"Version"
					}),
				retriever.Retrieve(SysInfoQueries.Machine).Filter(
					new[]
					{
						"TotalPhysicalMemory",
						"Manufacturer",
						"Model"
					})
			};
			return results;
		}
	}
}