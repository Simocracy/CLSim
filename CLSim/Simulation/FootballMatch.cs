using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleLogger;

namespace Simocracy.CLSim.Simulation
{

	/// <summary>
	/// Football Match
	/// </summary>
	/// <remarks>By Laserdisc, adapted from Simocracy Sport Simulator</remarks>
	[DebuggerDisplay("Match: {TeamA.Name} vs {TeamB.Name}")]
	public class FootballMatch
	{
		#region Members

		private int _Ball;
		private int _Minutes;
		private int _Start;

		private const int TORWART_A = 10;
		private const int TORWART_B = 20;
		private const int ABWEHR_A = 11;
		private const int ABWEHR_B = 21;
		private const int MITTELFELD_A = 12;
		private const int MITTELFELD_B = 22;
		private const int STURM_A = 13;
		private const int STURM_B = 23;
		private const int TOR_A = 14;
		private const int TOR_B = 24;

		#endregion

		#region Properties

		/// <summary>
		/// Team A (Home)
		/// </summary>
		public FootballTeam TeamA { get; set; }

		/// <summary>
		/// Team B (Away)
		/// </summary>
		public FootballTeam TeamB { get; set; }

		/// <summary>
		/// Result Team A
		/// </summary>
		public int ResultA { get; set; }

		/// <summary>
		/// Result Team B
		/// </summary>
		public int ResultB { get; set; }

		/// <summary>
		/// Match date
		/// </summary>
		public DateTime Date { get; set; }

		/// <summary>
		/// Match name
		/// </summary>
		public string Name { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Swaps the teams
		/// </summary>
		public void SwapTeams()
		{
			var oldTeamA = TeamA;
			var oldResultA = ResultA;
			TeamA = TeamB;
			ResultA = ResultB;
			TeamB = oldTeamA;
			ResultB = oldResultA;
			Name = $"{TeamA}-{TeamB}";
		}

		public override string ToString()
		{
			return
				$"TeamA={TeamA}, TeamB={TeamB}, ResultA={ResultA}, ResultB={ResultB}, Date={Date.ToShortDateString()}";
		}

		#endregion

		#region Simulation

		/// <summary>
		/// Simulates the Match
		/// </summary>
		public void Simulate()
		{
			SimpleLog.Info($"Simulate Match: TeamA={TeamA}, TeamB={TeamB}");

			int resA = 0;
			int resB = 0;

			Reset(90);

			_Ball = Kickoff();
			for(int i = 1; i <= _Minutes; i++)
			{
				if(i == 45)
				{
					if(_Ball == TOR_A)
						resA++;
					else if(_Ball == TOR_B)
						resB++;
					_Ball = _Start;
				}

				switch(_Ball)
				{
					case TORWART_A:
						_Ball = Turn(TeamA.Strength, TeamB.Strength);
						break;
					case ABWEHR_A:
						_Ball = Turn(TeamA.Strength, TeamB.Strength);
						break;
					case MITTELFELD_A:
						_Ball = Turn(TeamA.Strength, TeamB.Strength);
						break;
					case STURM_A:
						_Ball = Turn(TeamA.Strength, TeamB.Strength + TeamB.Strength / 2);
						break;
					case TOR_A:
						resA++;
						_Ball = MITTELFELD_B;
						break;
					case TORWART_B:
						_Ball = Turn(TeamB.Strength, TeamA.Strength);
						break;
					case ABWEHR_B:
						_Ball = Turn(TeamB.Strength, TeamA.Strength);
						break;
					case MITTELFELD_B:
						_Ball = Turn(TeamB.Strength, TeamA.Strength);
						break;
					case STURM_B:
						_Ball = Turn(TeamB.Strength, TeamA.Strength + TeamA.Strength / 2);
						break;
					case TOR_B:
						resB++;
						_Ball = MITTELFELD_A;
						break;
				}
			}

			ResultA = resA;
			ResultB = resB;
		}

		/// <summary>
		/// Resets the simulation
		/// </summary>
		private void Reset(int zeit = 0)
		{
			ResultA = 0;
			ResultB = 0;
			_Ball = 0;
			_Minutes = zeit;
			_Start = 0;
		}

		private int Turn(int strength1, int strength2)
		{
			int random = Globals.Random.Next(strength1 + strength2);
			if(random < strength1)
				return ++_Ball;
			else
				switch(_Ball)
				{
					case TORWART_A:
						return STURM_B;
					case ABWEHR_A:
						return MITTELFELD_B;
					case MITTELFELD_A:
						return ABWEHR_B;
					case STURM_A:
						return TORWART_B;
					case TORWART_B:
						return STURM_A;
					case ABWEHR_B:
						return MITTELFELD_A;
					case MITTELFELD_B:
						return ABWEHR_A;
					case STURM_B:
						return TORWART_A;
				}

			return 0;
		}

		private int Kickoff()
		{
			int random = Globals.Random.Next(2);
			if(random == 0)
			{ _Start = MITTELFELD_B; return MITTELFELD_A; }
			if(random == 1)
			{ _Start = MITTELFELD_A; return MITTELFELD_B; }
			else
				return 0;
		}

		#endregion

	}
}
