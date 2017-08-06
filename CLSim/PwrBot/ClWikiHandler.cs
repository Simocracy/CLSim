using System.ComponentModel;
using DotNetWikiBot;
using Simocracy.PwrBot;

namespace Simocracy.CLSim.PwrBot
{
    /// <summary>
    /// Wiki handler class for UAFA Champions League
    /// </summary>
    public class ClWikiHandler : INotifyPropertyChanged
    {

        #region Members

        private static Site _Site;

        #endregion

        #region Constructors

        #endregion

        #region Properties

        /// <summary>
        /// The Site for the Bot
        /// </summary>
        public static Site Site => _Site ?? (_Site = new Site("https://simocracy.de", PwrBotLoginData.Username, PwrBotLoginData.Password));

        #endregion

        #region Methods



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
