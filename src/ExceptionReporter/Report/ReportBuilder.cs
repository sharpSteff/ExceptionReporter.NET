using System.Collections.Generic;
using ExceptionReporting.SystemInfo;

namespace ExceptionReporting.Report
{
	internal class ReportBuilder
	{
		private readonly ExceptionReportInfo _info;
		private readonly IEnumerable<SysInfoResult> _sysInfoResults;

		public ReportBuilder(ExceptionReportInfo info, IEnumerable<SysInfoResult> sysInfoResults)
		{
			_info = info;
			_sysInfoResults = sysInfoResults;
		}
		
		public ReportModel Build()
		{
			return new ReportModel
			{
				App = new App
				{
					Name = _info.AppName,
					Version= _info.AppVersion,
					Region = _info.RegionInfo,
					User = _info.UserName,
					AssemblyRefs = new AssemblyDigger(this._info.AppAssembly).AssemblyRefs()
				},
				SystemInfo = SysInfoResultMapper.CreateStringList(_sysInfoResults),
				Error = new Error
				{
					Exception = _info.MainException,
					Explanation = _info.UserExplanation,
					When = _info.ExceptionDate,
					FullStackTrace = new StackTraceMaker(_info.Exceptions).FullStackTrace()
				}
			};
		}
	}
}
