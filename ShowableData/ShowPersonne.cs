using Projet_AFFICHEURFERMETTE.MDF.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowableData
{
    public class ShowPersonne
    {
        #region Données membres
        private int _ID;
        private string _Nom;
        private string _Prenom;
        private string _DateNaissance;
        private string _Photo;
        private string _Role;
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
            set { _ID = value; }
        }
        public string Nom
        {
            get { return _Nom; }
            set { _Nom = value; }
        }
        public string Prenom
        {
            get { return _Prenom; }
            set { _Prenom = value; }
        }
        public string DateNaissance
        {
            get { return _DateNaissance; }
            set { _DateNaissance = value; }
        }
        public string Photo
        {
            get { return _Photo; }
            set { _Photo = value; }
        }
        public string Role
        {
            get { return _Role; }
            set { _Role = value; }
        }
        #endregion
    }
}
