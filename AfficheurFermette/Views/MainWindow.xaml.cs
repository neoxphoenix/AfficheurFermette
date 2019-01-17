using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using AfficheurFermette.ViewModels; //Utiliser les ViewModels
using System.IO; 
using Projet_AFFICHEURFERMETTE.MDF.Classes;
using Projet_AFFICHEURFERMETTE.MDF.Acces;
using Projet_AFFICHEURFERMETTE.MDF.Gestion;
using ShowableData;
using System.Diagnostics;

namespace AfficheurFermette
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        //Attributs
		private string sChConn = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + System.AppDomain.CurrentDomain.BaseDirectory + "AfficheurFermette.mdf;Integrated Security=True;Connect Timeout=30";
        private MainWindowViewModel vm;
        private int _dateAjd;

        //Constructeur
        public MainWindow()
        {
            if (ExisteInstance())
            {
                MessageBox.Show("Une instance du modifieur ou de l'afficheur est déjà ouverte ! Veuillez fermer cette instance avant de relancer ce programme...");
                this.Close();
            }
            InitializeComponent();

            vm = new MainWindowViewModel();
            this.DataContext = vm; //tell the View about what its ViewModel via DataContext

            // On vérifie si le fichier de config existe
            if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "config.xml"))
                vm.config = ConfigClass.DeserializeFromFile(System.AppDomain.CurrentDomain.BaseDirectory + "config.xml"); // Si oui, on le charge
            else
                vm.config = new ConfigClass(); // sinon on créée une nouvelle config

            // On récupère uniquement le chemin d'accès du fichier à partir du ConnectionString
            string[] stab = vm.config.sChConn.Split('=');
            string[] stab2 = stab[2].Split(';');
            OpenFileDialog dlgChargerDB = new OpenFileDialog();

            //Charge les données météo au démarrage
            dateAjd = DateTime.Now.Day; //rafraichissement météo une fois par jour
            BrowserMeteo.Navigate("https://www.prevision-meteo.ch/services/html/la-reid/horizontal?bg=ffffff&txtcol=000000&tmpmin=000000&tmpmax=378ADF");

            //Timer pour mise à jour météo navigateur
            System.Windows.Threading.DispatcherTimer CheckDay = new System.Windows.Threading.DispatcherTimer();
            CheckDay.Tick += new EventHandler(CheckDay_Tick);
            CheckDay.Interval = TimeSpan.FromSeconds(10);
            CheckDay.Start();

            // Si le fichier n'existe pas on propose à l'utilisateur d'indiquer l'emplacement de la base de données
            if (!File.Exists(stab2[0]))
            {
                bool boucle = false;
                do
                {
                    if (MessageBox.Show("La base de donnée par défaut est introuvable.\nSouhaitez-vous indiquer un autre emplacement ?", "Base de données introuvable", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                    {
                        dlgChargerDB.Filter = "Fichier de base de données Microsoft SQL|*.mdf|Tous fichiers|*.*";
                        if (dlgChargerDB.ShowDialog() == true)
                        {
                            if (File.Exists(dlgChargerDB.FileName) && System.IO.Path.GetExtension(dlgChargerDB.FileName) == ".mdf")
                            {
                                vm.config.sChConn = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + dlgChargerDB.FileName + ";Integrated Security=True";
                                boucle = false;
                                // On sauvegarde la config
                                vm.config.SerializeToFile(System.AppDomain.CurrentDomain.BaseDirectory + "config.xml");
                            }
                            else
                            {
                                MessageBox.Show("Ce fichier n'est pas valide !");
                                boucle = true;
                            }
                        }
                        else
                            boucle = true;
                    }
                    else // L'utilisateur refuse d'indiquer l'emplacement de la DB => On ferme le programme
                    {
                        boucle = false;
                        this.Close();
                    }
                }
                while (boucle);
            }

            // Le fichier existe
            if (File.Exists(stab2[0]) || File.Exists(dlgChargerDB.FileName))
            {
                vm.ChargerDonneesDaily();
            }
        }

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.WindowStyle = WindowStyle.None;
			this.WindowState = WindowState.Maximized;
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.F1)
			{
                if (MessageBox.Show("Êtes-vous sûr de vouloir démarrer le modifieur ? L'action fermera ce programme !", "Confirmer", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // On veut modifier les données => Lancement du modifieur (si trouvable)
                    if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "ModifieurFermette.exe"))
                    {
                        Process p = new Process { StartInfo = new ProcessStartInfo("ModifieurFermette") };
                        p.Start();
                        this.Close(); // Indispensable de fermer ce programme après avoir lancé le modifieur vu qu'au sinon le mdf est indisponible vu que deux app essaient de s'y connecter en même temps
                    }
                    else
                        MessageBox.Show("l'exécutable du modifieur est introuvable ! Veuillez le replacer dans le même dossier que ce programme...");
                }
            }
            if (e.Key == Key.Escape)
			{
				// On veut quitter le programme
				if (MessageBox.Show("Êtes-vous sûr de vouloir fermer l'application ?", "Confirmer", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
					this.Close();
			}
		}

        #region CONFIG DU BROWSER + DESACTIVATION APRES CHARGEMENT DES DONNEES METEO UNE FOIS PAR JOUR
        bool BrowserIsLoaded = false;
        private void BrowserMeteo_LoadCompleted(object sender, NavigationEventArgs e)
        {
            BrowserIsLoaded = true;
        }

        private void BrowserMeteo_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (BrowserIsLoaded)
                e.Cancel = true;
        }

        private void BrowserMeteo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (BrowserIsLoaded)
                e.Handled = true;
        }

        private void BrowserMeteoRefresh()
        {
            BrowserIsLoaded = false;
            BrowserMeteo.Navigate("https://www.prevision-meteo.ch/services/html/la-reid/horizontal?bg=ffffff&txtcol=000000&tmpmin=000000&tmpmax=378ADF");
        }

        public int dateAjd
        {
            get
            {
                return _dateAjd;
            }
            set
            {
                if (_dateAjd != value)
                {
                    _dateAjd = value;
                    BrowserMeteoRefresh();
                }
            }
        }

        public void CheckDay_Tick(object sender, EventArgs e)
        {
            dateAjd = DateTime.Now.Day;
        }
        #endregion


        public void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        // Vérifie si une instance d'un des deux programmes n'existe pas déjà
        static bool ExisteInstance()
        {
            Process actu = Process.GetCurrentProcess();
            Process[] acti = Process.GetProcesses();
            foreach (Process p in acti)
                if (p.Id != actu.Id)
                    if (p.ProcessName == "AfficheurFermette" || p.ProcessName == "ModifieurFermette")
                        return true;
            return false;
        }
    }
}
