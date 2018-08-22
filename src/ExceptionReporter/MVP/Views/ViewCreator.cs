using ExceptionReporting.MVP.Views;

namespace ExceptionReporting.MVP.Views
{
	internal class ViewCreator : IViewCreator
	{
		private readonly ExceptionReportInfo _reportInfo;

		public ViewCreator(ExceptionReportInfo reportInfo)
		{
			_reportInfo = reportInfo;
		}

		public IExceptionReportView Create()
		{
			return new ExceptionReportView(_reportInfo);
		}
	}
}

/// <summary>
/// 
/// </summary>
public interface IViewCreator
{
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	IExceptionReportView Create();
}