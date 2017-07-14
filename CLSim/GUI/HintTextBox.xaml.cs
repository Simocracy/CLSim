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
    /// TextBox with hints that dissapears if text inputted
    /// </summary>
    public partial class HintTextBox
    {
        public HintTextBox()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string HintText { get; set; }

        public string Text { get; set; }
    }
}
