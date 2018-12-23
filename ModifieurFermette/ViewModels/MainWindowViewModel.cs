using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ModifieurFermette.Models;

// dll
using Projet_AFFICHEURFERMETTE.MDF.Acces;
using Projet_AFFICHEURFERMETTE.MDF.Classes;
using Projet_AFFICHEURFERMETTE.MDF.Gestion;
using ShowableData;

namespace ModifieurFermette.ViewModels
{
    class MainWindowViewModel : ObservableData
    {
        #region Données membres
        public ObservableCollection<ShowViewMenuDuJour> MenusAff;
        public ObservableCollection<ShowViewEvenement> EvenementsAff;
        public ObservableCollection<ShowPersonne> PersonnesAff;

        private bool _IsAllItemsEvenementsSelected, _IsAllItemsPersonnesSelected, _IsAllItemsMenuDuJourSelected;

        public ConfigClass config;

        #region Commands
        private ICommand _DeleteShowViewMenuDuJourItem;
        #endregion
        #endregion

        public MainWindowViewModel()
        {
            DeleteShowViewMenuDuJourItem = new RelayCommand(Exec => DeleteSelectedShowViewMenuDuJourItem(), CanExec => CanExecDeleteSelectedShowViewMenuDuJourItem());
        }

        #region Méthodes
        public void ChargerDonnees()
        {
            // Extraction des données de la DB
            List<C_ViewMenuDuJour> Menus = new G_ViewMenuDuJour(config.sChConn).Lire("");
            List<C_ViewEvenement> Evenements = new G_ViewEvenement(config.sChConn).Lire("");
            List<C_Personne> Personnes = new G_Personne(config.sChConn).Lire("");

            // Placement dans des Oservables
            MenusAff = new ObservableCollection<ShowViewMenuDuJour>();
            EvenementsAff = new ObservableCollection<ShowViewEvenement>();
            PersonnesAff = new ObservableCollection<ShowPersonne>();
            foreach (C_ViewMenuDuJour TmpMenu in Menus)
            {
                MenusAff.Add(new ShowViewMenuDuJour(TmpMenu));
            }
            foreach (C_ViewEvenement TmpEvenement in Evenements)
            {
                EvenementsAff.Add(new ShowViewEvenement(TmpEvenement));
            }
            foreach (C_Personne TmpPersonne in Personnes)
            {
                PersonnesAff.Add(new ShowPersonne(TmpPersonne));
            }
        }
        #region Sélection DG
        /// <summary>
        /// (dé)sélectionne tous les éléments de la DG des menus du jour
        /// </summary>
        /// <param name="Check">Si vrai, sélectionne tous les éléments</param>
        private void SelectAllShowViewMenuDuJour(bool Check)
        {
            foreach (ShowViewMenuDuJour TmpMenu in MenusAff)
            {
                TmpMenu.IsSelected = Check;
            }
        }
        /// <summary>
        /// (dé)sélectionne tous les éléments de la DG des événements
        /// </summary>
        /// <param name="Check">Si vrai, sélectionne tous les éléments</param>
        private void SelectAllShowViewEvenement(bool Check)
        {
            foreach (ShowViewEvenement TmpEvenement in EvenementsAff)
            {
                TmpEvenement.IsSelected = Check;
            }
        }
        /// <summary>
        /// (dé)sélectionne tous les éléments de la DG des personnes
        /// </summary>
        /// <param name="Check">Si vrai, sélectionne tous les éléments</param>
        private void SelectAllShowPersonne(bool Check)
        {
            foreach (ShowPersonne TmpPersonne in PersonnesAff)
            {
                TmpPersonne.IsSelected = Check;
            }
        }
        #endregion
        #region Commands
        private void DeleteSelectedShowViewMenuDuJourItem()
        {
            List<ShowViewMenuDuJour> ItemsToRemove = new List<ShowViewMenuDuJour>();
            foreach (ShowViewMenuDuJour TmpMenu in MenusAff)
            {
                if (TmpMenu.IsSelected)
                    ItemsToRemove.Add(TmpMenu);
            }
            for (int i = 0; i < ItemsToRemove.Count; i++)
            {
                MenusAff.RemoveAt(MenusAff.IndexOf(ItemsToRemove[i])); // Retiré localement
                new G_Menu(config.sChConn).Supprimer(ItemsToRemove[i].ID); // Retiré dans la DB (uniquement le menu, pas les plats
            }
        }
        /// <summary>
        /// Vérifie qu'au moins un élément est sélectionné
        /// </summary>
        /// <returns>vrai si au moins un élément est sélectionné</returns>
        private bool CanExecDeleteSelectedShowViewMenuDuJourItem()
        {
            foreach (ShowViewMenuDuJour TmpMenu in MenusAff)
            {
                if (TmpMenu.IsSelected)
                    return true;
            }
            return false;
        }
        #endregion
        #endregion

        #region Accesseurs
        public bool IsAllItemsEvenementsSelected
        {
            get { return _IsAllItemsEvenementsSelected; }
            set
            {
                if (this._IsAllItemsEvenementsSelected != value)
                {
                    this._IsAllItemsEvenementsSelected = value;
                    SelectAllShowViewEvenement(value);
                    OnPropertyChanged("IsAllItemsEvenementsSelected");
                }
            }
        }
        public bool IsAllItemsPersonnesSelected
        {
            get { return _IsAllItemsPersonnesSelected; }
            set
            {
                if (this._IsAllItemsPersonnesSelected != value)
                {
                    this._IsAllItemsPersonnesSelected = value;
                    SelectAllShowPersonne(value);
                    OnPropertyChanged("IsAllItemsPersonnesSelected");
                }
            }
        }
        public bool IsAllItemsMenuDuJourSelected
        {
            get { return _IsAllItemsMenuDuJourSelected; }
            set
            {
                if (this._IsAllItemsMenuDuJourSelected != value)
                {
                    this._IsAllItemsMenuDuJourSelected = value;
                    SelectAllShowViewMenuDuJour(value);
                    OnPropertyChanged("IsAllItemsMenuDuJourSelected");
                }
            }
        }
        #region Commands
        public ICommand DeleteShowViewMenuDuJourItem
        {
            get
            {
                return _DeleteShowViewMenuDuJourItem;
            }
            set
            {
                _DeleteShowViewMenuDuJourItem = value;
            }
        }
        #endregion
        #endregion
    }
}
