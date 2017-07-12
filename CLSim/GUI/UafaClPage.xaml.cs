using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Simocracy.CLSim.Football.UAFA;

namespace Simocracy.CLSim.GUI
{
    /// <summary>
    /// Page for <see cref="ChampionsLeague"/> simulation
    /// </summary>
    public partial class UafaClPage : Page
    {

        #region Members



        #endregion

        #region Constructor

        public UafaClPage()
        {
            InitializeComponent();

            DataContext = null;
        }

        #endregion

        #region Properties

        #endregion

        #region Event Handler

        private void ClInitButton_Click(object sender, RoutedEventArgs e)
        {
            if(!String.IsNullOrWhiteSpace(SeasonTextBox.Text))
            {
                DataContext = new ChampionsLeague(SeasonTextBox.Text);
            }
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
