using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using System.IO;

using ModifieurFermette.Views.Dialogs;

// dll
using Projet_AFFICHEURFERMETTE.MDF.Acces;
using Projet_AFFICHEURFERMETTE.MDF.Classes;
using Projet_AFFICHEURFERMETTE.MDF.Gestion;
using ShowableData;
using System.Diagnostics;

namespace ModifieurFermette.ViewModels
{
    class MainWindowViewModel : ObservableData
    {
        #region Données membres
        public ObservableCollection<ShowViewMenuDuJour> _MenusAff;
        public ObservableCollection<ShowViewEvenement> _EvenementsAff;
        public ObservableCollection<ShowPersonne> _PersonnesAff;
        // Verrous utilisés pour les opérations cross-thread sur nos collections
        public object MenusAffLock = new object();
        public object EvenementsAffLock = new object();
        public object PersonnesAffLock = new object();

        private bool _IsAllItemsEvenementsSelected, _IsAllItemsPersonnesSelected, _IsAllItemsMenuDuJourSelected;

        public ConfigClass config;

        #region Commands
        #region App
        private ICommand _OpenAffCmd;
        private ICommand _CloseAppCmd;
        #endregion
        #region Suppression
        private ICommand _DeleteShowViewMenuDuJourCmd;
        private ICommand _DeleteShowViewEvenementCmd;
        private ICommand _DeleteShowPersonneCmd;
        #endregion
        #region Ajout
        private ICommand _AddShowViewMenuDuJourCmd;
        private ICommand _AddShowPersonneCmd;
        private ICommand _AddShowViewEvenementCmd;
        #endregion
        #region Modification
        private ICommand _UpdateShowViewMenuDuJourCmd;
        private ICommand _UpdateShowPersonneCmd;
        private ICommand _UpdateShowViewEvenementCmd;
        #endregion
        #region Détails
        private ICommand _DetailsShowViewMenuDuJourCmd;
        private ICommand _DetailsShowViewEvenementCmd;
        private ICommand _DetailsShowPersonneCmd;
        #endregion
        #region Copie
        private ICommand _CopyShowViewEvenementCmd;
        private ICommand _CopyShowViewMenuDuJourCmd;
        #endregion
        #region Autres
        private ICommand _ManagePlatsCmd;
        private ICommand _ManagePartEvenementCmd;
        private ICommand _ManagePicEvenementCmd;
        private ICommand _ManageTitreLieuCmd;
        #endregion
        #endregion
        #endregion

