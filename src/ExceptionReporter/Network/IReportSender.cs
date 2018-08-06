namespace ExceptionReporting.Network
{
	internal interface IReportSender
	{
		/// <summary>
		/// Send the report using implementation destination
		/// </summary>
		void Send(string report);
		
		/// <summary>
		/// One-word description of the sender type
		/// </summary>
		string Description { get; }
	}
}