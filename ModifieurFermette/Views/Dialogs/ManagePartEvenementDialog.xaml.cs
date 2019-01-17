using ModifieurFermette.ViewModels.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for ManagePartEvenementDialog.xaml
    /// </summary>
    public partial class ManagePartEvenementDialog : UserControl
    {
        ManagePartEvenementDialogViewModel vm;
        public ManagePartEvenementDialog(ShowableData.ShowViewEvenement evenement, string sChConn)
        {
            InitializeComponent();
            vm = new ManagePartEvenementDialogViewModel(evenement, sChConn);
            this.DataContext = vm;
        }

        /// <summary>
        /// Vérifie que l'utilisateur n'entre que des nombres dans la TextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[0-9]$");
            
            if (!regex.IsMatch(e.Text))
            {
                e.Handled = false;
                MessageBox.Show("Veuillez n'entrer que des nombres !");
                TextBox tb = (TextBox)sender;
                tb.Text = "";
            }
        }
    }
}
