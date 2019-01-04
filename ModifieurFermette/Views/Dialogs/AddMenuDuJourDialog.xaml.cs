using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ModifieurFermette.ViewModels.Dialogs;
using Projet_AFFICHEURFERMETTE.MDF.Classes;
using Projet_AFFICHEURFERMETTE.MDF.Gestion;

namespace ModifieurFermette.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for AddMenuDuJourDialog.xaml
    /// </summary>
    public partial class AddMenuDuJourDialog : UserControl
    {
        public AddMenuDuJourDialogViewModel vm;
        public AddMenuDuJourDialog(string sChConn)
        {
            vm = new AddMenuDuJourDialogViewModel(sChConn);
            InitializeComponent();
            this.DataContext = vm;
        }

        /// <summary>
        /// Evenement lié à la RoutedCommand du bouton de confirmation de notre dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (vm.SelectedPotage == null || vm.SelectedPlat == null || vm.SelectedDessert == null || vm.Time == null)
                e.CanExecute = false;
            else
                e.CanExecute = true;
        }
        /// <summary>
        /// NE fait rien après que la commande soit exécutée, mais nécessaire à la création du binding
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }
    }
}