        public MainWindowViewModel()
        {
            OpenAffCmd = new RelayCommand(Exec => ExecuteOpenAff());
            CloseAppCmd = new RelayCommand(Exec => CloseAction()); // Fermeture du programme

            DeleteShowViewMenuDuJourCmd = new RelayCommand(Exec => ExecuteDeleteShowViewMenuDuJour(), CanExec => CanExecDeleteShowViewMenuDuJour());
            DeleteShowViewEvenementCmd = new RelayCommand(Exec => ExecuteDeleteShowViewEvenement(), CanExec => CanExecDeleteShowViewEvenement());
            DeleteShowPersonneCmd = new RelayCommand(Exec => ExecuteDeleteShowPersonne(), CanExec => CanExecDeleteShowPersonne());

            AddShowViewMenuDuJourCmd = new RelayCommand(Exec => ExecuteAddShowViewMenuDuJour(), CanExec => true);
            AddShowPersonneCmd = new RelayCommand(Exec => ExecuteAddShowPersonne(), CanExec => true);
            AddShowViewEvenementCmd = new RelayCommand(Exec => ExecuteAddShowViewEvenement(), CanExec => true);

            UpdateShowViewMenuDuJourCmd = new RelayCommand(Exec => ExecuteUpdateShowViewMenuDuJour(), CanExec => CanExecUpdateShowViewMenuDuJour());
            UpdateShowViewEvenementCmd = new RelayCommand(Exec => ExecuteUpdateShowViewEvenement(), CanExec => CanExecUpdateShowViewEvenement());
            UpdateShowPersonneCmd = new RelayCommand(Exec => ExecuteUpdateShowPersonne(), CanExec => CanExecUpdateShowPersonne());

            DetailsShowViewMenuDuJourCmd = new RelayCommand(Exec => ExecuteDetailsShowViewMenuDuJour(), CanExec => CanExecDetailsShowViewMenuDuJour());
            DetailsShowViewEvenementCmd = new RelayCommand(Exec => ExecuteDetailsShowViewEvenement(), CanExec => CanExecDetailsShowViewEvenement());
            DetailsShowPersonneCmd = new RelayCommand(Exec => ExecuteDetailsShowPersonne(), CanExec => CanExecDetailsShowPersonne());

            CopyShowViewMenuDuJourCmd = new RelayCommand(Exec => ExecuteCopyShowViewMenuDuJour(), CanExec => CanExecDeleteShowViewMenuDuJour());
            CopyShowViewEvenementCmd = new RelayCommand(Exec => ExecuteCopyShowViewEvenement(), CanExec => CanExecDeleteShowViewEvenement());

            ManagePlatsCmd = new RelayCommand(Exec => ExecuteManagePlats(), CanExec => true);
            ManagePartEvenementCmd = new RelayCommand(Exec => ExecuteManagePartEvenement(), CanExec => CanExecDetailsShowViewEvenement());
            ManagePicEvenementCmd = new RelayCommand(Exec => ExecuteManagePicEvenement(), CanExec => CanExecDetailsShowViewEvenement());
            ManageTitreLieuCmd = new RelayCommand(Exec => ExecuteManageTitreLieu(), CanExec => true);
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
        /// <summary>
        /// Retourne le nombre de menus sélectionnés dans la datagrid
        /// </summary>
        /// <returns>Ce nombre/returns>
        private int HowManyShowViewMenusDuJourSelected()
        {
            int count = 0;
            foreach (ShowViewMenuDuJour TmpMenu in MenusAff)
            {
                if (TmpMenu.IsSelected)
                    count++;
            }
            return count;
        }
        /// <summary>
        /// Retourne le nombre d'événements sélectionnés dans la datagrid
        /// </summary>
        /// <returns>Ce nombre/returns>
        private int HowManyShowViewEvenementSelected()
        {
            int count = 0;
            foreach (ShowViewEvenement TmpEvenement in EvenementsAff)
            {
                if (TmpEvenement.IsSelected)
                    count++;
            }
            return count;
        }
        /// <summary>
        /// Retourne le nombre de personnes sélectionnés dans la datagrid
        /// </summary>
        /// <returns>Ce nombre/returns>
        private int HowManyShowPersonneSelected()
        {
            int count = 0;
            foreach (ShowPersonne TmpPersonne in PersonnesAff)
            {
                if (TmpPersonne.IsSelected)
                    count++;
            }
            return count;
        }

        #endregion
        #region Commands
        public void ExecuteOpenAff() // Démarre l'afficheur (si trouvable)
        {
            if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "AfficheurFermette.exe"))
            {
                Process p = new Process { StartInfo = new ProcessStartInfo("AfficheurFermette") };
                p.Start();
                CloseAction(); // Indispensable de fermer ce programme après avoir lancé l'afficheur vu qu'au sinon le mdf est indisponible vu que deux app essaient de s'y connecter en même temps
            }
            else
                System.Windows.MessageBox.Show("l'exécutable de l'afficheur est introuvable ! Veuillez le replacer dans le même dossier que ce programme...");
        }
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
        private bool CanExecDeleteShowViewMenuDuJour()
        {
            return HowManyShowViewMenusDuJourSelected() > 0;
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
                #region Ajout
        private async void ExecuteAddShowViewMenuDuJour()
        {
            var Dialog = new AddMenuDuJourDialog(config.sChConn);

            await DialogHost.Show(Dialog, AddShowViewMenuDuJourDialogClosing);
        }
        private async void AddShowViewMenuDuJourDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter == false) return; // Si l'utilisateur à appuyer sur annuler, on arrête là

            eventArgs.Cancel(); // On empêche la fermeture
            AddMenuDuJourDialog dg = (AddMenuDuJourDialog)eventArgs.Session.Content; // On récupère le dialog pour avoir accès à ses données
            eventArgs.Session.UpdateContent(new ProgressDialog()); // On remplace l'ancien dialogue par un nouveau avec une roue de chargement

            Task AddingItems = Task.Run(() => // Lancement d'un thread pour l'ajout de l'élément
            {
                DateTime date = dg.vm.Date.Add(dg.vm.Time.TimeOfDay); // On récupère séparément la date et l'heure dans le dialog, on les recombine donc ici

                // On vérifie si les ID sont à 0, auquel cas cela signifie que l'item doit être ajouté à la DB
                if (dg.vm.SelectedPotage.ID == 0)
                    dg.vm.SelectedPotage.ID = new G_Plat(config.sChConn).Ajouter(dg.vm.SelectedPotage.nom, dg.vm.SelectedPotage.Type);
                if (dg.vm.SelectedPlat.ID == 0)
                    dg.vm.SelectedPlat.ID = new G_Plat(config.sChConn).Ajouter(dg.vm.SelectedPlat.nom, dg.vm.SelectedPlat.Type);
                if (dg.vm.SelectedDessert.ID == 0)
                    dg.vm.SelectedDessert.ID = new G_Plat(config.sChConn).Ajouter(dg.vm.SelectedDessert.nom, dg.vm.SelectedDessert.Type);
                lock (MenusAffLock) // Verrouillage de l'ObservableCollection pour modif
                {
                    int ID = new G_Menu(config.sChConn).Ajouter(date, dg.vm.SelectedPotage.ID, dg.vm.SelectedPlat.ID, dg.vm.SelectedDessert.ID); // Insertion dans la DB
                    MenusAff.Add(new ShowViewMenuDuJour(new C_ViewMenuDuJour(ID, date, dg.vm.SelectedPotage.nom, dg.vm.SelectedPlat.nom, dg.vm.SelectedDessert.nom))); // Insertion de la liste affichée
                }
            });

