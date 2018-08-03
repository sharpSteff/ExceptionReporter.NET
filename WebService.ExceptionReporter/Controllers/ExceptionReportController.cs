using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebService.ExceptionReporter.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebService.ExceptionReporter.Controllers
{
	[Route("api/er")]
	[ApiController]
	public class ExceptionReportController : Controller
	{
		private readonly ExceptionReportContext _context;
		public ExceptionReportController(ExceptionReportContext context)
		{
			_context = context;

			if (!_context.ExceptionReportItems.Any())
			{
				_context.ExceptionReportItems.Add(new ExceptionReportItem
				{
					AppName = "Sample App",
					AppVersion = "1.0.0",
					ExceptionMessage = "NullReferenceException: Object reference not set to an instance of an object.",
					ExceptionReport = "Stack Trace: at WinFormsDemoApp.DemoAppView.AndAnotherOne() in Z:\\src\\DemoAppView.cs:line 110\n..."
				});
				_context.SaveChanges();
			}
		}

		[HttpGet]
		public ActionResult<List<ExceptionReportItem>> GetAll()
		{
			return _context.ExceptionReportItems.ToList();
		}

		// GET api/<controller>/5
		[HttpGet("{id}", Name = "GetReport")]
		public ActionResult<ExceptionReportItem> GetById(long id)
		{
			var item = _context.ExceptionReportItems.Find(id);
			if (item == null)
			{
				return NotFound();
			}
			return item;
		}

		[HttpPost]
		public IActionResult Create(ExceptionReportItem item)
		{
			_context.ExceptionReportItems.Add(item);
			_context.SaveChanges();

			return CreatedAtRoute("GetReport", new { id = item.ID }, item);
		}

		//		// POST api/<controller>
		//		[HttpPost]
		//		public void Post([FromBody] string value)
		//		{
		//		}

		// PUT api/<controller>/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		// DELETE api/<controller>/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}
	}
}