﻿using System;
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
        public AddMenuDuJourDialog(string sChConn, ShowableData.ShowViewMenuDuJour menu)
        {
            InitializeComponent();
            vm = new AddMenuDuJourDialogViewModel(sChConn, menu);
            this.DataContext = vm;
        }
    }
}
