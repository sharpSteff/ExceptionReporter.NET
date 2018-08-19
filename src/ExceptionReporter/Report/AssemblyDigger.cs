using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExceptionReporting.Report
{
	internal interface IAssemblyDigger
	{
		IEnumerable<AssemblyRef> GetAssemblyRefs();
	}

	/// <summary>
	/// Used to find referenced assemblies
	/// </summary>
	internal class AssemblyDigger : IAssemblyDigger
	{
		private readonly Assembly _assembly;

		///<summary>Initialise with root/main assembly</summary>
		public AssemblyDigger(Assembly assembly)
		{
			_assembly = assembly;
		}

		/// <summary> returns all referenced assemblies and returns a custom array used in <see cref="ReportModel"/></summary>
		public IEnumerable<AssemblyRef> GetAssemblyRefs()
		{
			return from a in _assembly.GetReferencedAssemblies()
				select new AssemblyRef
				{
					Name = a.Name,
					Version = a.Version.ToString()
				};
		}
	}
}