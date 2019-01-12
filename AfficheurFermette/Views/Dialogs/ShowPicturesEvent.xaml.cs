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
using ShowableData;
using System.Collections.ObjectModel; //pour ObservableData
using AfficheurFermette.ViewModels.Dialogs;

namespace AfficheurFermette.Views.Dialogs
{
    public partial class ShowPicturesEvent : UserControl
    {
        ShowPicturesEventViewModel vm;

        public ShowPicturesEvent(string sChConn, ShowViewEvenement EventSelected)
        {
            InitializeComponent();
            vm = new ShowPicturesEventViewModel(sChConn, EventSelected);
            this.DataContext = vm;
        }
    }
}