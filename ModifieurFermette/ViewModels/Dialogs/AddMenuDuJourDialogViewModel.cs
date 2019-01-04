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
        private ObservableCollection<C_Plat> _Potages;
        private ObservableCollection<C_Plat> _Plats;
        private ObservableCollection<C_Plat> _Desserts;

        private C_Plat _SelectedPotage;
        private C_Plat _SelectedPlat;
        private C_Plat _SelectedDessert;
        private DateTime _Date;
        private DateTime _Time;

        public AddMenuDuJourDialogViewModel(string sChConn)
        {
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
            Date = DateTime.Today;
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
                }
            }
        }
    }
}
