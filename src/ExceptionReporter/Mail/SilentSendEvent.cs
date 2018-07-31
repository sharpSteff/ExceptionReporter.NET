using System;

namespace ExceptionReporting.Mail
{
	/// <summary>
	/// A fake/slient version of the events responding to sending
	/// </summary>
	public class SilentSendEvent : IEmailSendEvent
	{
		/// <summary>
		/// 
		/// </summary>
		public void Completed(bool success)
		{
			// do nothing
		}

		/// <summary>
		/// 
		/// </summary>
		public void ShowError(string message, Exception exception)
		{
			// do nothing
		}
	}
}