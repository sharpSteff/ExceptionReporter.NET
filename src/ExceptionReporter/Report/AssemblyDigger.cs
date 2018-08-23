using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExceptionReporting.Templates;

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
		private static IEnumerable<AssemblyRef> _assemblyRefs;
		
		///<summary>Initialise with root/main assembly</summary>
		public AssemblyDigger(Assembly assembly)
		{
			_assembly = assembly;
		}

		/// <summary>
		/// Returns all referenced assemblies in a customized array used in <see cref="ReportModel"/>
		/// Memoized
		/// </summary>
		public IEnumerable<AssemblyRef> GetAssemblyRefs()
		{
			return _assemblyRefs ?? (_assemblyRefs =
			 from a in _assembly.GetReferencedAssemblies()
				 .Concat(new List<AssemblyName>
				 {
					 _assembly.GetName() // ensure we add the app assembly to the list
				 })
			 orderby a.Name.ToLower()
			 select new AssemblyRef
			 {
				 Name = a.Name,
				 Version = a.Version.ToString()
			 });
		}
	}
}