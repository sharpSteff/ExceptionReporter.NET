using System.IO;
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
			var assembly = System.Reflection.Assembly.GetExecutingAssembly();
			var resource = string.Format("ExceptionReporting.Templates.ReportTemplate.{0}", format == TemplateFormat.Text ? "txt" : "html");

			using (var stream = assembly.GetManifestResourceStream(resource))
			{
				using (var reader = new StreamReader(stream))
				{
					var template = reader.ReadToEnd();
					return template;
				}
			}
		}
	}
}