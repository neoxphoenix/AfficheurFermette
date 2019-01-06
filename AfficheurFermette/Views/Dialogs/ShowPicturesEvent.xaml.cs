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
using AfficheurFermette.ViewModels.Dialogs;

namespace AfficheurFermette.Views.Dialogs
{
    public partial class ShowPicturesEvent : UserControl
    {
        //ShowPicturesEventViewModel vm;

        public ShowPicturesEvent() //string Header_, string Message_
        {
            InitializeComponent();
            //vm = new ConfirmDialogViewModel();
            //this.DataContext = vm;



        }
    }

    //public class ConfirmDialogViewModel : ObservableData
    //    {
    //        private string _Header, _Message;
    //        public ConfirmDialogViewModel(string Header_, string Message_)
    //        {
    //            Header = Header_;
    //            Message = Message_;
    //        }
    //        public string Message { get => _Message; set { _Message = value; OnPropertyChanged("Message"); } }
    //        public string Header { get => _Header; set { _Header = value; OnPropertyChanged("Header"); } }
    //    }
    //}
}
