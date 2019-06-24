using System;
using System.ComponentModel;
using Projet_AFFICHEURFERMETTE.MDF.Classes;

namespace ShowableData
{
    /// <summary>
    /// Modification de la classe "C_ViewMenuDuJour" afin de la rendre directement affichable (=> ne contient plus que des string)
    /// </summary>
    public class ShowViewMenuDuJour : ObservableData
    {
        #region Données membres
        private int _ID;
        private string _Date;
        private string _eNom;
        private string _pNom;
        private string _dNom;
        private bool _IsSelected; // Est utilisé pour savoir si l'item est sélectionné dans les DataGrid ou non
        #endregion

        public ShowViewMenuDuJour(C_ViewMenuDuJour OriViewMenuDuJour)
        {
            ID = OriViewMenuDuJour.ID;
            Date = OriViewMenuDuJour.Date.ToShortDateString();
            eNom = OriViewMenuDuJour.eNom;
            pNom = OriViewMenuDuJour.pNom;
            dNom = OriViewMenuDuJour.dNom;
            IsSelected = false;
        }

        public C_ViewMenuDuJour GetOriginal()
        {
            return new C_ViewMenuDuJour(ID, DateTime.Parse(Date), eNom, null, pNom, null, dNom, null);
        }

        public bool ToggleIsSelected()
        {
            if (IsSelected)
                IsSelected = false;
            else
                IsSelected = true;
            return IsSelected;
        }

        #region Accesseurs
        public int ID
        {
            get { return _ID; }
            set
            {
                if (this._ID != value)
                {
                    this._ID = value;
                    OnPropertyChanged("ID");
                }
            }
        }
        public string Date
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
        public string eNom
        {
            get { return _eNom; }
            set
            {
                if (this._eNom != value)
                {
                    this._eNom = value;
                    OnPropertyChanged("eNom");
                }
            }
        }
        public string pNom
        {
            get { return _pNom; }
            set
            {
                if (this._pNom != value)
                {
                    this._pNom = value;
                    OnPropertyChanged("pNom");
                }
            }
        }
        public string dNom
        {
            get { return _dNom; }
            set
            {
                if (this._dNom != value)
                {
                    this._dNom = value;
                    OnPropertyChanged("dNom");
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
