using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShowableData;
using ModifieurFermette.Models;
using Projet_AFFICHEURFERMETTE.MDF.Classes;
using Projet_AFFICHEURFERMETTE.MDF.Gestion;
using System.Windows.Input;

namespace ModifieurFermette.ViewModels.Dialogs
{
    class ManagePlatsDialogViewModel : ObservableData
    {
        private ObservableCollection<ExtendedPlat> _Plats;
        private ExtendedPlat _SelectedPlat;
        private ICommand _DeletePlatCmd;
        private readonly string sChConn;

        public ManagePlatsDialogViewModel(string sChConn)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait; // Curseur d'attente
            this.sChConn = sChConn;
            List<C_Plat> TmpPlats = new G_Plat(sChConn).Lire("");
            List<C_Menu> TmpMenus = new G_Menu(sChConn).Lire("");
            Plats = new ObservableCollection<ExtendedPlat>();
            foreach(C_Plat plat in TmpPlats)
            {
                // Ajoute les plats à l'observableCollection en précisant si ce plat est utilisé ou non dans un menu
                Plats.Add(new ExtendedPlat(plat, TmpMenus.Any(menu => menu.IDpotage == plat.ID || menu.IDplat == plat.ID || menu.IDdessert == plat.ID)));
            }
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;

            DeletePlatCmd = new RelayCommand(Exec => ExecuteDeletePlat(), CanExec => CanExecDeletePlat());
        }

        private bool CanExecDeletePlat()
        {
            return SelectedPlat != null && !SelectedPlat.IsUsed;
        }

        private void ExecuteDeletePlat()
        {
            new G_Plat(sChConn).Supprimer(SelectedPlat.ID);
            Plats.Remove(SelectedPlat);
            SelectedPlat = null;
        }

        public ObservableCollection<ExtendedPlat> Plats
        {
            get { return _Plats; }
            set
            {
                if (this._Plats != value)
                {
                    this._Plats = value;
                    OnPropertyChanged();
                }
            }
        }
        public ExtendedPlat SelectedPlat
        {
            get { return _SelectedPlat; }
            set
            {
                if (_SelectedPlat != value)
                {
                    _SelectedPlat = value;
                    OnPropertyChanged();
                }
            }
        }
        public ICommand DeletePlatCmd
        {
            get { return _DeletePlatCmd; }
            set { _DeletePlatCmd = value; }
        }
    }
}
