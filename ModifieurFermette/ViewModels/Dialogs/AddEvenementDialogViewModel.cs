using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Projet_AFFICHEURFERMETTE.MDF.Classes;
using Projet_AFFICHEURFERMETTE.MDF.Gestion;
using ShowableData;

namespace ModifieurFermette.ViewModels.Dialogs
{
    public class AddEvenementDialogViewModel : ObservableData
    {
        /* ===== Affichage ===== */
        private ObservableCollection<C_TitreEvenement> _Titres;
        private ObservableCollection<C_LieuEvenement> _Lieus;

        /* ===== Données entrées par l'user ===== */
        private C_TitreEvenement _SelectedTitre;
        private C_LieuEvenement _SelectedLieu;
        private DateTime _DateDebut;
        private DateTime _TimeDebut;
        private DateTime _DateFin;
        private DateTime _TimeFin;
        private int _SelectedTypeEvenement;
        private string _Description;

        public int IDevenement; // uniquement utilisé en cas de modification d'un événement existant

        /* ===== Validation ===== */
        private bool _Validated;
        private string _DateError; // Permet de définir la visibilité du message d'erreur; on alterne entre "Collapsed" (idem que caché mais ne prend pas de place) et "Visible"

        public AddEvenementDialogViewModel(string sChConn) // Ajout
        {
            LoadData(sChConn);

            // On met les dates à maintenant et maintenant + 1h pour la date de fin
            DateDebut = DateFin = DateTime.Now.Date;
            TimeDebut = DateTime.Now;
            TimeFin = DateTime.Now.AddHours(1);

            DateError = "Collapsed"; // On cache le message d'erreur
        }
        public AddEvenementDialogViewModel(string sChConn, ShowViewEvenement evenement) // Modification
        {
            LoadData(sChConn);

            // Pré-remplissage du dialog avec les données de l'événement à modifier
            IDevenement = evenement.ID;
            SelectedTitre = Titres.First(titre => titre.Titre == evenement.Titre); // Ne sélectionne pas réellement !?!
            SelectedLieu = Lieus.First(lieu => lieu.Lieu == evenement.Lieu);
            DateDebut = evenement.DateDebut.Date;
            TimeDebut = evenement.DateDebut;
            DateFin = evenement.DateFin.Date;
            TimeFin = evenement.DateFin;
            switch (evenement.TypeEvenement)
            {
                case "divers":
                default:
                    SelectedTypeEvenement = 0;
                    break;
                case "atelier":
                    SelectedTypeEvenement = 1;
                    break;
                case "compétition":
                    SelectedTypeEvenement = 2;
                    break;
            }
            Description = evenement.Description;

            DateError = "Collapsed"; // On cache le message d'erreur
        }

        private void LoadData(string sChConn)
        {
            // Chargement des données de la DB
            Titres = new ObservableCollection<C_TitreEvenement>();
            new G_TitreEvenement(sChConn).Lire("").ForEach(item => Titres.Add(item));
            Lieus = new ObservableCollection<C_LieuEvenement>();
            new G_LieuEvenement(sChConn).Lire("").ForEach(item => Lieus.Add(item));
        }

        private void IsAllItemsValid()
        {
            if (!IsDateDebutBeforeDateFin() || SelectedTitre == null || SelectedLieu == null)
                Validated = false;
            else
                Validated = true;
        }
        private bool IsDateDebutBeforeDateFin()
        {
            DateTime Debut = DateDebut.Add(TimeDebut.TimeOfDay);
            DateTime Fin = DateFin.Add(TimeFin.TimeOfDay);

            if (Debut < Fin)
            {
                DateError = "Collapsed";
                return true;
            }
            DateError = "Visible";
            return false;
        }

        public ObservableCollection<C_TitreEvenement> Titres
        {
            get { return _Titres; }
            set
            {
                if (this._Titres != value)
                {
                    this._Titres = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<C_LieuEvenement> Lieus
        {
            get { return _Lieus; }
            set
            {
                if (this._Lieus != value)
                {
                    this._Lieus = value;
                    OnPropertyChanged();
                }
            }
        }
        public C_TitreEvenement SelectedTitre
        {
            get { return _SelectedTitre; }
            set
            {
                if (this._SelectedTitre != value)
                {
                    this._SelectedTitre = value;
                    OnPropertyChanged();
                    IsAllItemsValid();
                }
            }
        }
        public C_LieuEvenement SelectedLieu
        {
            get { return _SelectedLieu; }
            set
            {
                if (this._SelectedLieu != value)
                {
                    this._SelectedLieu = value;
                    OnPropertyChanged();
                    IsAllItemsValid();
                }
            }
        }
        public DateTime DateDebut
        {
            get { return _DateDebut; }
            set
            {
                if (this._DateDebut != value)
                {
                    this._DateDebut = value;
                    OnPropertyChanged();
                    IsAllItemsValid();
                }
            }
        }
        public DateTime TimeDebut
        {
            get { return _TimeDebut; }
            set
            {
                if (this._TimeDebut != value)
                {
                    this._TimeDebut = value;
                    OnPropertyChanged();
                    IsAllItemsValid();
                }
            }
        }
        public DateTime DateFin
        {
            get { return _DateFin; }
            set
            {
                if (this._DateFin != value)
                {
                    this._DateFin = value;
                    OnPropertyChanged();
                    IsAllItemsValid();
                }
            }
        }
        public DateTime TimeFin
        {
            get { return _TimeFin; }
            set
            {
                if (this._TimeFin != value)
                {
                    this._TimeFin = value;
                    OnPropertyChanged();
                    IsAllItemsValid();
                }
            }
        }
        public int SelectedTypeEvenement
        {
            get { return _SelectedTypeEvenement; }
            set
            {
                if (this._SelectedTypeEvenement != value)
                {
                    this._SelectedTypeEvenement = value;
                    OnPropertyChanged();
                    IsAllItemsValid();
                }
            }
        }
        public string Description
        {
            get { return _Description; }
            set
            {
                if (this._Description != value)
                {
                    this._Description = value;
                    OnPropertyChanged();
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
        public string DateError
        {
            get { return _DateError; }
            set
            {
                if (this._DateError != value)
                {
                    this._DateError = value;
                    OnPropertyChanged();
                }
            }
        }

        // Utilisés en cas d'ajout dans les combobox
        // On sait qu'un nouvel item devra être ajouté à la DB grâce à son ID de 0
        public string NewTitre
        {
            set
            {
                // On ajoute que si l'item n'existe pas encore
                if (!string.IsNullOrEmpty(value) && Titres.FirstOrDefault(t => t.Titre == value) == null)
                {
                    Titres.Add(new C_TitreEvenement(0, value));
                    SelectedTitre = Titres.Last();
                }
            }
        }
        public string NewLieu
        {
            set
            {
                if (!string.IsNullOrEmpty(value) && Lieus.FirstOrDefault(l => l.Lieu == value) == null)
                {
                    Lieus.Add(new C_LieuEvenement(0, value));
                    SelectedLieu = Lieus.Last();
                }
            }
        }
    }
}