            // Fermeture du Dialog
            await AddingItems.ContinueWith((t, _) => eventArgs.Session.Close(false), null,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }
        #endregion
                #region Modification
        private async void ExecuteUpdateShowViewMenuDuJour()
        {
            var Dialog = new AddMenuDuJourDialog(config.sChConn, MenusAff.First(menu => menu.IsSelected)); // On envoie le menu à modifier

            await DialogHost.Show(Dialog, UpdateShowViewMenuDuJourDialogClosing);
        }
        private async void UpdateShowViewMenuDuJourDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter == false) return; // Si l'utilisateur à appuyer sur annuler, on arrête là

            eventArgs.Cancel(); // On empêche la fermeture
            AddMenuDuJourDialog dg = (AddMenuDuJourDialog)eventArgs.Session.Content; // On récupère le dialog pour avoir accès à ses données
            eventArgs.Session.UpdateContent(new ProgressDialog()); // On remplace l'ancien dialogue par un nouveau avec une roue de chargement

            Task UpdatingItem = Task.Run(() =>
            {
                DateTime date = dg.vm.Date.Add(dg.vm.Time.TimeOfDay); // On récupère séparément la date et l'heure dans le dialog, on les recombine donc ici

                // On vérifie si les ID sont à 0, auquel cas cela signifie que l'item doit être ajouté à la DB
                if (dg.vm.SelectedPotage.ID == 0)
                    dg.vm.SelectedPotage.ID = new G_Plat(config.sChConn).Ajouter(dg.vm.SelectedPotage.nom, dg.vm.SelectedPotage.Type);
                if (dg.vm.SelectedPlat.ID == 0)
                    dg.vm.SelectedPlat.ID = new G_Plat(config.sChConn).Ajouter(dg.vm.SelectedPlat.nom, dg.vm.SelectedPlat.Type);
                if (dg.vm.SelectedDessert.ID == 0)
                    dg.vm.SelectedDessert.ID = new G_Plat(config.sChConn).Ajouter(dg.vm.SelectedDessert.nom, dg.vm.SelectedDessert.Type);
                lock (MenusAffLock)
                {
                    new G_Menu(config.sChConn).Modifier(dg.vm.IDMenuDuJour, date, dg.vm.SelectedPotage.ID, dg.vm.SelectedPlat.ID, dg.vm.SelectedDessert.ID);
                    int Index = MenusAff.IndexOf(MenusAff.First(item => item.ID == dg.vm.IDMenuDuJour));
                    MenusAff[Index] = new ShowViewMenuDuJour(new C_ViewMenuDuJour(dg.vm.IDMenuDuJour, date, dg.vm.SelectedPotage.nom, dg.vm.SelectedPlat.nom, dg.vm.SelectedDessert.nom));
                }
            });

            // Fermeture du Dialog
            await UpdatingItem.ContinueWith((t, _) => eventArgs.Session.Close(false), null,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }
        private bool CanExecUpdateShowViewMenuDuJour()
        {
            return HowManyShowViewMenusDuJourSelected() == 1;
        }
        #endregion
                #region Détails
        private async void ExecuteDetailsShowViewMenuDuJour()
        {
            var Dialog = new DetailsMenuDuJourDialog(MenusAff.First(menu => menu.IsSelected));

            await DialogHost.Show(Dialog, DetailsMenuDuJourDialogClosing);
        }
        private  void DetailsMenuDuJourDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter == false) return; // Si l'utilisateur à appuyer sur fermer, on arrête là

            eventArgs.Cancel();
            // A la fermeture de ce nouveau Dialog il repasse par cet eventHandler, c'est pourquoi il est important de set le paramètre de sortie à False pour ne pas avoir de boucle
            eventArgs.Session.UpdateContent(new ManagePlatsDialog(config.sChConn));
        }
        private bool CanExecDetailsShowViewMenuDuJour()
        {
            return HowManyShowViewMenusDuJourSelected() == 1;
        }
        #endregion
                #region Copie
        private async void ExecuteCopyShowViewMenuDuJour()
        {
            var Dialog = new ProgressDialog();

            await DialogHost.Show(Dialog, CopyShowViewMenuDuJourDialogOpening);
        }
        private async void CopyShowViewMenuDuJourDialogOpening(object sender, DialogOpenedEventArgs eventArgs)
        {
            Task CopyItems = Task.Run(() =>
            {
                lock (MenusAffLock) // verrou pour opération cross-thread
                {
                    List<C_Plat> TmpPlats = new G_Plat(config.sChConn).Lire("");
                    List<ShowViewMenuDuJour> MenusToAdd = new List<ShowViewMenuDuJour>();
                    foreach (ShowViewMenuDuJour menu in MenusAff)
                    {
                        if (menu.IsSelected) // C'est une menu qui doit être copié
                        {
                            // Récupération des infos
                            DateTime Date = DateTime.Parse(menu.Date);
                            int IDpotage = TmpPlats.First(p => p.nom == menu.eNom).ID;
                            int IDplat = TmpPlats.First(p => p.nom == menu.pNom).ID;
                            int IDdessert = TmpPlats.First(p => p.nom == menu.dNom).ID;

                            // Ajout à la DB
                            int ID = new G_Menu(config.sChConn).Ajouter(Date, IDpotage, IDplat, IDdessert);

                            // Ajout dans une liste tampon (on ne peut pas directement ajouter dans la liste principale depuis son foreach)
                            MenusToAdd.Add(new ShowViewMenuDuJour(new C_ViewMenuDuJour(ID, Date, menu.eNom, menu.pNom, menu.dNom)));
                        }
                    }
                    // Ajout local
                    MenusToAdd.ForEach(item => MenusAff.Add(item));
                }
            });

            // Fermeture du Dialog
            await CopyItems.ContinueWith((t, _) => eventArgs.Session.Close(false), null,
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
            var ConfDialog = new ConfirmDialog("Confirmer", "Confirmer la suppression ?");

            await DialogHost.Show(ConfDialog, ConfDeleteShowViewEvenementDialogClosing); // arg 1) le dialog, 2) l'événement de fermeture du dialog (on y intercepte le choix de l'utilisateur, OK ou Cancel)

        }
        private bool CanExecDeleteShowViewEvenement()
        {
            return HowManyShowViewEvenementSelected() > 0;
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
                        // Suppression des photos sur le disque dur
                        new G_ViewEvenement(config.sChConn).LirePhotosEvenement(ItemsToRemove[i].ID).ForEach
                        (
                            Pic => File.Delete(Pic.Photo)
                        );

                        EvenementsAff.Remove(ItemsToRemove[i]); // Retiré localement
                        new G_Evenement(config.sChConn).Supprimer(ItemsToRemove[i].ID); // Retiré dans la DB (supprime également les tables liées)
                    }
                }
            });

            // Fermeture du Dialog
            await RemovingItems.ContinueWith((t, _) => eventArgs.Session.Close(false), null,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }
        #endregion
                #region Ajout
        private async void ExecuteAddShowViewEvenement()
        {
            var Dialog = new AddEvenementDialog(config.sChConn);
            await DialogHost.Show(Dialog, AddShowViewEvenementDialogClosing);
        }
        private async void AddShowViewEvenementDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter == false) return; // Si l'utilisateur à appuyer sur annuler, on arrête là

            eventArgs.Cancel(); // On empêche la fermeture
            AddEvenementDialog dg = (AddEvenementDialog)eventArgs.Session.Content; // On récupère le dialog pour avoir accès à ses données
            eventArgs.Session.UpdateContent(new ProgressDialog()); // On remplace l'ancien dialogue par un nouveau avec une roue de chargement

            Task AddingItems = Task.Run(() => // Lancement d'un thread pour l'ajout de l'élément
            {
                DateTime DateDebut = dg.vm.DateDebut.Add(dg.vm.TimeDebut.TimeOfDay);
                DateTime DateFin = dg.vm.DateFin.Add(dg.vm.TimeFin.TimeOfDay);

                dg.vm.SelectedLieu.Adresse = dg.vm.Adresse;

                // On vérifie les ID, si ils sont à 0 c'est que les items n'existent pas encore dans la DB => création
                if (dg.vm.SelectedTitre.ID == 0)
                    dg.vm.SelectedTitre.ID = new G_TitreEvenement(config.sChConn).Ajouter(dg.vm.SelectedTitre.Titre);
                if (dg.vm.SelectedLieu.ID == 0)
                    dg.vm.SelectedLieu.ID = new G_LieuEvenement(config.sChConn).Ajouter(dg.vm.SelectedLieu.Lieu, dg.vm.SelectedLieu.Adresse);
                lock (EvenementsAffLock) // Verrouillage de l'ObservableCollection pour modif
                {
                    int ID = new G_Evenement(config.sChConn).Ajouter(DateDebut, DateFin, dg.vm.Description, dg.vm.SelectedTypeEvenement, dg.vm.SelectedTitre.ID, dg.vm.SelectedLieu.ID);
                    EvenementsAff.Add(new ShowViewEvenement(new C_ViewEvenement(ID, dg.vm.SelectedTitre.Titre, dg.vm.SelectedLieu.Lieu, dg.vm.SelectedLieu.Adresse, dg.vm.SelectedTypeEvenement, DateDebut, DateFin, dg.vm.Description)));
                }
            });

            // Fermeture du Dialog
            await AddingItems.ContinueWith((t, _) => eventArgs.Session.Close(false), null,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }
        #endregion
                #region Modification
        private async void ExecuteUpdateShowViewEvenement()
        {
            var Dialog = new AddEvenementDialog(config.sChConn, EvenementsAff.First(evenement => evenement.IsSelected)); // On envoie l'événement sélectionné

            await DialogHost.Show(Dialog, UpdateShowViewEvenementClosing);
        }
        private bool CanExecUpdateShowViewEvenement()
        {
            return HowManyShowViewEvenementSelected() == 1;
        }
        private async void UpdateShowViewEvenementClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter == false) return; // Si l'utilisateur à appuyer sur annuler, on arrête là

            eventArgs.Cancel(); // On empêche la fermeture
            AddEvenementDialog dg = (AddEvenementDialog)eventArgs.Session.Content; // On récupère le dialog pour avoir accès à ses données
            eventArgs.Session.UpdateContent(new ProgressDialog()); // On remplace l'ancien dialogue par un nouveau avec une roue de chargement

            Task UpdatingItem = Task.Run(() =>
            {
                DateTime DateDebut = dg.vm.DateDebut.Add(dg.vm.TimeDebut.TimeOfDay);
                DateTime DateFin = dg.vm.DateFin.Add(dg.vm.TimeFin.TimeOfDay);

                dg.vm.SelectedLieu.Adresse = dg.vm.Adresse;

                // On vérifie les ID, si ils sont à 0 c'est que les items n'existent pas encore dans la DB => création
                if (dg.vm.SelectedTitre.ID == 0)
                    dg.vm.SelectedTitre.ID = new G_TitreEvenement(config.sChConn).Ajouter(dg.vm.SelectedTitre.Titre);
                if (dg.vm.SelectedLieu.ID == 0)
                    dg.vm.SelectedLieu.ID = new G_LieuEvenement(config.sChConn).Ajouter(dg.vm.SelectedLieu.Lieu, dg.vm.SelectedLieu.Adresse);
                lock (EvenementsAffLock)
                {
                    new G_Evenement(config.sChConn).Modifier(dg.vm.IDevenement, DateDebut, DateFin, dg.vm.Description, dg.vm.SelectedTypeEvenement, dg.vm.SelectedTitre.ID, dg.vm.SelectedLieu.ID);
                    int Index = EvenementsAff.IndexOf(EvenementsAff.First(item => item.ID == dg.vm.IDevenement));
                    EvenementsAff[Index] = new ShowViewEvenement(new C_ViewEvenement(dg.vm.IDevenement, dg.vm.SelectedTitre.Titre, dg.vm.SelectedLieu.Lieu, dg.vm.SelectedLieu.Adresse, dg.vm.SelectedTypeEvenement, DateDebut, DateFin, dg.vm.Description));
                }
            });

            // Fermeture du Dialog
            await UpdatingItem.ContinueWith((t, _) => eventArgs.Session.Close(false), null,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }
        #endregion
                #region Détails
        private async void ExecuteDetailsShowViewEvenement()
        {
            var Dialog = new DetailsEvenementDialog(EvenementsAff.First(evenement => evenement.IsSelected), config.sChConn);

            await DialogHost.Show(Dialog, DetailsEvenementDialogClosing);
        }
        private void DetailsEvenementDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((int)eventArgs.Parameter == 0) return; // Si l'utilisateur à appuyer sur fermer, on arrête là

            eventArgs.Cancel();
            // Crash si le Dialog n'est pas celui de détails, mais ceux de gestion personnes et photos se ferment avec l'arg à 0, donc ne devraient pas arriver jusqu'ici
            DetailsEvenementDialog dg = (DetailsEvenementDialog)eventArgs.Session.Content; // On récupère le dialog pour avoir accès à ses données

            if ((int)eventArgs.Parameter == 1) // Ouverture de la gestion des participants
            {
                // On récupère l'événement des détails
                eventArgs.Session.UpdateContent(new ManagePartEvenementDialog(dg.vm.Evenement, config.sChConn));
            }
            else if ((int)eventArgs.Parameter == 2) // Ouverture de la gestion des photos
            {
                // On récupère l'événement des détails
                eventArgs.Session.UpdateContent(new ManagePicEvenementDialog(dg.vm.Evenement, config.sChConn));
            }
        }
        private bool CanExecDetailsShowViewEvenement()
        {
            return HowManyShowViewEvenementSelected() == 1;
        }
        #endregion
                #region Copie
        private async void ExecuteCopyShowViewEvenement()
        {
            var Dialog = new ProgressDialog();

            await DialogHost.Show(Dialog, CopyShowViewEvenementDialogOpening);
        }
        // /!\ ON NE COPIE NI LES PHOTOS, NI LES PARTICIPANTS /!\
        private async void CopyShowViewEvenementDialogOpening(object sender, DialogOpenedEventArgs eventArgs)
        {
            Task CopyItems = Task.Run(() =>
            {
                lock (EvenementsAffLock) // Verrou pour opérations cross-thread
                {
                    List<C_TitreEvenement> TmpTitres = new G_TitreEvenement(config.sChConn).Lire("");
                    List<C_LieuEvenement> TmpLieus = new G_LieuEvenement(config.sChConn).Lire("");
                    List<ShowViewEvenement> EvenementsToAdd = new List<ShowViewEvenement>();
                    foreach (ShowViewEvenement evenement in EvenementsAff)
                    {
                        if (evenement.IsSelected) // C'est un événement qui doit être copié
                        {
                            // Récupération des infos
                            C_ViewEvenement TmpEvenement = evenement.GetOriginal();
                            int IDtitre = TmpTitres.First(t => t.Titre == TmpEvenement.Titre).ID;
                            int IDlieu = TmpLieus.First(l => l.Lieu == TmpEvenement.Lieu).ID;

                            // Ajout à la DB
                            int ID = new G_Evenement(config.sChConn).Ajouter(TmpEvenement.DateDebut, TmpEvenement.DateFin, TmpEvenement.Description, TmpEvenement.TypeEvenement, IDtitre, IDlieu);

                            // Ajout dans une liste tampon (on ne peut pas directement ajouter dans la liste principale depuis son foreach)
                            EvenementsToAdd.Add(new ShowViewEvenement(new C_ViewEvenement(ID, TmpEvenement.Titre, TmpEvenement.Lieu, TmpEvenement.Adresse, TmpEvenement.TypeEvenement, TmpEvenement.DateDebut, TmpEvenement.DateFin, TmpEvenement.Description)));
                        }
                    }
                    // Ajout local
                    EvenementsToAdd.ForEach(item => EvenementsAff.Add(item));
                }
            });

            // Fermeture du Dialog
            await CopyItems.ContinueWith((t, _) => eventArgs.Session.Close(false), null,
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
        private bool CanExecDeleteShowPersonne()
        {
            return HowManyShowPersonneSelected() > 0;
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
                        // Suppression des photos sur le disque dur
                        File.Delete(ItemsToRemove[i].Photo);

                        PersonnesAff.Remove(ItemsToRemove[i]); // Retiré localement
                        new G_Personne(config.sChConn).Supprimer(ItemsToRemove[i].ID); // Retiré dans la DB (supprime également les tables liées)
                    }
                }
            });

            // Fermeture du Dialog
            await RemovingItems.ContinueWith((t, _) => eventArgs.Session.Close(false), null,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }
        #endregion
                #region Ajout
        private async void ExecuteAddShowPersonne()
        {
            var Dialog = new AddPersonneDialog();
            await DialogHost.Show(Dialog, AddShowPersonneDialogClosing);
        }
        private async void AddShowPersonneDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter == false) return; // Si l'utilisateur à appuyer sur annuler, on arrête là

            eventArgs.Cancel(); // On empêche la fermeture
            AddPersonneDialog dg = (AddPersonneDialog)eventArgs.Session.Content; // On récupère le dialog pour avoir accès à ses données
            eventArgs.Session.UpdateContent(new ProgressDialog()); // On remplace l'ancien dialogue par un nouveau avec une roue de chargement

            Task AddingItems = Task.Run(() => // Lancement d'un thread pour l'ajout de l'élément
            {
                string path;
                if (!string.IsNullOrWhiteSpace(dg.vm.PicFullPath))
                {
                    // Sauvegarde de la photo dans le dossier "~\Images\Personnes\"
                    string FileName = Path.GetFileName(dg.vm.PicFullPath); // On récupère uniquement le nom du fichier et son extension du chemin entré dans le dialog
                    path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\Images\\Personnes"); // On génère le chemin du dossier "~\Images\Personnes\"
                    Directory.CreateDirectory(path); // Si les dossiers n'existent pas encore, ils sont créés
                    path = Path.Combine(path, FileName); // On rajoute le nom du fichier au path
                    // Vérification qu'un fichier du même nom n'existe pas déjà
                    string TestPath = path;
                    int Count = 0;
                    while (File.Exists(TestPath))
                    {
                        string tempFileName = string.Format("{0}({1})", Path.GetFileNameWithoutExtension(path), Count++);
                        TestPath = Path.Combine(Path.GetDirectoryName(path), tempFileName + Path.GetExtension(path));
                    }
                    path = TestPath;
                    File.Copy(dg.vm.PicFullPath, path); // Et on copie le fichier sélectionné dans "~\Images\Personnes\"
                }
                else // On a pas rentré de photo
                {
                    path = "NoPic";
                }

                lock (PersonnesAffLock)
                {
                    int ID = new G_Personne(config.sChConn).Ajouter(dg.vm.Nom, dg.vm.Prenom, dg.vm.Date, path, dg.vm.SelectedRole);
                    PersonnesAff.Add(new ShowPersonne(new C_Personne(ID, dg.vm.Nom, dg.vm.Prenom, dg.vm.Date, path, dg.vm.SelectedRole)));
                }
            });

            // Fermeture du Dialog
            await AddingItems.ContinueWith((t, _) => eventArgs.Session.Close(false), null,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }
        #endregion
                #region Modification
        private async void ExecuteUpdateShowPersonne()
        {
            var Dialog = new AddPersonneDialog(PersonnesAff.First(personne => personne.IsSelected));

            await DialogHost.Show(Dialog, UpdateShowPersonneDialogClosing);
        }
        private async void UpdateShowPersonneDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter == false) return; // Si l'utilisateur à appuyer sur annuler, on arrête là

            eventArgs.Cancel(); // On empêche la fermeture
            AddPersonneDialog dg = (AddPersonneDialog)eventArgs.Session.Content; // On récupère le dialog pour avoir accès à ses données
            eventArgs.Session.UpdateContent(new ProgressDialog()); // On remplace l'ancien dialogue par un nouveau avec une roue de chargement

            Task UpdatingItem = Task.Run(() =>
            {
                string path;
                if (dg.vm.PicFullPath != null) // On veut changer la photo de profil
                {
                    // Sauvegarde de la photo dans le dossier "~\Resources\Images\Personnes\"
                    string FileName = Path.GetFileName(dg.vm.PicFullPath); // On récupère uniquement le nom du fichier et son extension du chemin entré dans le dialog
                    path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\Images\\Personnes"); // On génère le chemin du dossier "~\Images\Personnes\"
                    Directory.CreateDirectory(path); // Si les dossiers n'existent pas encore, ils sont créés
                    path = Path.Combine(path, FileName); // On rajoute le nom du fichier au path
                    // Vérification qu'un fichier du même nom n'existe pas déjà
                    string TestPath = path;
                    int Count = 0;
                    while (File.Exists(TestPath))
                    {
                        string tempFileName = string.Format("{0}({1})", Path.GetFileNameWithoutExtension(path), Count++);
                        TestPath = Path.Combine(Path.GetDirectoryName(path), tempFileName + Path.GetExtension(path));
                    }
                    path = TestPath;
                    File.Copy(dg.vm.PicFullPath, path); // Et on copie le fichier sélectionné dans "~\Resources\Images\Personnes\"

                    try
                    {
                        // Suppression de l'ancienne photo
                        File.Delete(dg.vm.OldPicPath);
                    }
                    catch { }
                }
                else
                    path = dg.vm.OldPicPath;

                lock (PersonnesAffLock)
                {
                    new G_Personne(config.sChConn).Modifier(dg.vm.IDpersonne, dg.vm.Nom, dg.vm.Prenom, dg.vm.Date, path, dg.vm.SelectedRole);
                    int Index = PersonnesAff.IndexOf(PersonnesAff.First(item => item.ID == dg.vm.IDpersonne));
                    PersonnesAff[Index] = new ShowPersonne(new C_Personne(dg.vm.IDpersonne, dg.vm.Nom, dg.vm.Prenom, dg.vm.Date, path, dg.vm.SelectedRole));
                }
            });

            // Fermeture du Dialog
            await UpdatingItem.ContinueWith((t, _) => eventArgs.Session.Close(false), null,
                    TaskScheduler.FromCurrentSynchronizationContext());
        }
        private bool CanExecUpdateShowPersonne()
        {
            return HowManyShowPersonneSelected() == 1;
        }
        #endregion
                #region Détails
        private async void ExecuteDetailsShowPersonne()
        {
            var Dialog = new DetailsPersonneDialog(PersonnesAff.First(personne => personne.IsSelected));

            await DialogHost.Show(Dialog);
        }

        private bool CanExecDetailsShowPersonne()
        {
            return HowManyShowPersonneSelected() == 1;
        }
        #endregion
        #endregion
        #region Autres
        private async void ExecuteManagePlats()
        {
            var Dialog = new ManagePlatsDialog(config.sChConn);

            await DialogHost.Show(Dialog);
        }
        private async void ExecuteManagePartEvenement()
        {
            var Dialog = new ManagePartEvenementDialog(EvenementsAff.First(item => item.IsSelected), config.sChConn);

            await DialogHost.Show(Dialog);
        }
        private async void ExecuteManagePicEvenement()
        {
            var Dialog = new ManagePicEvenementDialog(EvenementsAff.First(item => item.IsSelected), config.sChConn);

            await DialogHost.Show(Dialog);
        }
        private async void ExecuteManageTitreLieu()
        {
            var Dialog = new ManageTitreLieuDialog(config.sChConn);

            await DialogHost.Show(Dialog);
        }
        #endregion
        #endregion
        #endregion

        #region Accesseurs
        public Action CloseAction { get; set; } // Permet de fermer la fenêtre depuis le ViewModel; solution de -> http://jkshay.com/closing-a-wpf-window-using-mvvm-and-minimal-code-behind/
        public ObservableCollection<ShowViewMenuDuJour> MenusAff
        {
            get => _MenusAff;
            set
            {
                if (_MenusAff != value)
                {
                    _MenusAff = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<ShowViewEvenement> EvenementsAff
        {
            get => _EvenementsAff;
            set
            {
                if (_EvenementsAff != value)
                {
                    _EvenementsAff = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<ShowPersonne> PersonnesAff
        {
            get => _PersonnesAff;
            set
            {
                if (_PersonnesAff != value)
                {
                    _PersonnesAff = value;
                    OnPropertyChanged();
                }
            }
        }
        #region Sélection DG
        public bool IsAllItemsEvenementsSelected
        {
            get { return _IsAllItemsEvenementsSelected; }
            set
            {
                if (this._IsAllItemsEvenementsSelected != value) // On (dé)sélectionne tous les éléments de la DG selon la valeur de la checkbox du header
                {
                    this._IsAllItemsEvenementsSelected = value;
                    SelectAllShowViewEvenement(value);
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
                }
            }
        }
        #endregion
        #region Commands
            #region App
        public ICommand OpenAffCmd
        {
            get
            {
                return _OpenAffCmd;
            }
            set
            {
                _OpenAffCmd = value;
            }
        }
        public ICommand CloseAppCmd
        {
            get
            {
                return _CloseAppCmd;
            }
            set
            {
                _CloseAppCmd = value;
            }
        }
        #endregion
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
            #region Ajout
        public ICommand AddShowViewMenuDuJourCmd
        {
            get
            {
                return _AddShowViewMenuDuJourCmd;
            }
            set
            {
                _AddShowViewMenuDuJourCmd = value;
            }
        }
        public ICommand AddShowPersonneCmd
        {
            get
            {
                return _AddShowPersonneCmd;
            }
            set
            {
                _AddShowPersonneCmd = value;
            }
        }
        public ICommand AddShowViewEvenementCmd
        {
            get
            {
                return _AddShowViewEvenementCmd;
            }
            set
            {
                _AddShowViewEvenementCmd = value;
            }
        }
        #endregion
            #region Modification
        public ICommand UpdateShowViewMenuDuJourCmd
        {
            get
            {
                return _UpdateShowViewMenuDuJourCmd;
            }
            set
            {
                _UpdateShowViewMenuDuJourCmd = value;
            }
        }
        public ICommand UpdateShowViewEvenementCmd
        {
            get
            {
                return _UpdateShowViewEvenementCmd;
            }
            set
            {
                _UpdateShowViewEvenementCmd = value;
            }
        }
        public ICommand UpdateShowPersonneCmd
        {
            get
            {
                return _UpdateShowPersonneCmd;
            }
            set
            {
                _UpdateShowPersonneCmd = value;
            }
        }
        #endregion
            #region Détails
        public ICommand DetailsShowViewMenuDuJourCmd
        {
            get
            {
                return _DetailsShowViewMenuDuJourCmd;
            }
            set
            {
                _DetailsShowViewMenuDuJourCmd = value;
            }
        }
        public ICommand DetailsShowViewEvenementCmd
        {
            get
            {
                return _DetailsShowViewEvenementCmd;
            }
            set
            {
                _DetailsShowViewEvenementCmd = value;
            }
        }
        public ICommand DetailsShowPersonneCmd
        {
            get
            {
                return _DetailsShowPersonneCmd;
            }
            set
            {
                _DetailsShowPersonneCmd = value;
            }
        }
        #endregion
            #region Copie
        public ICommand CopyShowViewEvenementCmd { get => _CopyShowViewEvenementCmd; set => _CopyShowViewEvenementCmd = value; }
        public ICommand CopyShowViewMenuDuJourCmd { get => _CopyShowViewMenuDuJourCmd; set => _CopyShowViewMenuDuJourCmd = value; }
        #endregion
            #region Autres
        public ICommand ManagePlatsCmd
        {
            get
            {
                return _ManagePlatsCmd;
            }
            set
            {
                _ManagePlatsCmd = value;
            }
        }

        public ICommand ManagePartEvenementCmd { get => _ManagePartEvenementCmd; set => _ManagePartEvenementCmd = value; }
        public ICommand ManagePicEvenementCmd { get => _ManagePicEvenementCmd; set => _ManagePicEvenementCmd = value; }
        public ICommand ManageTitreLieuCmd { get => _ManageTitreLieuCmd; set => _ManageTitreLieuCmd = value; }
        #endregion
        #endregion
        #endregion
    }
}
