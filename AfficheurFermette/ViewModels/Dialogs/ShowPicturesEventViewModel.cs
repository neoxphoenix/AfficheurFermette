using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projet_AFFICHEURFERMETTE.MDF.Classes;
using Projet_AFFICHEURFERMETTE.MDF.Gestion;
using ShowableData;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using System.Windows.Data;
using System.Windows.Input;
using Projet_AFFICHEURFERMETTE.MDF.Acces;
using System.Windows;
using MaterialDesignThemes.Wpf;
using System.Globalization;
using System.Windows.Media.Imaging;
using System.IO;

namespace AfficheurFermette.ViewModels.Dialogs
{
    class ShowPicturesEventViewModel : ObservableData, INotifyPropertyChanged
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ATTRIBUTS + ACCESSEURS
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        public string dossierResources = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Resources\Images\";

        public List<C_PhotoEvenement> ListePhotosEvenements { get; set; }
        public List<C_PersonnePos> GetEvenements { get; set; }

        public int maxPic; //nombre max de photos dispo

        private int _indexPic; //index de la photo actuel
        public int indexPic
        {
            get { return _indexPic; }
            set
            {
                if (_indexPic != value)
                {
                    _indexPic = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _infosPicPos; //indique combien et sur quel photos l'on se trouve
        public string infosPicPos
        {
            get { return _infosPicPos; }
            set
            {
                if (_infosPicPos != value)
                {
                    _infosPicPos = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _titreEvent;
        public string titreEvent
        {
            get { return _titreEvent; }
            set
            {
                if (_titreEvent != value)
                {
                    _titreEvent = value;
                    OnPropertyChanged();
                }
            }
        }

        private BitmapImage _photoToDisplay; //Bitmap de l'image à afficher
        public BitmapImage photoToDisplay
        {
            get { return _photoToDisplay; }
            set
            {
                if (_photoToDisplay != value)
                {
                    _photoToDisplay = value;
                    OnPropertyChanged();
                }
            }
        }

        private Visibility _AfficherBtnPrev; //visibilité ON ou OFF
        public Visibility AfficherBtnPrev
        {
            get
            {
                return _AfficherBtnPrev;
            }
            set
            {
                _AfficherBtnPrev = value;
                OnPropertyChanged();
            }
        }

        private Visibility _AfficherBtnNext;
        public Visibility AfficherBtnNext
        {
            get
            {
                return _AfficherBtnNext;
            }
            set
            {
                _AfficherBtnNext = value;
                OnPropertyChanged();
            }
        }

        private Visibility _AfficherClassementDG;
        public Visibility AfficherClassementDG
        {
            get
            {
                return _AfficherClassementDG;
            }
            set
            {
                _AfficherClassementDG = value;
                OnPropertyChanged();
            }
        }

        //<<< ICOMMAND >>>
        private ICommand _DisplayNextPic;
        public ICommand Cmd_DisplayNextPic
        {
            get { return _DisplayNextPic; }
            set
            {
                if (_DisplayNextPic != value)
                    _DisplayNextPic = value;
            }
        }
        //CanChange
        private bool ICmd_CanDisplayNextPic(object o)
        {
            return true;
        }

        //Exec
        private void ICmd_ExecDisplayNextPic()
        {
            if (indexPic < maxPic - 1)
            {
                indexPic++; //incrémente l'index
                infosPicPos = (indexPic + 1).ToString() + "/" + maxPic.ToString();

                if (File.Exists(ListePhotosEvenements[0].Photo))
                    photoToDisplay = new BitmapImage(new Uri(ListePhotosEvenements[indexPic].Photo));
                else
                    photoToDisplay = new BitmapImage(new Uri(dossierResources + "errorimg.png"));
            }
        }

        private ICommand _DisplayPrevPic;
        public ICommand Cmd_DisplayPrevPic
        {
            get { return _DisplayPrevPic; }
            set
            {
                if (_DisplayPrevPic != value)
                    _DisplayPrevPic = value;
            }
        }
        //CanChange
        private bool ICmd_CanDisplayPrevPic(object o)
        {
            return true;
        }

        //Exec
        private void ICmd_ExecDisplayPrevPic()
        {
            if (indexPic > 0)
            {
                indexPic--; //décrémente l'index
                infosPicPos = (indexPic + 1).ToString() + "/" + maxPic.ToString();

                if (File.Exists(ListePhotosEvenements[0].Photo))
                    photoToDisplay = new BitmapImage(new Uri(ListePhotosEvenements[indexPic].Photo));
                else
                    photoToDisplay = new BitmapImage(new Uri(dossierResources + "errorimg.png"));
            }
        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////
        /// CONSTRUCTEUR
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public ShowPicturesEventViewModel(string sChConn, ShowViewEvenement EventSelected)
        {
            Cmd_DisplayNextPic = new RelayCommand(Exec => ICmd_ExecDisplayNextPic(), CanExec => true);
            Cmd_DisplayPrevPic = new RelayCommand(Exec => ICmd_ExecDisplayPrevPic(), CanExec => true);

            titreEvent = EventSelected.Titre; //configure le titre de la fenetre
            
            //Photos
            ListePhotosEvenements = new G_ViewEvenement(sChConn).LirePhotosEvenement(EventSelected.ID);
            if (ListePhotosEvenements.Count() > 0)
            {
                AfficherBtnPrev = Visibility.Visible;
                AfficherBtnNext = Visibility.Visible;

                if (File.Exists(ListePhotosEvenements[0].Photo))
                    photoToDisplay = new BitmapImage(new Uri(ListePhotosEvenements[0].Photo));
                else
                    photoToDisplay = new BitmapImage(new Uri(dossierResources + "errorimg.png"));

                indexPic = 0;
                maxPic = ListePhotosEvenements.Count();
                infosPicPos = (indexPic+1).ToString() + "/" + maxPic.ToString();
            }
            else
            {
                AfficherBtnPrev = Visibility.Hidden;
                AfficherBtnNext = Visibility.Hidden;
            }

            //Classement
            GetEvenements = new G_ViewEvenement(sChConn).LireClassementEvenement(EventSelected.ID);
            if (GetEvenements.Count() > 0)
                AfficherClassementDG = Visibility.Visible;
            else
                AfficherClassementDG = Visibility.Collapsed;
        }

        //EVENT HANDLER qui signal un changement d'état
        public new event PropertyChangedEventHandler PropertyChanged;
        private new void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

