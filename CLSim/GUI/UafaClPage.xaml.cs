using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
using Simocracy.CLSim.Football.UAFA;
using SimpleLogger;

namespace Simocracy.CLSim.GUI
{
    /// <summary>
    /// Page for <see cref="ChampionsLeague"/> simulation
    /// </summary>
    public partial class UafaClPage : Page
    {

        #region Members

        private Object _SelectedExpander;
        private int _TeamsEntered;

        #endregion

        #region Constructor

        public UafaClPage()
        {
            InitializeComponent();

            DataContext = null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The CL instance
        /// </summary>
        public ChampionsLeague Cl => DataContext as ChampionsLeague;

        /// <summary>
        /// The opened expander
        /// </summary>
        public Object SelectedExpander
        {
            get => _SelectedExpander;
            set
            {
                _SelectedExpander = value;
                Notify();
            }
        }

        /// <summary>
        /// Line count of entered teams
        /// </summary>
        public int TeamsEntered
        {
            get => _TeamsEntered;
            set
            {
                _TeamsEntered = value;
                // Workarounds for not working binding
                EnteredTeamsTextBlock.Text = _TeamsEntered.ToString();
                SaveTeamsButton.IsEnabled = (_TeamsEntered == 40);
                Notify();
            }
        }

        #endregion

        #region Event Handler

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            ExpandExculsively(sender as Expander);
        }

        private void ExpandExculsively(Expander expander)
        {
            foreach (var child in ExpanderPanel.Children)
            {
                if (child is Expander && child != expander)
                    ((Expander)child).IsExpanded = false;
            }
        }

        #region Init

        private void ClInitButton_Click(object sender, RoutedEventArgs e)
        {
            if(!String.IsNullOrWhiteSpace(SeasonTextBox.Text))
            {
                DataContext = new ChampionsLeague(SeasonTextBox.Text);
            }
        }

        private void ClResetButton_Click(object sender, RoutedEventArgs e)
        {
            DataContext = null;
        }

        #endregion

        #region Team input

        private void SaveTeamsButton_Click(object sender, RoutedEventArgs e)
        {
            if (TeamsEntered == ChampionsLeague.TournamentTeamCount)
                Cl.ReadTeamlist(TeamInputTextBox.Text);
        }

        private void TeamInputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TeamsEntered = TeamInputTextBox.Text
                .Split(new[] {Environment.NewLine, "\n", "\r"}, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        #endregion

        #region Group Stage

        private void GroupDrawingButton_Click(object sender, RoutedEventArgs e)
        {
            Cl.DrawGroups();
        }

        private async void SimulateGroupsButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(var g in Cl.Groups)
                await g.SimulateAsync();

            if (Cl.IsAllGroupsSimulated) CalculateTablesButton.IsEnabled = true;
        }

        private async void CalculateTablesButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(var g in Cl.Groups)
                await g.CalculateTableAsync();
        }

        #endregion

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
