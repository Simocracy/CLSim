﻿using SimpleLogger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simocracy.CLSim.Simulation
{
	/// <summary>
	/// Football group simulation
	/// </summary>
	[DebuggerDisplay("TeamCount={" + nameof(TeamCount) + "}")]
	public class FootballLeague : INotifyPropertyChanged
	{
		#region Members

		private ObservableCollection<FootballTeam> _Teams;
		private ObservableCollection<FootballMatch> _Matches;
		private DataTable _Table;

		#endregion

		#region Properties

		/// <summary>
		/// Group ID
		/// </summary>
		public string ID { get; set; }

		/// <summary>
		/// Teams of groupt
		/// </summary>
		public ObservableCollection<FootballTeam> Teams
		{
			get => _Teams;
			set { _Teams = value; Notify(); }
		}

		/// <summary>
		/// Group Matches
		/// </summary>
		public ObservableCollection<FootballMatch> Matches
		{
			get => _Matches;
			set { _Matches = value; Notify(); }
		}

		/// <summary>
		/// Team count
		/// </summary>
		public int TeamCount => Teams.Count;

		/// <summary>
		/// Group table
		/// </summary>
		public DataTable Table
		{
			get => _Table;
			set { _Table = value; Notify(); }
		}

		#endregion

		#region Simulation

		/// <summary>
		/// Creates matches
		/// </summary>
		public void CreateMatches()
		{
			Matches.Clear();

			foreach (var teamA in Teams)
			{
				foreach (var teamB in Teams)
				{
					if (teamA != teamB)
						Matches.Add(new FootballMatch() {TeamA = teamA, TeamB = teamB});
				}
			}

			SimpleLog.Log($"Matches Created in Football League ID={ID}");
		}

		/// <summary>
		/// Simulate matches
		/// </summary>
		public void Simulate()
		{
			SimpleLog.Log($"Simulate Matches in Football League ID={ID}");
			foreach(var match in Matches)
				match.Simulate();
		}

		/// <summary>
		/// Simulate matches async
		/// </summary>
		public async Task SimulateAsync()
		{
			await Task.Run(() => Simulate());
		}

		/// <summary>
		/// Calculates table async
		/// </summary>
		public async Task CalculateTableAsync()
		{
			await Task.Run(() => CalculateTable());
		}

		/// <summary>
		/// Calculates Table
		/// </summary>
		public void CalculateTable()
		{
			SimpleLog.Log($"Calculate Table in Football League ID={ID}");
			CreateTable();

			foreach(var team in Teams)
			{
				var row = Table.NewRow();

				int drawn, lose, goalsFor, goalsAgainst;
				var win = drawn = lose = goalsFor = goalsAgainst = 0;

				foreach(var match in Matches)
				{
					if(match.TeamA == team)
					{
						if(match.ResultA > match.ResultB)
							win++;
						else if(match.ResultA == match.ResultB)
							drawn++;
						else
							lose++;

						goalsFor += match.ResultA;
						goalsAgainst += match.ResultB;
					}
					else if(match.TeamB == team)
					{
						if(match.ResultB > match.ResultA)
							win++;
						else if(match.ResultB == match.ResultA)
							drawn++;
						else
							lose++;

						goalsFor += match.ResultB;
						goalsAgainst += match.ResultA;
					}
				}

				row["Team"] = team;
				row["Matches"] = win + drawn + lose;
				row["Win"] = win;
				row["Drawn"] = drawn;
				row["Lose"] = lose;
				row["GoalsFor"] = goalsFor;
				row["GoalsAgainst"] = goalsAgainst;
				row["GoalDiff"] = goalsFor - goalsAgainst;
				row["Points"] = win * 3 + drawn;

				Table.Rows.Add(row);
			}

			DataView dv = Table.DefaultView;
			dv.Sort = "Points DESC, GoalDiff DESC, GoalsFor DESC";
			Table = dv.ToTable();
		}

		/// <summary>
		/// Creates table
		/// </summary>
		private void CreateTable()
		{
			var table = new DataTable();

			table.Columns.Add("Team", typeof(FootballTeam));
			table.Columns.Add("Matches", typeof(int));
			table.Columns.Add("Win", typeof(int));
			table.Columns.Add("Drawn", typeof(int));
			table.Columns.Add("Lose", typeof(int));
			table.Columns.Add("GoalsFor", typeof(int));
			table.Columns.Add("GoalsAgainst", typeof(int));
			table.Columns.Add("GoalDiff", typeof(int));
			table.Columns.Add("Points", typeof(int));

			Table = table;
		}

		/// <summary>
		/// Gibt einen <see cref="String"/> zurück, der das Objekt darstellt.
		/// </summary>
		/// <returns>Objekt als String</returns>
		public override string ToString()
		{
			return $"ID={ID}, TeamCount={TeamCount}";
		}

		#endregion

		#region INotifyPropertyChanged

		/// <summary>
		/// Observer-Event
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Observer
		/// </summary>
		/// <param name="propertyName">Property</param>
		protected void Notify([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}
