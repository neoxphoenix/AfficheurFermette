using ModifieurFermette.ViewModels.Dialogs;
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
    /// Interaction logic for ManageTitreLieuDialog.xaml
    /// </summary>
    public partial class ManageTitreLieuDialog : UserControl
    {
        ManageTitreLieuDialogViewModel vm;
        public ManageTitreLieuDialog(string sChConn)
        {
            InitializeComponent();
            this.vm = new ManageTitreLieuDialogViewModel(sChConn);
            this.DataContext = vm;
        }
    }
}
