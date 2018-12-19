using System;
using System.Collections.Generic;
using System.Text;
using Projet_AFFICHEURFERMETTE.MDF.Classes;

namespace ShowableData
{
    class ShowViewEvenement
    {
        #region Données membres
        private int _ID;
        private string _Titre;
        private string _Lieu;
        private string _TypeEvenement;
        private DateTime _DateDebut;
        private DateTime _DateFin;
        private string _Description;
        #endregion

        ShowViewEvenement(C_ViewEvenement OriViewEvenement)
        {
            ID = OriViewEvenement.ID;
            Titre = OriViewEvenement.Titre;
            Lieu = OriViewEvenement.Lieu;
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
            return new C_ViewEvenement(ID, Titre, Lieu, TypeEvenement_, DateDebut, DateFin, Description);
        }

        #region Accesseurs
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        public string Titre
        {
            get { return _Titre; }
            set { _Titre = value; }
        }
        public string Lieu
        {
            get { return _Lieu; }
            set { _Lieu = value; }
        }
        public string TypeEvenement
        {
            get { return _TypeEvenement; }
            set { _TypeEvenement = value; }
        }
        public DateTime DateDebut
        {
            get { return _DateDebut; }
            set { _DateDebut = value; }
        }
        public DateTime DateFin
        {
            get { return _DateFin; }
            set { _DateFin = value; }
        }
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        #endregion
    }
}
