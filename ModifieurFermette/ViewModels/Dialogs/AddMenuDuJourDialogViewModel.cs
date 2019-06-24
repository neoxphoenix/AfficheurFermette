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
    public class AddMenuDuJourDialogViewModel : ObservableData
    {
        /* ===== Affichage ===== */
        private ObservableCollection<C_Plat> _Potages;
        private ObservableCollection<C_Plat> _Plats;
        private ObservableCollection<C_Plat> _Desserts;

        /* ===== Données entrées par l'user ===== */
        private C_Plat _SelectedPotage;
        private C_Plat _SelectedPlat;
        private C_Plat _SelectedDessert;
        private DateTime _Date;
        private DateTime _Time;

        public int IDMenuDuJour; // uniquement utilisé en cas de modification d'un événement existant

        /* ===== Validation ===== */
        private bool _Validated;

        public AddMenuDuJourDialogViewModel(string sChConn)
        {
            LoadData(sChConn);
            Date = DateTime.Today;
        }
        public AddMenuDuJourDialogViewModel(string sChConn, ShowViewMenuDuJour menu)
        {
            LoadData(sChConn);

            // Pré-remplissage du dialog avec les données du menu à modifier
            IDMenuDuJour = menu.ID;
            SelectedPotage = Potages.First(p => p.nom == menu.eNom);
            SelectedPlat = Plats.First(p => p.nom == menu.pNom);
            SelectedDessert = Desserts.First(d => d.nom == menu.dNom);
            Date = DateTime.Parse(menu.Date);
            
        }

        private void LoadData(string sChConn)
        {
            // Chargement des données de la DB
            Potages = new ObservableCollection<C_Plat>();
            Plats = new ObservableCollection<C_Plat>();
            Desserts = new ObservableCollection<C_Plat>();
            List<C_Plat> TmpPlats = new G_Plat(sChConn).Lire("");
            foreach (C_Plat plat in TmpPlats)
            {
                switch (plat.Type)
                {
                    case 1: // C'est un potage
                        Potages.Add(plat);
                        break;
                    case 2: // C'est un plat
                        Plats.Add(plat);
                        break;
                    case 3: // C'est un dessert
                        Desserts.Add(plat);
                        break;
                }
            }
        }

        private void IsAllItemsValid()
        {
            if (SelectedPotage == null || SelectedPlat == null || SelectedDessert == null || Time == null)
                Validated = false;
            else
                Validated = true;
        }

        public ObservableCollection<C_Plat> Potages
        {
            get { return _Potages; }
            set
            {
                if (this._Potages != value)
                {
                    this._Potages = value;
                    OnPropertyChanged("Potages");
                }
            }
        }
        public ObservableCollection<C_Plat> Plats
        {
            get { return _Plats; }
            set
            {
                if (this._Plats != value)
                {
                    this._Plats = value;
                    OnPropertyChanged("Plats");
                }
            }
        }
        public ObservableCollection<C_Plat> Desserts
        {
            get { return _Desserts; }
            set
            {
                if (this._Desserts != value)
                {
                    this._Desserts = value;
                    OnPropertyChanged("Desserts");
                }
            }
        }

        public C_Plat SelectedPotage
        {
            get { return _SelectedPotage; }
            set
            {
                if (this._SelectedPotage != value)
                {
                    this._SelectedPotage = value;
                    OnPropertyChanged("SelectedPotage");
                    IsAllItemsValid();
                }
            }
        }
        public C_Plat SelectedPlat
        {
            get { return _SelectedPlat; }
            set
            {
                if (this._SelectedPlat != value)
                {
                    this._SelectedPlat = value;
                    OnPropertyChanged("SelectedPlat");
                    IsAllItemsValid();
                }
            }
        }
        public C_Plat SelectedDessert
        {
            get { return _SelectedDessert; }
            set
            {
                if (this._SelectedDessert != value)
                {
                    this._SelectedDessert = value;
                    OnPropertyChanged("SelectedDessert");
                    IsAllItemsValid();
                }
            }
        }
        public DateTime Date
        {
            get { return _Date; }
            set
            {
                if (this._Date != value)
                {
                    this._Date = value;
                    OnPropertyChanged("Date");
                    IsAllItemsValid();
                }
            }
        }
        public DateTime Time
        {
            get { return _Time; }
            set
            {
                if (this._Time != value)
                {
                    this._Time = value;
                    OnPropertyChanged("Time");
                    IsAllItemsValid();
                }
            }
        }
        public bool Validated
        {
            get { return _Validated; }
            set
            {
                if (this._Validated != value)
                {
                    this._Validated = value;
                    OnPropertyChanged();
                }
            }
        }

        // Utilisés en cas d'ajout dans les combobox
        // On sait qu'un nouvel item devra être ajouté à la DB grâce à son ID de 0
        public string NewPotage
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Potages.Add(new C_Plat(0, value, 1, null));
                    SelectedPotage = Potages.Last();
                }
            }
        }
        public string NewPlat
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Plats.Add(new C_Plat(0, value, 2, null));
                    SelectedPlat = Plats.Last();
                }
            }
        }
        public string NewDessert
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Desserts.Add(new C_Plat(0, value,3, null));
                    SelectedDessert = Desserts.Last();
                }
            }
        }
    }
}
