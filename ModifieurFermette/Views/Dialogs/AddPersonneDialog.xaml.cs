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
    /// Interaction logic for AddPersonneDialog.xaml
    /// </summary>
    public partial class AddPersonneDialog : UserControl
    {
        public ViewModels.Dialogs.AddPersonneDialogViewModel vm;
        public AddPersonneDialog()
        {
            vm = new ViewModels.Dialogs.AddPersonneDialogViewModel();
            InitializeComponent();
            this.DataContext = vm;
        }
        public AddPersonneDialog(ShowableData.ShowPersonne personne)
        {
            InitializeComponent();
            vm = new ViewModels.Dialogs.AddPersonneDialogViewModel(personne);
            this.DataContext = vm;
        }
    }
}
