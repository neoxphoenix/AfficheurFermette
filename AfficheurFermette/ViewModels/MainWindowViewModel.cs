using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Projet_AFFICHEURFERMETTE.MDF.Acces;
using Projet_AFFICHEURFERMETTE.MDF.Classes;
using Projet_AFFICHEURFERMETTE.MDF.Gestion;
using System.ComponentModel;
using System.Collections.ObjectModel; //pour ObservableData
using System.Runtime.CompilerServices;
using System.Windows;
using MaterialDesignThemes.Wpf;
using ShowableData; //afin d'hériter de ObservableData
using System.Globalization;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace AfficheurFermette.ViewModels
{
    class MainWindowViewModel : ObservableData, INotifyPropertyChanged
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ATTRIBUTS
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        public ConfigClass config;

        private ICommand _ChangeTabByClick;
        private ICommand _ShowPhotosEvent;

        private int _OngletChoisi = 0;
        private string _ongletActuelDesc = "Repas du jour"; //Chaine de texte (description) de l'onglet actuel
        private string _repasEntreeDuJour, _repasPlatDuJour, _repasDessertDuJour;
        private string _dateAjd; //stocke la date du jour
        private string _aSonAnnifAjd1 = "", _aSonAnnifAjd2 = "", _aSonAnnifAjd3 = "";
        private string[] _prochainEvent1, _prochainEvent2, _prochainEvent3;

        public List<ShowViewEvenement> ProchainEvenements = new List<ShowViewEvenement>();
        public List<C_Personne> ASonAnnifAjd = new List<C_Personne>();
        public ObservableCollection<ShowViewEvenement> EvenementsAff { get; set; }

        CultureInfo myCI = new CultureInfo("fr-FR"); //pour obtenir les infos en FR (date, mois, etc..)


        //////////////////////////////////////////////////////////////////////////////////////////////////////
        /// CONSTRUCTEUR
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        public MainWindowViewModel()
        {
            ChangeTabByClick = new RelayCommand(ICmd_ExecChangeTabByClick, ICmd_CanChangeTabByClick);
            //Cmd_ShowPhotosEvent = new RelayCommand(ICmd_ExecShowPhotosEventClick, ICmd_CanShowPhotosEventClick);
            Cmd_ShowPhotosEvent = new RelayCommand(Exec => ICmd_ExecShowPhotosEventClick(), CanExec => true);
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
                        case 0: ongletActuelDesc = "REPAS DU JOUR"; break;
                        case 1: ongletActuelDesc = "ACTUALITÉS"; break;
                        case 2: ongletActuelDesc = "ANNIVERSAIRE"; break;
                        case 3: ongletActuelDesc = "MÉTÉO"; break;
                        default: ongletActuelDesc = "REPAS DU JOUR"; break;
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

        //RelayCmd lors du clic sur un onglet
        public ICommand Cmd_ShowPhotosEvent
        {
            get { return _ShowPhotosEvent; }
            set
            {
                if (_ShowPhotosEvent != value)
                    _ShowPhotosEvent = value;
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
            get { return myCI.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek).ToUpper() + " " + DateTime.Now.ToString("dd") + " " + myCI.DateTimeFormat.GetMonthName(DateTime.Now.Month).ToUpper() + " " + DateTime.Now.ToString("yyyy"); } //DateTime.Now.ToString("dd/MM/yy");
            set
            {
                _dateAjd = DateTime.Now.ToString("dd/MM/yy");
                OnPropertyChanged();
            }
        }
        public string urlMeteo
        {
            get { return "https://www.prevision-meteo.ch/services/html/liege/horizontal?bg=ffffff&txtcol=000000&tmpmin=000000&tmpmax=378ADF"; }
        }

        //retourne un string contenant les personnes ayant leur anniversaire ajd
        public string aSonAnnifAjd1
        {
           get { return _aSonAnnifAjd1; }
           set
           {
                if (_aSonAnnifAjd1 != value)
                    _aSonAnnifAjd1 = value;
                OnPropertyChanged();
           }
        }

        public string aSonAnnifAjd2
        {
            get { return _aSonAnnifAjd2; }
            set
            {
                if (_aSonAnnifAjd2 != value)
                    _aSonAnnifAjd2 = value;
                OnPropertyChanged();
            }
        }

        public string aSonAnnifAjd3
        {
            get { return _aSonAnnifAjd3; }
            set
            {
                if (_aSonAnnifAjd3 != value)
                    _aSonAnnifAjd3 = value;
                OnPropertyChanged();
            }
        }

        private BitmapImage _ImgSrcASonAnnifAjd1;
        public BitmapImage ImgSrcASonAnnifAjd1
        {
            get { return _ImgSrcASonAnnifAjd1; }
            set {
                _ImgSrcASonAnnifAjd1 = value;
                OnPropertyChanged();
            }
        }

        private BitmapImage _ImgSrcASonAnnifAjd2;
        public BitmapImage ImgSrcASonAnnifAjd2
        {
            get { return _ImgSrcASonAnnifAjd2; }
            set
            {
                _ImgSrcASonAnnifAjd2 = value;
                OnPropertyChanged();
            }
        }

        private BitmapImage _ImgSrcASonAnnifAjd3;
        public BitmapImage ImgSrcASonAnnifAjd3
        {
            get { return _ImgSrcASonAnnifAjd3; }
            set
            {
                _ImgSrcASonAnnifAjd3 = value;
                OnPropertyChanged();
            }
        }
        private Visibility _AfficherAnniversaire1;
        public Visibility AfficherAnniversaire1
        {
            get
            {
                return _AfficherAnniversaire1;
            }
            set
            {
                _AfficherAnniversaire1 = value;
                OnPropertyChanged();
            }
        }

        private Visibility _AfficherAnniversaire2;
        public Visibility AfficherAnniversaire2
        {
            get
            {
                return _AfficherAnniversaire2;
            }
            set
            {
                _AfficherAnniversaire2 = value;
                OnPropertyChanged();
            }
        }

        private Visibility _AfficherAnniversaire3;
        public Visibility AfficherAnniversaire3
        {
            get
            {
                return _AfficherAnniversaire3;
            }
            set
            {
                _AfficherAnniversaire3 = value;
                OnPropertyChanged();
            }
        }

        private Visibility _AfficherTextAnniv2;
        public Visibility AfficherTextAnniv2
        {
            get
            {
                return _AfficherTextAnniv2;
            }
            set
            {
                _AfficherTextAnniv2 = value;
                OnPropertyChanged();
            }
        }

        public string AffichageHeure
        {
            get { return DateTime.Now.ToString("HH:mm"); }
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
        public string[] prochainEvent2
        {
            get { return _prochainEvent2; }
            set
            {
                if (_prochainEvent2 != value)
                    _prochainEvent2 = value;
                OnPropertyChanged();
            }
        }
        public string[] prochainEvent3
        {
            get { return _prochainEvent3; }
            set
            {
                if (_prochainEvent3 != value)
                    _prochainEvent3 = value;
                OnPropertyChanged();
            }
        }

        private int _SelectedArticle;
        public int SelectedArticle
        {
            get { return _SelectedArticle; }
            set
            {
                _SelectedArticle = value;
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

        //Show
        private bool ICmd_CanShowPhotosEventClick(object o)
        {
            return true;
        }
        //Exec
        private async void ICmd_ExecShowPhotosEventClick()
        {
            var Dialog = new Views.Dialogs.ShowPicturesEvent();
            await DialogHost.Show(Dialog);
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

            List<C_ViewEvenement> Evenements = new G_ViewEvenement(config.sChConn).Lire("");

            EvenementsAff = new ObservableCollection<ShowViewEvenement>();
            foreach (C_ViewEvenement TmpEvenement in Evenements)
            {
                EvenementsAff.Add(new ShowViewEvenement(TmpEvenement));
            }
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
            int NbreAnnifAjd = 0;
            List<C_Personne> PersonnesList = new G_Personne(config.sChConn).Lire("");
            ASonAnnifAjd.Clear();
            foreach (C_Personne Beneficiaire in PersonnesList)
            {
                string _annif = Beneficiaire.DateNaissance.ToString().Substring(0, 5);

                if (string.Compare(_annif, DateTime.Now.ToString("dd/MM")) == 0)
                {
                    ASonAnnifAjd.Add(Beneficiaire);
                    NbreAnnifAjd++;
                }
                if (NbreAnnifAjd == 2) break; //sort de la boucle si on en a 2
            }
            if (NbreAnnifAjd == 1)
            {
                AfficherAnniversaire1 = Visibility.Hidden;
                AfficherAnniversaire2 = Visibility.Visible;
                AfficherAnniversaire3 = Visibility.Hidden;
                AfficherTextAnniv2 = Visibility.Visible;

                aSonAnnifAjd2 = ASonAnnifAjd[0].Nom + " " + ASonAnnifAjd[0].Prenom;

                if (File.Exists(ASonAnnifAjd[0].Photo))
                    ImgSrcASonAnnifAjd2 = new BitmapImage(new Uri(ASonAnnifAjd[0].Photo));
                else
                    ImgSrcASonAnnifAjd2 = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + @"Images\unknown.png"));
            }
            else if (NbreAnnifAjd == 2)
            {
                AfficherAnniversaire1 = Visibility.Visible;
                AfficherAnniversaire2 = Visibility.Visible;
                AfficherAnniversaire3 = Visibility.Visible;
                AfficherTextAnniv2 = Visibility.Hidden;

                aSonAnnifAjd1 = ASonAnnifAjd[0].Nom + " " + ASonAnnifAjd[0].Prenom;
                if (File.Exists(ASonAnnifAjd[0].Photo))
                    ImgSrcASonAnnifAjd1 = new BitmapImage(new Uri(ASonAnnifAjd[0].Photo));
                else
                    ImgSrcASonAnnifAjd1 = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + @"Images\unknown.png"));

                aSonAnnifAjd2 = "";
                    ImgSrcASonAnnifAjd2 = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + @"Images\joyeuxanniversaire2.jpg"));

                aSonAnnifAjd3 = ASonAnnifAjd[1].Nom + " " + ASonAnnifAjd[1].Prenom;
                if (File.Exists(ASonAnnifAjd[1].Photo))
                    ImgSrcASonAnnifAjd3 = new BitmapImage(new Uri(ASonAnnifAjd[1].Photo));
                else
                    ImgSrcASonAnnifAjd3 = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + @"Images\unknown.png"));
            }
            else
            {
                AfficherAnniversaire1 = Visibility.Hidden;
                AfficherAnniversaire2 = Visibility.Visible;
                AfficherAnniversaire3 = Visibility.Hidden;
                AfficherTextAnniv2 = Visibility.Hidden;

                aSonAnnifAjd2 = "";
                ImgSrcASonAnnifAjd2 = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + @"Images\nobirthdaytoday.jpg"));
            }

        }

        //public void CheckAnnifToday()
        //{
        //    string TmpaSonAnnifAjd = "";
        //    string separateur = "";
        //    int i = 0;
        //    List<C_Personne> AnniversaireDuJour = new G_Personne(config.sChConn).Lire("");
        //    foreach (C_Personne aSonAnnif in AnniversaireDuJour)
        //    {
        //        string _annif = aSonAnnif.DateNaissance.ToString().Substring(0, 5);
        //        if (i > 0)
        //            separateur = " et "; //rajoute un séparateur si + d'une personne on leur annif
        //        if (string.Compare(_annif, DateTime.Now.ToString("dd/MM")) == 0)
        //        {
        //            TmpaSonAnnifAjd += separateur + aSonAnnif.Nom + " " + aSonAnnif.Prenom; //détecte si quelqu'un à son annif
        //            i++;
        //        }
        //    }
        //    aSonAnnifAjd = TmpaSonAnnifAjd;
        //}

        public void GenerationDesEvenementsExistant()
        {
            List<C_ViewEvenement> Evenements = new G_ViewEvenement(config.sChConn).Lire_DateNextEvents(DateTime.Now);
            int nbreEventsFound = Evenements.Count(); //nombre d'Events récupéré
            int nbreEventsWeWant = 3; //nombre d'Events que l'on veux afficher
            foreach (C_ViewEvenement TmpEvent in Evenements)
            {
                ProchainEvenements.Add(new ShowViewEvenement(TmpEvent));
                if ((ProchainEvenements.Count() >= nbreEventsWeWant) || (nbreEventsFound <= 0))
                    break;
            }
            if (nbreEventsFound > 0)
            {
                prochainEvent1 = ConstruitStringEvents(0);
                //tbTest.Text += _prochainEvent1[3];
                if (nbreEventsFound > 1)
                {
                    prochainEvent2 = ConstruitStringEvents(1);
                    if (nbreEventsFound > 2)
                        prochainEvent3 = ConstruitStringEvents(2);
                }
            }
        }
        public string[] ConstruitStringEvents(int num)
        {
            return new string[] {
                ProchainEvenements[num].ID.ToString(),
                ProchainEvenements[num].DateDebut.ToString(),
                ProchainEvenements[num].Titre,
                ProchainEvenements[num].Lieu,
                ProchainEvenements[num].Description
            };
        }
    }


}
