using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simocracy.CLSim.Simulation
{

	[DebuggerDisplay("Team={Name}, Strength={Strength}")]
	public class FootballTeam
	{
		/// <summary>
		/// Team name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Team strength
		/// </summary>
		public int Strength { get; set; }

		public override string ToString()
		{
			return $"Team={Name}, Strength={Strength}";
		}
	}
}
