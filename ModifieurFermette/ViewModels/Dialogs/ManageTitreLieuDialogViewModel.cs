using Projet_AFFICHEURFERMETTE.MDF.Classes;
using Projet_AFFICHEURFERMETTE.MDF.Gestion;
using ShowableData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModifieurFermette.ViewModels.Dialogs
{

    class ManageTitreLieuDialogViewModel : ObservableData
    {
        private ObservableCollection<C_TitreEvenement> _Titres;
        private C_TitreEvenement _SelectedTitre;
        private ICommand _DeleteTitreCmd;

        private ObservableCollection<C_LieuEvenement> _Lieus;
        private C_LieuEvenement _SelectedLieu;
        private ICommand _DeleteLieuCmd;

        private readonly string sChConn;

        public ManageTitreLieuDialogViewModel(string sChConn)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait; // Curseur d'attente

            this.sChConn = sChConn;
            Titres = new ObservableCollection<C_TitreEvenement>(new G_TitreEvenement(sChConn).Lire(""));
            Lieus = new ObservableCollection<C_LieuEvenement>(new G_LieuEvenement(sChConn).Lire(""));
            // On retire les titres et lieus utilisés (oui, c'est assez lourd comme méthode, mais modifier la DB aurait pris trop de temps)
            List<C_Evenement> TmpEvenements = new G_Evenement(sChConn).Lire("");
            foreach (C_Evenement evenement in TmpEvenements)
            {
                Titres.Remove(Titres.FirstOrDefault(t => t.ID == evenement.IDtitre));
                Lieus.Remove(Lieus.FirstOrDefault(l => l.ID == evenement.IDlieu));
            }

            DeleteTitreCmd = new RelayCommand(Exec => ExecuteDeleteTitre(), CanExec => CanExecDeleteTitre());
            DeleteLieuCmd = new RelayCommand(Exec => ExecuteDeleteLieu(), CanExec => CanExecDeleteLieu());

            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
        }

        private bool CanExecDeleteTitre()
        {
            return SelectedTitre != null;
        }

        private void ExecuteDeleteTitre()
        {
            // MàJ BD
            new G_TitreEvenement(sChConn).Supprimer(SelectedTitre.ID);

            // MàJ locale
            Titres.Remove(SelectedTitre);
            SelectedTitre = null;
        }

        private bool CanExecDeleteLieu()
        {
            return SelectedLieu != null;
        }

        private void ExecuteDeleteLieu()
        {
            // MàJ DB
            new G_LieuEvenement(sChConn).Supprimer(SelectedLieu.ID);

            // MàJ locale
            Lieus.Remove(SelectedLieu);
            SelectedLieu = null;
        }

        #region Accesseurs
        public ObservableCollection<C_TitreEvenement> Titres
        {
            get => _Titres;
            set
            {
                if (_Titres != value)
                {
                    _Titres = value;
                    OnPropertyChanged();
                }
            }
        }
        public C_TitreEvenement SelectedTitre
        {
            get => _SelectedTitre;
            set
            {
                if (_SelectedTitre != value)
                {
                    _SelectedTitre = value;
                    OnPropertyChanged();
                }
            }
        }
        public ICommand DeleteTitreCmd { get => _DeleteTitreCmd; set => _DeleteTitreCmd = value; }

        public ObservableCollection<C_LieuEvenement> Lieus
        {
            get => _Lieus;
            set
            {
                if (_Lieus != value)
                {
                    _Lieus = value;
                    OnPropertyChanged();
                }
            }
        }
        public C_LieuEvenement SelectedLieu
        {
            get => _SelectedLieu;
            set
            {
                if (_SelectedLieu != value)
                {
                    _SelectedLieu = value;
                    OnPropertyChanged();
                }
            }
        }
        public ICommand DeleteLieuCmd { get => _DeleteLieuCmd; set => _DeleteLieuCmd = value; }
        #endregion
    }
}
