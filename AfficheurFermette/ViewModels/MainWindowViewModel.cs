using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Projet_AFFICHEURFERMETTE.MDF.Acces;
using Projet_AFFICHEURFERMETTE.MDF.Classes;
using Projet_AFFICHEURFERMETTE.MDF.Gestion;
using System.ComponentModel;
using System.Collections.ObjectModel; //pour ObservableData
using System.Runtime.CompilerServices;
using System.Windows;
using ShowableData; //afin d'hériter de ObservableData

namespace AfficheurFermette.ViewModels
{
    class MainWindowViewModel : ObservableData, INotifyPropertyChanged
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ATTRIBUTS
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        public ConfigClass config;

        private ICommand _ChangeTabByClick;
        private int _OngletChoisi = 0;
        private string _ongletActuelDesc = "Repas du jour"; //Chaine de texte (description) de l'onglet actuel
        private string _repasEntreeDuJour, _repasPlatDuJour, _repasDessertDuJour;
        private string _dateAjd; //stocke la date du jour
        private string _aSonAnnifAjd = "";
        private string[] _prochainEvent1, _prochainEvent2, _prochainEvent3;

        private string _test;
        public List<ShowViewEvenement> ProchainEvenements = new List<ShowViewEvenement>();

        

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        /// CONSTRUCTEUR
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        public MainWindowViewModel()
        {
            ChangeTabByClick = new RelayCommand(ICmd_ExecChangeTabByClick, ICmd_CanChangeTabByClick);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ACCESSEURS
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //Get-Set choix onglets
        public int OngletChoisi
        {
            get { return _OngletChoisi; }
            set
            {
                if (_OngletChoisi != value)
                {
                    _OngletChoisi = value;
                    switch(_OngletChoisi)
                    {
                        case 0: ongletActuelDesc = "Repas du jour"; break;
                        case 1: ongletActuelDesc = "Actualités"; break;
                        case 2: ongletActuelDesc = "Ateliers"; break;
                        case 3: ongletActuelDesc = "Educateurs présent aujourd'hui"; break;
                        default: ongletActuelDesc = "Repas du jour"; break;
                    }
                    OnPropertyChanged(); //Signal le changement
                }
            }
        }

        //Get-Set choix onglets
        public string ongletActuelDesc
        {
            get { return _ongletActuelDesc; }
            set
            {
                if (_ongletActuelDesc != value)
                    _ongletActuelDesc = value;
                OnPropertyChanged();
            }
        }

        #region RelayCmd + ACCESSEURS

         //RelayCmd lors du clic sur un onglet
        public ICommand ChangeTabByClick
        {
            get { return _ChangeTabByClick; }
            set
            {
                if (_ChangeTabByClick != value)
                    _ChangeTabByClick = value;
            }
        }

        public string test
        {
            get
            {
                return _test;
            }
            set
            {
                _test = value;
            }
        }


        public string repasEntreeDuJour
        {
            get { return _repasEntreeDuJour; }
            set
            {
                if (_repasEntreeDuJour != value)
                    _repasEntreeDuJour = value;
                OnPropertyChanged();
            }
        }
        public string repasPlatDuJour
        {
            get { return _repasPlatDuJour; }
            set
            {
                if (_repasPlatDuJour != value)
                    _repasPlatDuJour = value;
                OnPropertyChanged();
            }
        }

        public string repasDessertDuJour
        {
            get { return _repasDessertDuJour; }
            set
            {
                if (_repasDessertDuJour != value)
                    _repasDessertDuJour = value;
                OnPropertyChanged();
            }
        }
        public string dateAjd
        {
            get { return DateTime.Now.ToString("dd/MM/yy"); }
            set
            {
                _dateAjd = DateTime.Now.ToString("dd/MM/yy");
                OnPropertyChanged();
            }
        }
        
        //retourne un string contenant les personnes ayant leur anniversaire ajd
        public string aSonAnnifAjd
        {
           get { return _aSonAnnifAjd; }
           set
           {
                if (_aSonAnnifAjd != value)
                    _aSonAnnifAjd = value;
                OnPropertyChanged();
           }
        }

        public string[] prochainEvent1
        {
            get { return _prochainEvent1; }
            set {
                if (_prochainEvent1 != value)
                    _prochainEvent1 = value;
                OnPropertyChanged();
            }
        }

        //CanChange
        private bool ICmd_CanChangeTabByClick(object o)
        {
            return true;
        }

        //Exec
        private void ICmd_ExecChangeTabByClick(object param)
        {
            OngletChoisi = Convert.ToInt32(param); //récupère l'argument et le converti en int
        }
        #endregion



        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //FONCTIONS
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        //EVENT HANDLER qui signal un changement d'état
        public new event PropertyChangedEventHandler PropertyChanged;
        private new void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // CHARGER DONNNES DU JOUR
        public void ChargerDonneesDaily()
        {
            GetMenuDuJour();
            CheckAnnifToday();
            GenerationDesEvenementsExistant();
        }

        //Menu du jour
        public void GetMenuDuJour()
        {
            C_ViewMenuDuJour MenuDuJour = new G_ViewMenuDuJour(config.sChConn).Lire_Date(DateTime.Now);
            if (MenuDuJour.eNom != null)
            {
                repasEntreeDuJour = MenuDuJour.eNom.ToString();
                repasPlatDuJour = MenuDuJour.pNom.ToString();
                repasDessertDuJour = MenuDuJour.dNom.ToString();
            }
            else
            {
                repasPlatDuJour = "Aucune donnée en ce jour";
            }
        }
        //Check Anniversaire du jour
        public void CheckAnnifToday()
        {
            string DateAjd = DateTime.Now.ToString("dd/MM");
            string TmpaSonAnnifAjd = "";
            string separateur = "";
            int i = 0;
            List<C_Personne> AnniversaireDuJour2 = new G_Personne(config.sChConn).Lire("");
            foreach (C_Personne aSonAnnif in AnniversaireDuJour2)
            {
                string _annif = aSonAnnif.DateNaissance.ToString().Substring(0, 5);
                if (i > 0)
                    separateur = " et "; //rajoute un séparateur si + d'une personne on leur annif
                if (string.Compare(_annif, DateAjd) == 0)
                {
                    TmpaSonAnnifAjd += separateur + aSonAnnif.Nom + " " + aSonAnnif.Prenom; //détecte si quelqu'un à son annif
                    i++;
                }
            }
            aSonAnnifAjd = TmpaSonAnnifAjd;
        }

        public void GenerationDesEvenementsExistant()
        {
            List<C_ViewEvenement> Evenements = new G_ViewEvenement(config.sChConn).Lire("");
            int nbreEvent = 0;
            foreach (C_ViewEvenement TmpEvent in Evenements)
            {
                if (DateTime.Compare(TmpEvent.DateDebut,DateTime.Now) > 0)
                {
                    ProchainEvenements.Add(new ShowViewEvenement(TmpEvent));
                    nbreEvent++;
                }
            }
            //for (int i=0;i<=nbreEvent;i++)
            //{
            //    _prochainEvent1 = new string[] { ProchainEvenements[i].Titre, ProchainEvenements[i].Lieu };
            //}

        }

        public string[] Food
        {
            get
            {
                return new string[] { "liver", "spam", "cake", "garlic" };
            }
        }
    }


}
