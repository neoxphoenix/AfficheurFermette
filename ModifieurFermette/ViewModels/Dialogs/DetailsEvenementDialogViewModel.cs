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
using System.Windows.Media.Imaging;

namespace ModifieurFermette.ViewModels.Dialogs
{
    public class DetailsEvenementDialogViewModel : ObservableData
    {
        public ShowViewEvenement Evenement;
        private int _ID;
        private string _Titre, _Lieu, _DateDebut, _DateFin, _Description, _Type;
        private ObservableCollection<C_PersonnePos> _Classement;

        private ICommand _NextPicCmd, _PrevPicCmd;
        private readonly List<C_PhotoEvenement> PicsPaths;
        private BitmapImage _Photo;
        private int _IndexPhoto;
        private string _CountPicTxt;

        public DetailsEvenementDialogViewModel(ShowViewEvenement evenement, string sChConn)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait; // Curseur d'attente
            this.Evenement = evenement;

            ID = evenement.ID;
            Titre = evenement.Titre;
            Lieu = evenement.Lieu;
            DateDebut = evenement.DateDebut.ToShortDateString() + " " + evenement.DateDebut.ToShortTimeString();
            DateFin = evenement.DateFin.ToShortDateString() + " " + evenement.DateFin.ToShortTimeString();
            Description = evenement.Description;
            Type = evenement.TypeEvenement;

            // Chargement du classement
            if (Type == "compétition")
            {
                Classement = new ObservableCollection<C_PersonnePos>(new G_ViewEvenement(sChConn).LireClassementEvenement(evenement.ID));
            }
            else // atelier ou divers => pas de position
            {
                Classement = new ObservableCollection<C_PersonnePos>();
                new G_ViewEvenement(sChConn).LirePersonnesEvenement(evenement.ID).ForEach
                (
                    item => Classement.Add(new C_PersonnePos(item.ID, item.Nom, item.Prenom, item.DateNaissance, item.Photo, item.Role, 0))
                );
            }

            // Chargement des photos
            PicsPaths = new G_ViewEvenement(sChConn).LirePhotosEvenement(evenement.ID);
            if (PicsPaths.Count > 0)
            {
                Photo = new BitmapImage(new Uri(PicsPaths.First().Photo)); // On charge la première photo
            }
            else // Pas de photo
                Photo = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + @"Resources\Images\errorimg.png"));
            IndexPhoto = 0; // Index de la photo

            // Création des commandes
            NextPicCmd = new RelayCommand(Exec => ExecuteNextPic(), CanExec => CanExecNextPic());
            PrevPicCmd = new RelayCommand(Exec => ExecutePrevPic(), CanExec => CanExecPrevPic());

            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow; // Retour au curseur normal
        }

        private bool CanExecPrevPic()
        {
            return this.IndexPhoto > 0;
        }

        private void ExecutePrevPic()
        {
            this.IndexPhoto--;
            Photo = new BitmapImage(new Uri(PicsPaths[this.IndexPhoto].Photo));
        }

        private bool CanExecNextPic()
        {
            return this.IndexPhoto < (PicsPaths.Count - 1);
        }

        private void ExecuteNextPic()
        {
            this.IndexPhoto++;
            Photo = new BitmapImage(new Uri(PicsPaths[this.IndexPhoto].Photo));
        }

        #region Accesseurs
        public int ID
        {
            get => _ID;
            set
            {
                if (_ID != value)
                {
                    _ID = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Titre
        {
            get => _Titre;
            set
            {
                if (_Titre != value)
                {
                    _Titre = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Lieu
        {
            get => _Lieu;
            set
            {
                if (_Lieu != value)
                {
                    _Lieu = value;
                    OnPropertyChanged();
                }
            }
        }
        public string DateDebut
        {
            get => _DateDebut;
            set
            {
                if (_DateDebut != value)
                {
                    _DateDebut = value;
                    OnPropertyChanged();
                }
            }
        }
        public string DateFin
        {
            get => _DateFin;
            set
            {
                if (_DateFin != value)
                {
                    _DateFin = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Description
        {
            get => _Description;
            set
            {
                if (_Description != value)
                {
                    _Description = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Type
        {
            get => _Type;
            set
            {
                if (_Type != value)
                {
                    _Type = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<C_PersonnePos> Classement
        {
            get => _Classement;
            set
            {
                if (_Classement != value)
                {
                    _Classement = value;
                    OnPropertyChanged();
                }
            }
        }

        public BitmapImage Photo
        {
            get => _Photo;
            set
            {
                if (_Photo != value)
                {
                    _Photo = value;
                    OnPropertyChanged();
                }
            }
        }

        public int IndexPhoto
        {
            get => _IndexPhoto;
            set
            {
                _IndexPhoto = value;
                OnPropertyChanged();
                if (PicsPaths.Count > 0)
                    CountPicTxt = (value + 1) + "/" + PicsPaths.Count;
                else
                    CountPicTxt = value + "/" + PicsPaths.Count;
            }
        }
        public string CountPicTxt
        {
            get => _CountPicTxt;
            set
            {
                if (_CountPicTxt != value)
                {
                    _CountPicTxt = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand NextPicCmd { get => _NextPicCmd; set => _NextPicCmd = value; }
        public ICommand PrevPicCmd { get => _PrevPicCmd; set => _PrevPicCmd = value; }
        #endregion
    }
}
