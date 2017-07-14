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
            var g = DataContext as FootballLeague;
            await g.SimulateAsync();
        }

        private async void CalculateTableButton_Click(object sender, RoutedEventArgs e)
        {
            var g = DataContext as FootballLeague;
            await g.CalculateTableAsync();
            TableDataGrid.ItemsSource = g.Table.AsDataView();
        }
    }
}
