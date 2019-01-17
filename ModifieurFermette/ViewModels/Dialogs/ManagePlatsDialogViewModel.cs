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
        private ObservableCollection<C_Plat> _Plats;
        private C_Plat _SelectedPlat;
        private ICommand _DeletePlatCmd;
        private readonly string sChConn;

        public ManagePlatsDialogViewModel(string sChConn)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait; // Curseur d'attente
            this.sChConn = sChConn;
            List<C_Plat> TmpPlats = new G_Plat(sChConn).Lire("");
            List<C_Menu> TmpMenus = new G_Menu(sChConn).Lire("");
            Plats = new ObservableCollection<C_Plat>();
            // On ajoute que les plats qui ne sont pas utilisés (oui, c'est assez lourd comme méthode, mais modifier la DB aurait pris trop de temps)
            foreach(C_Plat plat in TmpPlats)
            {
                bool IsUsed = false;
                foreach (C_Menu menu in TmpMenus)
                {
                    if (menu.IDpotage == plat.ID || menu.IDplat == plat.ID || menu.IDdessert == plat.ID)
                    {
                        IsUsed = true;
                        break;
                    }
                }
                if (!IsUsed)
                    Plats.Add(plat);
            }
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;

            DeletePlatCmd = new RelayCommand(Exec => ExecuteDeletePlat(), CanExec => CanExecDeletePlat());
        }

        private bool CanExecDeletePlat()
        {
            return SelectedPlat != null;
        }

        private void ExecuteDeletePlat()
        {
            new G_Plat(sChConn).Supprimer(SelectedPlat.ID);
            Plats.Remove(SelectedPlat);
            SelectedPlat = null;
        }

        public ObservableCollection<C_Plat> Plats
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
        public C_Plat SelectedPlat
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
