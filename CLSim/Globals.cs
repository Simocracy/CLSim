using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Simocracy.CLSim
{
	public static class Globals
	{

		#region Global Constants

		private static readonly ThreadLocal<Random> _Random = new ThreadLocal<Random>(() => new Random());
		
		/// <summary>
		/// Returns a new random value
		/// </summary>
		public static Random Random => _Random.Value;

		/// <summary>
		/// Returns the current program version
		/// </summary>
		public static string ProgramVersion
		{
			get
			{
				var version = Assembly.GetExecutingAssembly().GetName().Version;
				var versionString = $"{version.Major}.{version.Minor}.{version.Revision}";
#if DEBUG
				versionString = $"{versionString} Debug Build";
#endif
				return versionString;
			}
		}

		/// <summary>
		/// Returns the current program name
		/// </summary>
		public static string ProgramName => ((AssemblyTitleAttribute)Assembly.GetExecutingAssembly().GetCustomAttribute(typeof(AssemblyTitleAttribute))).Title;

		#endregion


	}
}
