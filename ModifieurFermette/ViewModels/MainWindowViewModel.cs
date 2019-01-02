﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;

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
        // Verrous utilisés pour les opérations cross-thread sur nos collections
        public object MenusAffLock = new object();
        public object EvenementsAffLock = new object();
        public object PersonnesAffLock = new object();

        private bool _IsAllItemsEvenementsSelected, _IsAllItemsPersonnesSelected, _IsAllItemsMenuDuJourSelected;

        public ConfigClass config;

        #region Commands
        private ICommand _DeleteShowViewMenuDuJourCmd;
        private ICommand _DeleteShowViewEvenementCmd;
        private ICommand _DeleteShowPersonneCmd;
        #endregion
        #endregion

        public MainWindowViewModel()
        {
            DeleteShowViewMenuDuJourCmd = new RelayCommand(Exec => ExecuteDeleteShowViewMenuDuJour(), CanExec => CanExecDeleteShowViewMenuDuJour());
            DeleteShowViewEvenementCmd = new RelayCommand(Exec => ExecuteDeleteShowViewEvenement(), CanExec => CanExecDeleteShowViewEvenement());
            DeleteShowPersonneCmd = new RelayCommand(Exec => ExecuteDeleteShowPersonne(), CanExec => CanExecDeleteShowPersonne());
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

            // Synchronisation de nos ObservableCollections pour opérations cross-thread
            BindingOperations.EnableCollectionSynchronization(MenusAff, MenusAffLock);
            BindingOperations.EnableCollectionSynchronization(EvenementsAff, EvenementsAffLock);
            BindingOperations.EnableCollectionSynchronization(PersonnesAff, PersonnesAffLock);
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
                #region ShowViewMenuDuJour
                    #region Suppression
        /// <summary>
        /// Lance un dialog async pour confirmer la suppression
        /// </summary>
        private async void ExecuteDeleteShowViewMenuDuJour()
        {
            var ConfDialog = new Views.Dialogs.ConfirmDialog("Confirmer", "Confirmer la suppression ?");

            await DialogHost.Show(ConfDialog, ConfDeleteShowViewMenuDuJourDialogClosing); // arg 1) le dialog, 2) l'événement de fermeture du dialog (on y intercepte le choix de l'utilisateur, OK ou Cancel)

        }
        /// <summary>
        /// Vérifie qu'au moins un élément est sélectionné
        /// </summary>
        /// <returns>vrai si au moins un élément est sélectionné</returns>
        private bool CanExecDeleteShowViewMenuDuJour()
        {
            foreach (ShowViewMenuDuJour TmpMenu in MenusAff)
            {
                if (TmpMenu.IsSelected)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Evenement déclenché par la fermeture du dialog de suppression d'un menu du jour;
        /// permet de vérifier la confirmation de l'utilisateur et de supprimer les éléments sélectionnés
        /// </summary>
        private async void ConfDeleteShowViewMenuDuJourDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter == false) return; // Si l'utilisateur à appuyer sur annuler, on arrête là

            eventArgs.Cancel(); // On empêche la fermeture

            eventArgs.Session.UpdateContent(new Views.Dialogs.ProgressDialog()); // On remplace l'ancien dialogue par un nouveau avec une roue de chargement

            Task RemovingItems = Task.Run(() => { // Lancement d'un thread pour la suppresion des éléments
                lock (MenusAffLock) // Verouillage de notre ObservableCollection pour pouvoir la modifier
                {
                    // Suppression des éléments
                    // Vu qu'on ne peut pas directement retirer les éléments sans perturber l'indexation de la liste,
                    // on stocke les éléments à supprimer dans une seconde liste
                    List<ShowViewMenuDuJour> ItemsToRemove = new List<ShowViewMenuDuJour>();
                    foreach (ShowViewMenuDuJour TmpMenu in MenusAff)
                    {
                        if (TmpMenu.IsSelected)
                            ItemsToRemove.Add(TmpMenu);
                    }
                    for (int i = 0; i < ItemsToRemove.Count; i++)
                    {
                        MenusAff.Remove(ItemsToRemove[i]); // Retiré localement
                        new G_Menu(config.sChConn).Supprimer(ItemsToRemove[i].ID); // Retiré dans la DB (uniquement le menu, pas les plats)
                    }
                }
            });

            // Fermeture du Dialog
            await RemovingItems.ContinueWith((t, _) => eventArgs.Session.Close(false), null,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }
        #endregion
        #endregion
                #region ShowViewEvenement
                    #region Suppression
        /// <summary>
        /// Lance un dialog async pour confirmer la suppression
        /// </summary>
        private async void ExecuteDeleteShowViewEvenement()
        {
            var ConfDialog = new Views.Dialogs.ConfirmDialog("Confirmer", "Confirmer la suppression ?");

            await DialogHost.Show(ConfDialog, ConfDeleteShowViewEvenementDialogClosing); // arg 1) le dialog, 2) l'événement de fermeture du dialog (on y intercepte le choix de l'utilisateur, OK ou Cancel)

        }
        /// <summary>
        /// Vérifie qu'au moins un élément est sélectionné
        /// </summary>
        /// <returns>vrai si au moins un élément est sélectionné</returns>
        private bool CanExecDeleteShowViewEvenement()
        {
            foreach (ShowViewEvenement TmpEvenement in EvenementsAff)
            {
                if (TmpEvenement.IsSelected)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Evenement déclenché par la fermeture du dialog de suppression d'un événement;
        /// permet de vérifier la confirmation de l'utilisateur et de supprimer les éléments sélectionnés
        /// </summary>
        private async void ConfDeleteShowViewEvenementDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter == false) return; // Si l'utilisateur à appuyer sur annuler, on arrête là

            eventArgs.Cancel(); // On empêche la fermeture

            eventArgs.Session.UpdateContent(new Views.Dialogs.ProgressDialog()); // On remplace l'ancien dialogue par un nouveau avec une roue de chargement

            Task RemovingItems = Task.Run(() => { // Lancement d'un thread pour la suppresion des éléments
                lock (EvenementsAffLock) // Verouillage de notre ObservableCollection pour pouvoir la modifier
                {
                    // Suppression des éléments
                    // Vu qu'on ne peut pas directement retirer les éléments sans perturber l'indexation de la liste,
                    // on stocke les éléments à supprimer dans une seconde liste
                    List<ShowViewEvenement> ItemsToRemove = new List<ShowViewEvenement>();
                    foreach (ShowViewEvenement TmpEvenement in EvenementsAff)
                    {
                        if (TmpEvenement.IsSelected)
                            ItemsToRemove.Add(TmpEvenement);
                    }
                    for (int i = 0; i < ItemsToRemove.Count; i++)
                    {
                        EvenementsAff.Remove(ItemsToRemove[i]); // Retiré localement
                        new G_Evenement(config.sChConn).Supprimer(ItemsToRemove[i].ID); // Retiré dans la DB (uniquement l'événement)
                        // TODO : suppression des PersonnesConcernees
                        // TODO : suppression des classements
                    }
                }
            });

            // Fermeture du Dialog
            await RemovingItems.ContinueWith((t, _) => eventArgs.Session.Close(false), null,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }
        #endregion
        #endregion
        #region ShowPersonne
        #region Suppression
        /// <summary>
        /// Lance un dialog async pour confirmer la suppression
        /// </summary>
        private async void ExecuteDeleteShowPersonne()
        {
            var ConfDialog = new Views.Dialogs.ConfirmDialog("Confirmer", "Confirmer la suppression ?");

            await DialogHost.Show(ConfDialog, ConfDeleteShowPersonneDialogClosing); // arg 1) le dialog, 2) l'événement de fermeture du dialog (on y intercepte le choix de l'utilisateur, OK ou Cancel)

        }
        /// <summary>
        /// Vérifie qu'au moins un élément est sélectionné
        /// </summary>
        /// <returns>vrai si au moins un élément est sélectionné</returns>
        private bool CanExecDeleteShowPersonne()
        {
            foreach (ShowPersonne TmpPersonne in PersonnesAff)
            {
                if (TmpPersonne.IsSelected)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Evenement déclenché par la fermeture du dialog de suppression d'un événement;
        /// permet de vérifier la confirmation de l'utilisateur et de supprimer les éléments sélectionnés
        /// </summary>
        private async void ConfDeleteShowPersonneDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter == false) return; // Si l'utilisateur à appuyer sur annuler, on arrête là

            eventArgs.Cancel(); // On empêche la fermeture

            eventArgs.Session.UpdateContent(new Views.Dialogs.ProgressDialog()); // On remplace l'ancien dialogue par un nouveau avec une roue de chargement

            Task RemovingItems = Task.Run(() => { // Lancement d'un thread pour la suppresion des éléments
                lock (PersonnesAffLock) // Verouillage de notre ObservableCollection pour pouvoir la modifier
                {
                    // Suppression des éléments
                    // Vu qu'on ne peut pas directement retirer les éléments sans perturber l'indexation de la liste,
                    // on stocke les éléments à supprimer dans une seconde liste
                    List<ShowPersonne> ItemsToRemove = new List<ShowPersonne>();
                    foreach (ShowPersonne TmpPersonne in PersonnesAff)
                    {
                        if (TmpPersonne.IsSelected)
                            ItemsToRemove.Add(TmpPersonne);
                    }
                    for (int i = 0; i < ItemsToRemove.Count; i++)
                    {
                        PersonnesAff.Remove(ItemsToRemove[i]); // Retiré localement
                        new G_Personne(config.sChConn).Supprimer(ItemsToRemove[i].ID); // Retiré dans la DB (uniquement l'événement)
                        // TODO : suppression des PersonnesConcernees
                        // TODO : suppression des classements
                    }
                }
            });

            // Fermeture du Dialog
            await RemovingItems.ContinueWith((t, _) => eventArgs.Session.Close(false), null,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }
        #endregion
        #endregion
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
        #region Suppression
        public ICommand DeleteShowViewMenuDuJourCmd
        {
            get
            {
                return _DeleteShowViewMenuDuJourCmd;
            }
            set
            {
                _DeleteShowViewMenuDuJourCmd = value;
            }
        }
        public ICommand DeleteShowViewEvenementCmd
        {
            get
            {
                return _DeleteShowViewEvenementCmd;
            }
            set
            {
                _DeleteShowViewEvenementCmd = value;
            }
        }
        public ICommand DeleteShowPersonneCmd
        {
            get
            {
                return _DeleteShowPersonneCmd;
            }
            set
            {
                _DeleteShowPersonneCmd = value;
            }
        }
        #endregion
        #endregion
        #endregion
    }
}
