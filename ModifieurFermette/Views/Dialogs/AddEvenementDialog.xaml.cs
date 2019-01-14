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
    /// Interaction logic for AddEvenementDialog.xaml
    /// </summary>
    public partial class AddEvenementDialog : UserControl
    {
        public ViewModels.Dialogs.AddEvenementDialogViewModel vm;
        public AddEvenementDialog(string sChConn) // Ajout
        {
            vm = new ViewModels.Dialogs.AddEvenementDialogViewModel(sChConn);
            InitializeComponent();
            this.DataContext = vm;
        }
        public AddEvenementDialog(string sChConn, ShowableData.ShowViewEvenement evenement) // modification
        {
            InitializeComponent();
            vm = new ViewModels.Dialogs.AddEvenementDialogViewModel(sChConn, evenement);
            this.DataContext = vm;
        }
    }
}
