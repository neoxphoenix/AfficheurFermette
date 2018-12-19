using System;
using Projet_AFFICHEURFERMETTE.MDF.Classes;

namespace ShowableData
{
    /// <summary>
    /// Modification de la classe "C_ViewMenuDuJour" afin de la rendre directement affichable (=> ne contient plus que des string)
    /// </summary>
    public class ShowViewMenuDuJour
    {
        #region Données membres
        private int _ID;
        private string _Date;
        private string _eNom;
        private string _pNom;
        private string _dNom;
        #endregion

        public ShowViewMenuDuJour(C_ViewMenuDuJour OriViewMenuDuJour)
        {
            ID = OriViewMenuDuJour.ID;
            Date = OriViewMenuDuJour.Date.ToShortDateString();
            eNom = OriViewMenuDuJour.eNom;
            pNom = OriViewMenuDuJour.pNom;
            dNom = OriViewMenuDuJour.dNom;
        }

        public C_ViewMenuDuJour GetOriginal()
        {
            return new C_ViewMenuDuJour(ID, DateTime.Parse(Date), eNom, pNom, dNom);
        }

        #region Accesseurs
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        public string Date
        {
            get { return _Date; }
            set { _Date = value; }
        }
        public string eNom
        {
            get { return _eNom; }
            set { _eNom = value; }
        }
        public string pNom
        {
            get { return _pNom; }
            set { _pNom = value; }
        }
        public string dNom
        {
            get { return _dNom; }
            set { _dNom = value; }
        }
        #endregion
    }
}
