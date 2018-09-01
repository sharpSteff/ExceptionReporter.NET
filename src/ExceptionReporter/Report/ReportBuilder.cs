using ExceptionReporting.Core;
using ExceptionReporting.SystemInfo;
using ExceptionReporting.Templates;

namespace ExceptionReporting.Report
{
	internal class ReportBuilder
	{
		private readonly ReportConfig _config;
		private readonly ErrorData _error;
		private readonly IAssemblyDigger _assemblyDigger;
		private readonly IStackTraceMaker _stackTraceMaker;
		private readonly ISysInfoResultMapper _sysInfoMapper;

		public ReportBuilder(ReportConfig config, ErrorData error,
			IAssemblyDigger assemblyDigger, 
			IStackTraceMaker stackTraceMaker, 
			ISysInfoResultMapper sysInfoMapper)
		{
			_config = config;
			_error = error;
			_assemblyDigger = assemblyDigger;
			_stackTraceMaker = stackTraceMaker;
			_sysInfoMapper = sysInfoMapper;
		}

		public string Report()
		{
			var renderer = new TemplateRenderer(this.ReportModel());
			return _config.ReportCustomTemplate.IsEmpty()
				? renderer.RenderPreset(_config.ReportTemplateFormat)
				: renderer.RenderCustom(_config.ReportCustomTemplate);
		}
		
		public ReportModel ReportModel()
		{
			return new ReportModel
			{
				App = new App
				{
					Title =   _config.TitleText,
					Name =    _config.AppName,
					Version = _config.AppVersion,
					Region =  _config.RegionInfo,
					User =    _config.UserName,
					AssemblyRefs = _assemblyDigger.GetAssemblyRefs()
				},
				SystemInfo = _sysInfoMapper.SysInfoString(),
				Error = new Error
				{
					Exception = _error.MainException,
					Explanation = _config.UserExplanation,
					When = _error.ExceptionDate,
					FullStackTrace = _stackTraceMaker.FullStackTrace()
				}
			};
		}
	}
}
