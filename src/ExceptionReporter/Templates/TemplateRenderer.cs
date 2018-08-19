using System.IO;
using System.Reflection;
using System.Text;
using HandlebarsDotNet;

// ReSharper disable UnusedMember.Global
#pragma warning disable 1591

namespace ExceptionReporting.Templates
{
	public class TemplateRenderer
	{
		private readonly TemplateModel _model;

		public TemplateRenderer(TemplateModel model)
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

		private static string GetTemplate(TemplateFormat format)
		{
			var resource = string.Format("ExceptionReporting.Templates.ReportTemplate.{0}", format.ToString().ToLower());
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