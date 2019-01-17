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

namespace ModifieurFermette.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for DetailsEvenementDialog.xaml
    /// </summary>
    public partial class DetailsEvenementDialog : UserControl
    {
        ViewModels.Dialogs.DetailsEvenementDialogViewModel vm;
        public DetailsEvenementDialog(ShowableData.ShowViewEvenement evenement, string sChConn)
        {
            InitializeComponent();
            vm = new ViewModels.Dialogs.DetailsEvenementDialogViewModel(evenement, sChConn);
            this.DataContext = vm;
        }
    }
}
