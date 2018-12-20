using Projet_AFFICHEURFERMETTE.MDF.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowableData
{
    public class ShowPersonne : ObservableData
    {
        #region Données membres
        private int _ID;
        private string _Nom;
        private string _Prenom;
        private string _DateNaissance;
        private string _Photo;
        private string _Role;
        private bool _IsSelected;
        #endregion

        public ShowPersonne(C_Personne OriPersonne)
        {
            ID = OriPersonne.ID;
            Nom = OriPersonne.Nom;
            Prenom = OriPersonne.Prenom;
            DateNaissance = OriPersonne.DateNaissance.ToShortDateString();
            Photo = OriPersonne.Photo;
            Role = OriPersonne.Role ? "éducateur" : "bénéficiaire";
        }

        public C_Personne GetOriginal()
        {
            return new C_Personne(ID, Nom, Prenom, DateTime.Parse(DateNaissance), Photo, Role == "éducateur" ? true : false);
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
        public string Nom
        {
            get { return _Nom; }
            set {
                if (this._Nom != value)
                {
                    this._Nom = value;
                    OnPropertyChanged("Nom");
                }
            }
        }
        public string Prenom
        {
            get { return _Prenom; }
            set {
                if (this._Prenom != value)
                {
                    this._Prenom = value;
                    OnPropertyChanged("Prenom");
                }
            }
        }
        public string DateNaissance
        {
            get { return _DateNaissance; }
            set
            {
                if (this._DateNaissance != value)
                {
                    this._DateNaissance = value;
                    OnPropertyChanged("DateNaissance");
                }
            }
        }
        public string Photo
        {
            get { return _Photo; }
            set {
                if (this._Photo != value)
                {
                    this._Photo = value;
                    OnPropertyChanged("Photo");
                }
            }
        }
        public string Role
        {
            get { return _Role; }
            set {
                if (this._Role != value)
                {
                    this._Role = value;
                    OnPropertyChanged("Role");
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
        #endregion
    }
}
