using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace Simocracy.CLSim.GUI
{
    /// <summary>
    /// Info Page
    /// </summary>
    public partial class InfoPage : Page
    {
        /// <summary>
        /// Creates a new Info Page
        /// </summary>
        public InfoPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Navigates to a hyperlink
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                var navigateUri = (sender as Hyperlink)?.NavigateUri.ToString();
                if(Globals.CheckValidHttpUrl(navigateUri))
                    System.Diagnostics.Process.Start(navigateUri);
            }
            catch { }
            e.Handled = true;
        }
    }
}
