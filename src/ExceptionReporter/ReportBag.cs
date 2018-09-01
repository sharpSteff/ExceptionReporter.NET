using System;

namespace ExceptionReporting
{
	/// <summary>
	/// combination of config and error details for convenience
	/// </summary>
	public class ReportBag
	{
		public ErrorDetail Error { get; }
		public ReportConfig Config{ get; }

		public ReportBag(ErrorDetail error, ReportConfig config)
		{
			Error = error ?? throw new ArgumentNullException(nameof(error));
			Config = config ?? throw new ArgumentNullException(nameof(config));
		}
	}
}