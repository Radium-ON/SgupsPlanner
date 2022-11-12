using FirstFloor.ModernUI.Windows.Controls;
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
using Prism.Services.Dialogs;

namespace SgupsPlanner.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for PrismModernDialog.xaml
    /// </summary>
    public partial class PrismModernDialog : ModernDialog, IDialogWindow
    {
        public PrismModernDialog()
        {
            InitializeComponent();

            // define the dialog buttons
            //this.Buttons = new Button[] { this.OkButton, this.CancelButton };
            this.CloseButton.Visibility = Visibility.Collapsed;
        }

        #region Implementation of IDialogWindow

        public IDialogResult Result { get; set; }

        #endregion
    }
}
