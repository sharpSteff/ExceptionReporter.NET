using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using ExceptionReporting.Report;
using HandlebarsDotNet;

// ReSharper disable UnusedMember.Global
#pragma warning disable 1591

namespace ExceptionReporting.Templates
{
	public class TemplateRenderer
	{
		private readonly ReportModel _model;

		public TemplateRenderer(ReportModel model)
		{
			_model = model;
		}

		public string Render(TemplateFormat format = TemplateFormat.Text)
		{
			var template = GetTemplate(format);
			var compiledTemplate = Handlebars.Compile(template);
			var report = compiledTemplate(_model);
			return report;
		}

		private string GetTemplate(TemplateFormat format)
		{
			var resource = string.Format("{0}.ReportTemplate.{1}", this.GetType().Namespace, format.ToString().ToLower());
			var assembly = Assembly.GetExecutingAssembly();

			using (var stream = assembly.GetManifestResourceStream(resource))
			{
				using (var reader = new StreamReader(stream, Encoding.UTF8))
				{
					var template = reader.ReadToEnd();
					return template;
				}
			}
		}
	}
}