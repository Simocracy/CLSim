using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Simocracy.CLSim.Football.Base;
using System.Data;
using SimpleLogger;

namespace Simocracy.CLSim.GUI
{
    /// <summary>
    /// Interaktionslogik für ClGroup.xaml
    /// </summary>
    public partial class ClGroup : UserControl
    {
        public ClGroup()
        {
            InitializeComponent();
        }

        private async void SimulateGroupButton_Click(object sender, RoutedEventArgs e)
        {
            var g = (sender as Button)?.DataContext as FootballLeague;
            if (g == null)
            {
                SimpleLog.Error("Error in DataContext while simulating group.");
                return;
            }
            await g.SimulateAsync();
        }

        private async void CalculateTableButton_Click(object sender, RoutedEventArgs e)
        {
            var g = (sender as Button)?.DataContext as FootballLeague;
            if (g == null)
            {
                SimpleLog.Error($"Error in DataContext while calculating group stage table.");
                return;
            }
            await g.CalculateTableAsync();
            TableDataGrid.ItemsSource = g.Table.AsDataView();
        }

        private void TableDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void TableDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            TableDataGrid.Columns.Clear();
            TableDataGrid.Columns.Add(new DataGridTextColumn { Header = "Team", Binding = new Binding(LeagueTable.TeamRow) });
            TableDataGrid.Columns.Add(new DataGridTextColumn { Header = "Sp.", Binding = new Binding(LeagueTable.MatchCountRow) });
            TableDataGrid.Columns.Add(new DataGridTextColumn { Header = "S", Binding = new Binding(LeagueTable.WinCountRow) });
            TableDataGrid.Columns.Add(new DataGridTextColumn { Header = "U", Binding = new Binding(LeagueTable.DrawnCountRow) });
            TableDataGrid.Columns.Add(new DataGridTextColumn { Header = "N", Binding = new Binding(LeagueTable.LoseCountRow) });
            TableDataGrid.Columns.Add(new DataGridTextColumn { Header = "Tore", Binding = new Binding(LeagueTable.GoalsRow) });
            TableDataGrid.Columns.Add(new DataGridTextColumn { Header = "TD", Binding = new Binding(LeagueTable.GoalDiffRow) });
            TableDataGrid.Columns.Add(new DataGridTextColumn { Header = "Pkt.", Binding = new Binding(LeagueTable.PointsRow) });
        }
    }
}
