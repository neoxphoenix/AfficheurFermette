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


namespace AfficheurFermette.ViewModels
{
    class MainWindowViewModel : ObservableData, INotifyPropertyChanged
    {
        #region ATTRIBUTS + ACCESSEURS
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ATTRIBUTS + ACCESSEURS
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        public ConfigClass config;
        public string dossierResources = Directory.GetCurrentDirectory() + @"\Resources\Images\";
        private int _OngletChoisi = 0; //par défaut 0
        private string _ongletActuelDesc = "REPAS DU JOUR"; //Chaine de texte (description) de l'onglet actuel
        private string _aSonAnnifAjd1 = "", _aSonAnnifAjd2 = "", _aSonAnnifAjd3 = "";

        public List<ShowViewEvenement> ProchainEvenements = new List<ShowViewEvenement>();
        public List<C_Personne> ASonAnnifAjd = new List<C_Personne>();
        public ObservableCollection<ShowViewEvenement> EvenementsAff { get; set; }

        CultureInfo myCI = new CultureInfo("fr-FR"); //pour obtenir les infos en FR (date, mois, etc..)

        public int OngletChoisi
        {
            get { return _OngletChoisi; }
            set
            {
                if (_OngletChoisi != value)
                {
                    _OngletChoisi = value;
                    switch (_OngletChoisi)
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

        //RelayCmd lors du clic sur un onglet
        private ICommand _ChangeTabByClick;
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
        private ICommand _ShowPhotosEvent;
        public ICommand Cmd_ShowPhotosEvent
        {
            get { return _ShowPhotosEvent; }
            set
            {
                if (_ShowPhotosEvent != value)
                    _ShowPhotosEvent = value;
            }
        }
        private string _repasEntreeDuJour;
        public string repasEntreeDuJour
        {
            get {
                //if (_repasEntreeDuJour != null)
                //    return _repasEntreeDuJour.ToUpper();
                //else
                    return _repasEntreeDuJour;
            }
            set
            {
                if (_repasEntreeDuJour != value)
                    _repasEntreeDuJour = value;
                OnPropertyChanged();
            }
        }
        private string _repasPlatDuJour;
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
        private string _repasDessertDuJour;
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
        private string _jourAjd;
        public string jourAjd
        {
            get { return _jourAjd; }
            set
            {
                if (_jourAjd != value)
                    _jourAjd = value;
                OnPropertyChanged();
            }
        }

        private string _dateAjd; //stocke la date du jour
        public string dateAjd
        {
            get { return _dateAjd; }
            set
            {
                if (_dateAjd != value)
                {
                    _dateAjd = value;
                    //ChargerDonneesDaily(); //On vérifie les données une fois par jour lors du changement de la date |>>>> FINALEMENT VERIFICATION TOUTES LES HEURES
                }
                OnPropertyChanged();
            }
        }
        public string urlMeteo
        {
            get { return "https://www.prevision-meteo.ch/services/html/liege/horizontal?bg=ffffff&txtcol=000000&tmpmin=000000&tmpmax=378ADF"; }
        }
        
        public string aSonAnnifAjd1 //retourne un string contenant les personnes ayant leur anniversaire ajd
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
            set
            {
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

        public string _affichageHeure;
        public string affichageHeure
        {
            get { return _affichageHeure; }
            set
            {
                _affichageHeure = value;
                OnPropertyChanged();

                //On recharge les données toutes les heures
                if (string.Compare(_affichageHeure.Substring(3, 2), "00") == 0)
                {
                    ChargerDonneesDaily();
                }
            }
        }

        private string[] _prochainEvent1;
        public string[] prochainEvent1
        {
            get { return _prochainEvent1; }
            set
            {
                if (_prochainEvent1 != value)
                    _prochainEvent1 = value;
                OnPropertyChanged();
            }
        }
        private string[] _prochainEvent2;
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
        private string[] _prochainEvent3;
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

        private BitmapImage _previsionMeteo1;
        public BitmapImage previsionMeteo1
        {
            get
            {
                    return _previsionMeteo1;
            }
            set
            {
                _previsionMeteo1 = value;
                OnPropertyChanged();
            }
        }

        private BitmapImage _previsionMeteo2;
        public BitmapImage previsionMeteo2
        {
            get
            {
                return _previsionMeteo2;
            }
            set
            {
                _previsionMeteo2 = value;
                OnPropertyChanged();
            }
        }

        private BitmapImage _photoToDisplay;
        public BitmapImage photoToDisplay
        {
            get {
                if (_photoToDisplay != null) 
                    return _photoToDisplay;
                else 
                    return photoToDisplay = new BitmapImage(new Uri(dossierResources + "gallery.png"));
            }
            set
            {
                _photoToDisplay = value;
                OnPropertyChanged();
            }
        }

        //<<< ICOMMAND >>>
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

        //ICmd_ExecShowPhotosEventClick Show
        private bool ICmd_CanShowPhotosEventClick(object o)
        {
            return true;
        }
        //ICmd_ExecShowPhotosEventClick Exec
        private async void ICmd_ExecShowPhotosEventClick()
        {
            bool canOpenDialog = false;
            List<C_PhotoEvenement> ListePhotosEvenements = new G_ViewEvenement(config.sChConn).LirePhotosEvenement(EvenementsAff[DGActu_SelectedItem].ID);
            if (ListePhotosEvenements.Count() > 0)
                canOpenDialog = true;

            List<C_PersonnePos> GetEvenements = new G_ViewEvenement(config.sChConn).LireClassementEvenement(EvenementsAff[DGActu_SelectedItem].ID);
            if (GetEvenements.Count() > 0)
                canOpenDialog = true;

            if (canOpenDialog == true)
            {
                var Dialog = new Views.Dialogs.ShowPicturesEvent(config.sChConn, EvenementsAff[DGActu_SelectedItem]);
                await DialogHost.Show(Dialog);
            }
        }

        //Récupère l'ID Index d'un événement séléctionné (onglet 1)
        private int _DGActu_SelectedItem;
        public int DGActu_SelectedItem
        {
            get { return _DGActu_SelectedItem; }
            set
            {
                if (_DGActu_SelectedItem != value)
                {
                    _DGActu_SelectedItem = value;
                    OnPropertyChanged();

                    //Vérifie si photo à afficher ou non
                    List<C_PhotoEvenement> ListePhotosEvenements = new G_ViewEvenement(config.sChConn).LirePhotosEvenement(EvenementsAff[DGActu_SelectedItem].ID);
                    if (ListePhotosEvenements.Count() > 0)
                    {
                        if (File.Exists(ListePhotosEvenements[0].Photo))
                            photoToDisplay = new BitmapImage(new Uri(ListePhotosEvenements[0].Photo));
                        else
                            photoToDisplay = new BitmapImage(new Uri(dossierResources + "gallery.png"));
                    }
                    else
                        photoToDisplay = new BitmapImage(new Uri(dossierResources + "gallery.png"));
                }
            }
        }

        //TEST
        private ICommand _Cmd_TestClick;
        public ICommand Cmd_TestClick
        {
            get { return _Cmd_TestClick; }
            set
            {
                if (_Cmd_TestClick != value)
                    _Cmd_TestClick = value;
            }
        }

        private void ICmd_ExecTestClick()
        {
            //ChargerDonneesDaily();
            
        }

        #endregion


        #region CONSTRUCTEUR
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        /// CONSTRUCTEUR
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        public MainWindowViewModel()
        {
            ChangeTabByClick = new RelayCommand(ICmd_ExecChangeTabByClick, ICmd_CanChangeTabByClick);
            Cmd_ShowPhotosEvent = new RelayCommand(Exec => ICmd_ExecShowPhotosEventClick(), CanExec => true);
            Cmd_TestClick = new RelayCommand(Exec => ICmd_ExecTestClick(), CanExec => true); //commande test

            //Timer pour mise à jour de l'heure et la date
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(UpdateTime_Tick);
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Start();
        }
        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //METHODES
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
            GetMenuDuJour(); //Génération Menu du jour
            CheckAnnifToday(); //Génération des Anniversaires
            //GenerationDesEvenementsExistant();
            GenerationEvenementsActu(); //Génération des événements affiché dans l'onglet ACTUALITES
            previsionMeteo1 = new BitmapImage(new Uri("https://www.prevision-meteo.ch/uploads/widget/la-reid_0.png")); //charge bitmap prévision météo pour AJD
            previsionMeteo2 = new BitmapImage(new Uri("https://www.prevision-meteo.ch/uploads/widget/la-reid_1.png")); //charge bitmap prévision météo pour DEMAIN
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
            List<C_Personne> PersonnesList = new G_Personne(config.sChConn).Lire("");
            ASonAnnifAjd.Clear();
            foreach (C_Personne Beneficiaire in PersonnesList)
            {
                DateTime _annif = Beneficiaire.DateNaissance;

                if (_annif.Month == DateTime.Now.Month && _annif.Day == DateTime.Now.Day)
                {
                    ASonAnnifAjd.Add(Beneficiaire);
                }
                if (ASonAnnifAjd.Count == 2) break; //sort de la boucle si on en a 2
            }
            if (ASonAnnifAjd.Count == 1)
            {
                AfficherAnniversaire1 = Visibility.Hidden;
                AfficherAnniversaire2 = Visibility.Visible;
                AfficherAnniversaire3 = Visibility.Hidden;
                AfficherTextAnniv2 = Visibility.Visible;

                aSonAnnifAjd2 = ASonAnnifAjd[0].Nom + " " + ASonAnnifAjd[0].Prenom;

                if (File.Exists(ASonAnnifAjd[0].Photo))
                    ImgSrcASonAnnifAjd2 = new BitmapImage(new Uri(ASonAnnifAjd[0].Photo));
                else
                    ImgSrcASonAnnifAjd2 = new BitmapImage(new Uri(dossierResources + "unknown.png"));
            }
            else if (ASonAnnifAjd.Count == 2)
            {
                AfficherAnniversaire1 = Visibility.Visible;
                AfficherAnniversaire2 = Visibility.Visible;
                AfficherAnniversaire3 = Visibility.Visible;
                AfficherTextAnniv2 = Visibility.Hidden;

                aSonAnnifAjd1 = ASonAnnifAjd[0].Nom + " " + ASonAnnifAjd[0].Prenom;
                if (File.Exists(ASonAnnifAjd[0].Photo))
                    ImgSrcASonAnnifAjd1 = new BitmapImage(new Uri(ASonAnnifAjd[0].Photo));
                else
                    ImgSrcASonAnnifAjd1 = new BitmapImage(new Uri(dossierResources + "unknown.png"));

                aSonAnnifAjd2 = "";
                    ImgSrcASonAnnifAjd2 = new BitmapImage(new Uri(dossierResources + "joyeuxanniversaire2.jpg"));

                aSonAnnifAjd3 = ASonAnnifAjd[1].Nom + " " + ASonAnnifAjd[1].Prenom;
                if (File.Exists(ASonAnnifAjd[1].Photo))
                    ImgSrcASonAnnifAjd3 = new BitmapImage(new Uri(ASonAnnifAjd[1].Photo));
                else
                    ImgSrcASonAnnifAjd3 = new BitmapImage(new Uri(dossierResources + "unknown.png"));
            }
            else
            {
                AfficherAnniversaire1 = Visibility.Hidden;
                AfficherAnniversaire2 = Visibility.Visible;
                AfficherAnniversaire3 = Visibility.Hidden;
                AfficherTextAnniv2 = Visibility.Hidden;

                aSonAnnifAjd2 = "";
                ImgSrcASonAnnifAjd2 = new BitmapImage(new Uri(dossierResources + "nobirthdaytoday.jpg"));
            }

        }

        public void GenerationDesEvenementsExistant()
        {
            List<C_ViewEvenement> Evenements = new G_ViewEvenement(config.sChConn).Lire_DateNextEvents(DateTime.Now);
            int nbreEventsFound = Evenements.Count(); //nombre d'Events récupéré
            int nbreEventsWeWant = 3; //nombre d'Events que l'on veux afficher
            ProchainEvenements.Clear(); //vide la liste

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

        // On ne prend (si possible) que les 6 prochains événements à partir de la date d'ajd
        public void GenerationEvenementsActu()
        {
            List<C_ViewEvenement> Evenements = new G_ViewEvenement(config.sChConn).Lire("DateDebut");
            int count = 0;
            int Index = -1;
            do // On cherche à partir d'il y a 5 jours et ensuite jusqu'au 7 prochains jours le prochain événement à venir
            {
                count++;
                Index = Evenements.FindIndex(e => e.DateDebut.Day == DateTime.Now.AddDays(count - 5).Day && e.DateDebut.Month == DateTime.Now.Month);
                if (count >= 7 && Index == -1) // Si on n'a toujours rien trouvé après une semaine, on sort de la boucle
                    break;
            }
            while (Index == -1); // Tant qu'on a pas trouvé d'événement se déroulant à la date cherchée
            if (Index == -1) // On a rien trouvé
                Evenements = Evenements.Skip(Evenements.Count - 7).ToList(); // On ne reprend que les 6 derniers
            else
                Evenements = Evenements.Skip(Index).Take(6).ToList(); // On en prend 6 à partir de la date prévue

            EvenementsAff = new ObservableCollection<ShowViewEvenement>();
            foreach (C_ViewEvenement TmpEvenement in Evenements)
            {
                EvenementsAff.Add(new ShowViewEvenement(TmpEvenement));
            }
        }

        //Timer mise à jour de la date et de l'heure
        public void UpdateTime_Tick(object sender, EventArgs e)
        {
            affichageHeure = DateTime.Now.ToString("HH:mm");
            dateAjd = DateTime.Now.ToString("dd") + " " + myCI.DateTimeFormat.GetMonthName(DateTime.Now.Month).ToUpper() + " " + DateTime.Now.ToString("yyyy");
            jourAjd = myCI.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek).ToUpper();
        }
    }

}
