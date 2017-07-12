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

namespace Simocracy.CLSim.GUI
{
    /// <summary>
    /// Main Window
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Constructor

        /// <summary>
        /// Creates a new MainWindow
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            MainFrame.Content = new UafaClPage();
        }

        #endregion

        #region Menue Handler

        private void MenuItemCredits_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new InfoPage();
        }

        private void MenuItemUafaClTournament_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new UafaClPage();
        }

        #endregion
    }
}
