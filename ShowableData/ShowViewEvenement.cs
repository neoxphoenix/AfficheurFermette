using System;
using System.Collections.Generic;
using System.Text;
using Projet_AFFICHEURFERMETTE.MDF.Classes;

namespace ShowableData
{
    public class ShowViewEvenement : ObservableData
    {
        #region Données membres
        private int _ID;
        private string _Titre;
        private string _Lieu;
        private string _Adresse;
        private string _TypeEvenement;
        private DateTime _DateDebut;
        private DateTime _DateFin;
        private string _Description;
        private bool _IsSelected;
        #endregion

        public ShowViewEvenement(C_ViewEvenement OriViewEvenement)
        {
            ID = OriViewEvenement.ID;
            Titre = OriViewEvenement.Titre;
            Lieu = OriViewEvenement.Lieu;
            Adresse = OriViewEvenement.Adresse;
            switch (OriViewEvenement.TypeEvenement)
            {
                case 0:
                default:
                    TypeEvenement = "divers";
                    break;
                case 1:
                    TypeEvenement = "atelier";
                    break;
                case 2:
                    TypeEvenement = "compétition";
                    break;
            }
            DateDebut = OriViewEvenement.DateDebut;
            DateFin = OriViewEvenement.DateFin;
            Description = OriViewEvenement.Description;
        }

        public C_ViewEvenement GetOriginal()
        {
            int TypeEvenement_;
            switch (TypeEvenement)
            {
                case "divers":
                default:
                    TypeEvenement_ = 0;
                    break;
                case "atelier":
                    TypeEvenement_ = 1;
                    break;
                case "compétition":
                    TypeEvenement_ = 2;
                    break;
            }
            return new C_ViewEvenement(ID, Titre, Lieu, Adresse, TypeEvenement_, DateDebut, DateFin, Description);
        }

        #region Accesseurs
        public int ID
        {
            get { return _ID; }
            set {
                if (this._ID != value)
                {
                    this._ID = value;
                    OnPropertyChanged("ID");
                }
            }
        }
        public string Titre
        {
            get { return _Titre; }
            set {
                if (this._Titre != value)
                {
                    this._Titre = value;
                    OnPropertyChanged("Titre");
                }
            }
        }
        public string Lieu
        {
            get { return _Lieu; }
            set {
                if (this._Lieu != value)
                {
                    this._Lieu = value;
                    OnPropertyChanged("Lieu");
                }
            }
        }
        public string TypeEvenement
        {
            get { return _TypeEvenement; }
            set {
                if (this._TypeEvenement != value)
                {
                    this._TypeEvenement = value;
                    OnPropertyChanged("TypeEvenement");
                }
            }
        }
        public DateTime DateDebut
        {
            get { return _DateDebut; }
            set {
                if (this._DateDebut != value)
                {
                    this._DateDebut = value;
                    OnPropertyChanged("DateDebut");
                }
            }
        }
        public DateTime DateFin
        {
            get { return _DateFin; }
            set {
                if (this._DateFin != value)
                {
                    this._DateFin = value;
                    OnPropertyChanged("DateFin");
                }
            }
        }
        public string Description
        {
            get { return _Description; }
            set {
                if (this._Description != value)
                {
                    this._Description = value;
                    OnPropertyChanged("Description");
                }
            }
        }
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (this._IsSelected != value)
                {
                    this._IsSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        public string Adresse { get => _Adresse;
            set
            {
                if (this._Adresse != value)
                {
                    this._Adresse = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }
        #endregion
    }
}
