using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simocracy.CLSim.Simulation
{
	/// <summary>
	/// Simulates the UAFA Champions League in the mode since 2051/52
	/// </summary>
	public class ChampionsLeague : INotifyPropertyChanged
	{

		#region Members

		private ObservableCollection<FootballTeam> _AllTeamsRaw;
		private ObservableCollection<FootballTeam> _AllTeamsOrdered;

		private ObservableCollection<FootballLeague> _Groups;
		private bool _IsGroupsSimulatable;

		private const int TeamsPerGroup = 5;
		private const int GroupCount = 8;

		#endregion

		#region Constructor

		/// <summary>
		/// Erstellt eine neue Instanz für einen Amerikapokal
		/// </summary>
		public ChampionsLeague()
		{
			IsGroupsSimulatable = false;
		}

		#endregion

		#region Properties

		/// <summary>
		/// All Teams in raw order
		/// </summary>
		public ObservableCollection<FootballTeam> AllTeamsRaw
		{
			get => _AllTeamsRaw;
			set
			{
				_AllTeamsRaw = value;
				Notify();
			}
		}

		/// <summary>
		/// All Teams in ordered order
		/// </summary>
		public ObservableCollection<FootballTeam> AllTeamsOrdered
		{
			get => _AllTeamsOrdered;
			set
			{
				_AllTeamsOrdered = value;
				Notify();
			}
		}

		/// <summary>
		/// CL Groups from A to H
		/// </summary>
		public ObservableCollection<FootballLeague> Groups
		{
			get => _Groups;
			set
			{
				_Groups = value;
				Notify();
			}
		}

		/// <summary>
		/// True if draw was successfully
		/// </summary>
		public bool IsGroupsSimulatable
		{
			get => _IsGroupsSimulatable;
			private set
			{
				_IsGroupsSimulatable = value;
				Notify();
			}
		}

		#endregion

		#region Group Drawing

		/// <summary>
		/// Draw groups
		/// </summary>
		/// <param name="recursiveCount">Try-Count</param>
		public void DrawGroups(int recursiveCount = 0)
		{
			var ordered = AllTeamsRaw.OrderBy(x => Globals.Random.Next());
			AllTeamsOrdered = new ObservableCollection<FootballTeam>(ordered);

			Groups = new ObservableCollection<FootballLeague>();
			char groupID = 'A';
			for(int i = 0; i < AllTeamsRaw.Count; i += 5)
			{
				Groups.Add(new FootballLeague(groupID.ToString(), AllTeamsOrdered[i], AllTeamsOrdered[i + 1], AllTeamsOrdered[i + 2], AllTeamsRaw[i + 3], AllTeamsRaw[i + 4]));
				groupID = (char) (groupID + 1);
			}

			bool[] isNationValid = ValidateGroups();

			// Restart if validation fails
			IsGroupsSimulatable = true;
			if(isNationValid.Contains(false))
			{
				if(recursiveCount < 5)
					DrawGroups();
				else
					IsGroupsSimulatable = false;
			}
		}

		/// <summary>
		/// Validate the groups (no multiple teams from one state) and tries to switch teams between groups
		/// </summary>
		private bool[] ValidateGroups()
		{
			bool[] isNationValid = new bool[8];
			bool reValidNeeded = false;
			for(int i = 0; i < GroupCount; i++)
			{
				var group = Groups[i];
				isNationValid[i] = AreSameStatesInGroup(group);
				if(isNationValid[i])
					continue;

				// Switch teams
				for(int teamA = 0; teamA < TeamsPerGroup - 1; teamA++)
				{
					for(int teamB = teamA + 1; teamB < TeamsPerGroup; teamB++)
					{
						var res = SwitchTeamGroups(i, teamA, teamB);
						if(!reValidNeeded)
							reValidNeeded = res;
					}
				}

				if(reValidNeeded)
					isNationValid[i] = AreSameStatesInGroup(group);
			}

			return isNationValid;
		}

		/// <summary>
		/// Switches teams between groups. If <paramref name="teamA"/> und <paramref name="teamB"/> are from same state, then switch <paramref name="teamA"/> to another group.
		/// </summary>
		/// <param name="groupNo">Base group number (group A = 0)</param>
		/// <param name="teamA">Position of Team A</param>
		/// <param name="teamB">Validation position of Team B</param>
		private bool SwitchTeamGroups(int groupNo, int teamA = 0, int teamB = 1)
		{
			var group = Groups[groupNo];

			// Check
			if(group.Teams[teamA].State != group.Teams[teamB].State)
				return true;

			// Previous group
			if(groupNo > 0 && Groups[groupNo - 1].Teams[teamA].State != group.Teams[teamA].State)
			{
				var newTeam = Groups[groupNo - 1].Teams[teamA];
				Groups[groupNo - 1].Teams[teamA] = group.Teams[teamA];
				group.Teams[teamA] = newTeam;
			}
			// Next group
			else if(groupNo < 7 && Groups[groupNo + 1].Teams[teamA].State != group.Teams[teamA].State)
			{
				var newTeam = Groups[groupNo + 1].Teams[teamA];
				Groups[groupNo + 1].Teams[teamA] = group.Teams[teamA];
				group.Teams[teamA] = newTeam;
			}
			else return false;

			return true;
		}

		/// <summary>
		/// Checks if 2 teams from the same state in the same group and returns false if not
		/// </summary>
		/// <param name="group">Group</param>
		private bool AreSameStatesInGroup(FootballLeague group)
		{
			for(int i = 0; i < TeamsPerGroup - 1; i++)
			for(int j = i + 1; j < TeamsPerGroup; j++)
				if(group.Teams[i].State == group.Teams[j].State)
					return true;
			return false;
		}

		#endregion

		#region Group Simulation

		/// <summary>
		/// Simulates all Groups
		/// </summary>
		public void SimulateGroups()
		{
			if(IsGroupsSimulatable && Groups != null)
				foreach(var group in Groups)
					group.Simulate();
		}

		/// <summary>
		/// Simulates all groups async
		/// </summary>
		public async void SimulateGroupsAsync()
		{
			await Task.Run(() => SimulateGroups());
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
