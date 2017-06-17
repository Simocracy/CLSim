using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Simocracy.CLSim.Simulation;

namespace Simocracy.CLSim.Extensions
{
    public static class DataTableExtensions
    {
        /// <summary>
        /// Sorts the Table as a football table from <see cref="FootballLeague"/>
        /// </summary>
        /// <param name="table">The unsorted table</param>
        /// <returns>The sorted table</returns>
        public static DataTable SortFootballTable(this DataTable table, ObservableCollection<FootballMatch> matchList)
        {

            DataTable clone = table.Clone();
            List<DataRow> rows = new List<DataRow>();
            foreach(DataRow row in table.Rows)
            {
                rows.Add(row);
            }

            // Base sorting
            rows.Sort(new FootballLeagueBaseComparer());

            // Direct matching
            var directGroups =
                from r in rows
                group r by new
                {
                    pts = r.Field<int>(FootballLeague.PointsRow),
                    gdiff = r.Field<int>(FootballLeague.GoalDiffRow),
                    gfor = r.Field<int>(FootballLeague.GoalsForCountRow)
                }
                into rGroups
                where rGroups.Count() > 1
                select rGroups;

            foreach(var dGroup in directGroups)
            {
                var group = dGroup.ToList();

            }

            foreach(DataRow row in rows)
            {
                clone.Rows.Add(row);
            }

            return clone;
        }
    }
}
