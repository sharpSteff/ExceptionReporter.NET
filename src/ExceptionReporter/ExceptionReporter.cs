/*
 * https://github.com/PandaWood/ExceptionReporter.NET
 */

using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using ExceptionReporting.MVP.Views;
using ExceptionReporting.Network;
using ExceptionReporting.Network.Events;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace ExceptionReporting
{
	/// <summary>
	/// The entry-point (class) to invoking an ExceptionReporter dialog
	/// eg new ExceptionReporter().Show(exceptions)
	/// </summary>
	public class ExceptionReporter
	{
		private readonly ReportConfig _config;
		private readonly ErrorDetail _error;
		
		/// <summary>
		/// Contract by which to show any dialogs/view
		/// </summary>
		public IViewMaker ViewMaker { get; set; }
		
		/// <summary>
		/// Initialise the ExceptionReporter
		/// </summary>
		public ExceptionReporter()
		{
			_config = new ReportConfig();
			_error = new ErrorDetail
			{
				AppAssembly = Assembly.GetCallingAssembly()
			};
			ViewMaker = new ViewMaker(new ReportBag(_error, _config));
		}

		// One issue we have with Config property here is that we store the exception and other info on it as well
		// This prevents us from allowing code like this new ExceptionReporter { Config = new ExceptionReportInfo { A = 1 } } 
		// which I would much prefer
		// TODO eventually allow this code above  
		
		/// <summary>
		/// Public access to configuration/settings
		/// </summary>
		public ReportConfig Config
		{
			get { return _config; }
		}
		
		/// <summary>
		/// Public access to exception/error
		/// </summary>
		public ErrorDetail Error
		{
			get { return _error; }
		}

		/// <summary>
		/// Show the ExceptionReport dialog
		/// </summary>
		/// <remarks>The <see cref="ExceptionReporter"/> will analyze the <see cref="Exception"/>s and 
		/// create and show the report dialog.</remarks>
		/// <param name="exceptions">The <see cref="Exception"/>s to show.</param>
		public bool Show(params Exception[] exceptions)
		{
			// silently ignore the mistake of passing null
			if (exceptions == null || exceptions.Length >= 1 && exceptions[0] == null) return false;		
			
			bool status;

			try
			{
				_error.SetExceptions(exceptions);
				
				var view = ViewMaker.Create();
				view.ShowWindow();
				status = true;
			}
			catch (Exception ex)
			{
				status = false;
				ViewMaker.ShowError(ex.Message, "Failed trying to report an Error");
			}

			return status;
		}

		/// <summary>
		/// Show the ExceptionReport dialog with a custom message instead of the Exception's Message property
		/// </summary>
		/// <param name="customMessage">custom (exception) message</param>
		/// <param name="exceptions">The exception/s to display in the exception report</param>
		public void Show(string customMessage, params Exception[] exceptions)
		{
			_config.CustomMessage = customMessage;
			Show(exceptions);
		}

		/// <summary>
		/// Send the report, asynchronously, without showing a dialog (silent send)
		/// For this <see cref="ReportConfig.SendMethod"/> must be SMTP or WebService, else this is ignored (silently)
		/// </summary>
		/// <param name="sendEvent">Provide implementation of IReportSendEvent to receive error/updates on calling thread</param>
		/// <param name="exceptions">The exception/s to include in the report</param>
		public void Send(IReportSendEvent sendEvent = null, params Exception[] exceptions)
		{
			_error.SetExceptions(exceptions);

			var reportBag = new ReportBag(_error, _config);
			var sender = new SenderFactory(reportBag, sendEvent ?? new SilentSendEvent()).Get();
			var report = new ReportGenerator(reportBag);
			sender.Send(report.Generate());
		}

		/// <summary>
		/// Send the report, asynchronously, without showing a dialog (silent send)
		/// For this <see cref="ReportConfig.SendMethod"/> must be SMTP or WebService, else this is ignored (silently)
		/// </summary>
		/// <param name="exceptions">The exception/s to include in the report</param>
		public void Send(params Exception[] exceptions)
		{
			Send(new SilentSendEvent(), exceptions);
		}

		/// <summary>
		/// Generate report without showing or sending
		/// </summary>
		/// <returns>report as a string (in format configured <see cref="TemplateFormat"/></returns>
		public string GetReport(params Exception[] exceptions)
		{
			_error.SetExceptions(exceptions);
			var generator = new ReportGenerator(new ReportBag(_error, _config));
			return generator.Generate();
		}

		private static readonly bool _isRunningMono = Type.GetType("Mono.Runtime") != null;

		/// <returns><c>true</c>, if running mono <c>false</c> otherwise.</returns>
		public static bool IsRunningMono() { return _isRunningMono; }
		
		/// <returns><c>true</c>, if not running mono <c>false</c> otherwise.</returns>
		public static bool NotRunningMono() { return !_isRunningMono; }
	}
}

// ReSharper restore UnusedMember.Global
