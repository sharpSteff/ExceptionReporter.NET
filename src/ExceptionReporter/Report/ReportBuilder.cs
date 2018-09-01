using ExceptionReporting.Core;
using ExceptionReporting.SystemInfo;
using ExceptionReporting.Templates;

namespace ExceptionReporting.Report
{
	internal class ReportBuilder
	{
		private readonly IAssemblyDigger _assemblyDigger;
		private readonly IStackTraceMaker _stackTraceMaker;
		private readonly ISysInfoResultMapper _sysInfoMapper;

		public ReportBuilder(
			IAssemblyDigger assemblyDigger, 
			IStackTraceMaker stackTraceMaker, 
			ISysInfoResultMapper sysInfoMapper)
		{
			
			_assemblyDigger = assemblyDigger;
			_stackTraceMaker = stackTraceMaker;
			_sysInfoMapper = sysInfoMapper;
		}

		public string Report(ReportBag bag)
		{
			var renderer = new TemplateRenderer(this.ReportModel(bag));
			return bag.Config.ReportCustomTemplate.IsEmpty()
				? renderer.RenderPreset(bag.Config.ReportTemplateFormat)
				: renderer.RenderCustom(bag.Config.ReportCustomTemplate);
		}
		
		public ReportModel ReportModel(ReportBag bag)
		{
			return new ReportModel
			{
				App = new App
				{
					Title =   bag.Config.TitleText,
					Name =    bag.Config.AppName,
					Version = bag.Config.AppVersion,
					Region =  bag.Config.RegionInfo,
					User =    bag.Config.UserName,
					AssemblyRefs = _assemblyDigger.GetAssemblyRefs()
				},
				SystemInfo = _sysInfoMapper.SysInfoString(),
				Error = new Error
				{
					Exception = bag.Error.MainException,
					Explanation = bag.Config.UserExplanation,
					When = bag.Error.ExceptionDate,
					FullStackTrace = _stackTraceMaker.FullStackTrace()
				}
			};
		}
	}
}
