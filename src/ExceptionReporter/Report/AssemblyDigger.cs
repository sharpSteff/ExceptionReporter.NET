using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExceptionReporting.Report
{
	/// <summary>
	/// Used to find and do things with referenced assemblies
	/// </summary>
	public class AssemblyDigger
	{
		private readonly Assembly _assembly;

		///<summary>Initialise with assembly</summary>
		public AssemblyDigger(Assembly assembly)
		{
			_assembly = assembly;
		}

		/// <summary> Finds all assemblies referenced and return a string </summary>
		/// <returns>line-delimited string of referenced assemblies</returns>
		public IEnumerable<AssemblyRef> AssemblyRefs()
		{
			return _assembly.GetReferencedAssemblies().Select(a => new AssemblyRef
			{
				Name = a.Name,
				Version = a.Version.ToString()
			});
		}
	}
}