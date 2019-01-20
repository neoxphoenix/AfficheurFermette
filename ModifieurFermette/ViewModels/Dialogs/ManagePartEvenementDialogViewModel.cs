using Projet_AFFICHEURFERMETTE.MDF.Classes;
using Projet_AFFICHEURFERMETTE.MDF.Gestion;
using ShowableData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModifieurFermette.ViewModels.Dialogs
{
    class ManagePartEvenementDialogViewModel : ObservableData
    {
        // === Liste des participants ===
        private ObservableCollection<ShowPersonne> _Personnes;
        private double _PositionWidth; // gère la largeur de la colonne positioin => on met à 0 pour la cacher

        // === Ajout d'une personne ===
        private ObservableCollection<ShowPersonne> _PersToAdd; // liste des personnes qui ne sont pas encore dans l'événement
        private ShowPersonne _SelectedPersToAdd;
        private bool _IsCompet; // vrai si l'événement est une compétition
        private int _PosPersToAdd; // Position de la personne qu'on va ajouter
        private ICommand _AddPersCmd;

        // === Retrait d'une personne ===
        // La combobox utilise la liste Personnes vu qu'on y met que les participants
        private ShowPersonne _SelectedPersToRemove;
        private ICommand _RemovePersCmd;

        // === Manipulation DB ===
        private readonly string sChConn;
        private readonly int IDevenement;
        private List<C_Classement> T_Classements; // Permet de ne charger qu'une fois toutes les tables de liaisons => sert à la suppression
        private List<C_PersonneConcernees> T_PersonnesConcernees; // idem

        public ManagePartEvenementDialogViewModel(ShowViewEvenement evenement, string sChConn)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait; // Curseur d'attente

            this.sChConn = sChConn;
            this.IDevenement = evenement.ID;
            IsCompet = (evenement.TypeEvenement == "compétition");
            Personnes = new ObservableCollection<ShowPersonne>();
            // On charge toutes les personnes
            PersToAdd = new ObservableCollection<ShowPersonne>();
            new G_Personne(sChConn).Lire("").ForEach(item => PersToAdd.Add(new ShowPersonne(item)));

            // Gestion des listes de personnes
            if (IsCompet)
            {
                T_Classements = new G_Classement(sChConn).Lire("");
                List<C_PersonnePos> TmpPers = new G_ViewEvenement(sChConn).LireClassementEvenement(evenement.ID);
                foreach (C_PersonnePos pers in TmpPers)
                {
                    // 1) On ajoute les personnes participantes à la liste du Dialog
                    Personnes.Add(new ShowPersonne(pers));
                    // 2) On retire de la liste qui contient toutes les pers de la DB celles qui participent à l'événement => On a plus que celles qui peuvent être ajoutées
                    PersToAdd.Remove(PersToAdd.First(item => item.ID == Personnes.Last().ID));
                }
            }
            else
            {
                T_PersonnesConcernees = new G_PersonneConcernees(sChConn).Lire("");
                List<C_Personne> TmpPers = new G_ViewEvenement(sChConn).LirePersonnesEvenement(evenement.ID);
                foreach (C_Personne pers in TmpPers)
                {
                    // 1) On ajoute les personnes participantes à la liste du Dialog
                    Personnes.Add(new ShowPersonne(pers));
                    // 2) On retire de la liste qui contient toutes les pers de la DB celles qui participent à l'événement => On a plus que celles qui peuvent être ajoutées
                    //PersToAdd.Remove(Personnes.Last()); // Ne fonctionne pas ?
                    PersToAdd.Remove(PersToAdd.First(item => item.ID == Personnes.Last().ID));
                }
            }

            AddPersCmd = new RelayCommand(Exec => ExecuteAddPers(), CanExec => CanExecAddPers());
            RemovePersCmd = new RelayCommand(Exec => ExecuteRemovePers(), CanExec => CanExecRemovePers());

            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow; // Retour au curseur normal
        }

        private bool CanExecRemovePers()
        {
            return SelectedPersToRemove != null;
        }

        private void ExecuteRemovePers()
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait; // Curseur d'attente

            // MàJ DB
            if (IsCompet)
            {
                // idéalement devrait utiliser une procédure stockée, mais plus le temps
                C_Classement TmpClass = T_Classements.First(item => item.IDevenement == this.IDevenement && item.IDpersonne == SelectedPersToRemove.ID);
                new G_Classement(sChConn).Supprimer(TmpClass.ID);
                T_Classements.Remove(TmpClass);
            }
            else
            {
                C_PersonneConcernees TmpPersConc = T_PersonnesConcernees.First(item => item.IDevenement == this.IDevenement && item.IDpersonne == SelectedPersToRemove.ID);
                new G_PersonneConcernees(sChConn).Supprimer(TmpPersConc.ID);
                T_PersonnesConcernees.Remove(TmpPersConc);
            }

            // MàJ locale
            PersToAdd.Add(SelectedPersToRemove);
            Personnes.Remove(SelectedPersToRemove);
            SelectedPersToAdd = null;
            SelectedPersToRemove = null;

            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow; // Retour au curseur normal
        }

        private bool CanExecAddPers()
        {
            return SelectedPersToAdd != null;
        }

        private void ExecuteAddPers()
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait; // Curseur d'attente

            // MàJ DB
            if (IsCompet)
            {
                int ID = new G_Classement(sChConn).Ajouter(this.IDevenement, SelectedPersToAdd.ID, PosPersToAdd);
                T_Classements.Add(new C_Classement(ID, this.IDevenement, SelectedPersToAdd.ID, PosPersToAdd));
                SelectedPersToAdd.Position = PosPersToAdd;
            }
            else
            {
                int ID = new G_PersonneConcernees(sChConn).Ajouter(this.IDevenement, SelectedPersToAdd.ID);
                T_PersonnesConcernees.Add(new C_PersonneConcernees(ID, this.IDevenement, SelectedPersToAdd.ID));
            }

            // MàJ locale
            Personnes.Add(SelectedPersToAdd);
            PersToAdd.Remove(SelectedPersToAdd);
            SelectedPersToAdd = null;
            SelectedPersToRemove = null;

            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow; // Retour au curseur normal
        }

        #region Accesseurs
        public ObservableCollection<ShowPersonne> Personnes
        {
            get => _Personnes;
            set
            {
                if (_Personnes != value)
                {
                    _Personnes = value;
                    OnPropertyChanged();
                }
            }
        }
        public double PositionWidth
        {
            get => _PositionWidth;
            set
            {
                if (_PositionWidth != value)
                {
                    _PositionWidth = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<ShowPersonne> PersToAdd
        {
            get => _PersToAdd;
            set
            {
                if (_PersToAdd != value)
                {
                    _PersToAdd = value;
                    OnPropertyChanged();
                }
            }
        }
        public ShowPersonne SelectedPersToAdd
        {
            get => _SelectedPersToAdd;
            set
            {
                if (_SelectedPersToAdd != value)
                {
                    _SelectedPersToAdd = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsCompet
        {
            get => _IsCompet;
            set
            {
                if (_IsCompet != value)
                {
                    _IsCompet = value;
                    OnPropertyChanged();
                    if (!value) // On cache la colonne position si ce n'est pas une compétition
                        PositionWidth = 0;
                    else
                        PositionWidth = 80;
                }
            }
        }
        public int PosPersToAdd
        {
            get => _PosPersToAdd;
            set
            {
                if (_PosPersToAdd != value)
                {
                    _PosPersToAdd = value;
                    OnPropertyChanged();
                }
            }
        }
        public ICommand AddPersCmd { get => _AddPersCmd; set => _AddPersCmd = value; }

        public ShowPersonne SelectedPersToRemove
        {
            get => _SelectedPersToRemove;
            set
            {
                if (_SelectedPersToRemove != value)
                {
                    _SelectedPersToRemove = value;
                    OnPropertyChanged();
                }
            }
        }
        public ICommand RemovePersCmd { get => _RemovePersCmd; set => _RemovePersCmd = value; }
        #endregion
    }
}
