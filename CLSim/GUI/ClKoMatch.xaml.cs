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
using SimpleLogger;

namespace Simocracy.CLSim.GUI
{
    /// <summary>
    /// User control for CL KO Match
    /// </summary>
    public partial class ClKoMatch : UserControl
    {

        public ClKoMatch()
        {
            InitializeComponent();
        }

        private void IsExtraTimeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var g = (sender as CheckBox)?.DataContext as DoubleMatch;
            if (g == null)
            {
                SimpleLog.Error("Error in DataContext while enabling extra time in KO match.");
                return;
            }

            if(g.Winner == null) PenaltyTextBlock.Visibility = Visibility.Visible;
        }

        private void IsExtraTimeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }
    }
}
