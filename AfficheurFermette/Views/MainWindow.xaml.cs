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

        //Constructeur
        public MainWindow()
        {
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
                // liaison des données avec les DataGrid
                //DGevenements.ItemsSource = vm.EvenementsAff;
                //dgpersonnes.ItemsSource = vm.PersonnesAff;
                //DGmenus.ItemsSource = vm.MenusAff;

            }

            //if (System.IO.File.Exists(stab2[0]) || System.IO.File.Exists(dlgChargerDB.FileName))
            //{
            //             // tests
            //             List<C_ViewMenuDuJour> menus = new G_ViewMenuDuJour(sChConn).Lire("");
            //             C_ViewMenuDuJour menu = new G_ViewMenuDuJour(sChConn).Lire_ID(1);
            //             menu = new G_ViewMenuDuJour(sChConn).Lire_Date(new DateTime(2018, 12, 12));

            //             List<C_ViewEvenement> evenements = new G_ViewEvenement(sChConn).Lire("");
            //             C_ViewEvenement evenement = new G_ViewEvenement(sChConn).Lire_ID(1);
            //             evenements = new G_ViewEvenement(sChConn).Lire_DateDebut(new DateTime(2018, 12, 12));
            //             evenements = new G_ViewEvenement(sChConn).Lire_DateFin(new DateTime(2018, 12, 13));
            //             evenements = new G_ViewEvenement(sChConn).Lire_Date(new DateTime(2018, 12, 12));

            //}
        }

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.WindowStyle = WindowStyle.None;
			this.WindowState = WindowState.Maximized;
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			//if (e.Key == Key.Return)
			//{
			//	// On veut modifier les données => Lancement d'un nouvelle fenêtre
			//	MessageBox.Show("Test");
			//}
			if (e.Key == Key.Escape)
			{
				// On veut quitter le programme
				if (MessageBox.Show("Êtes-vous sûr de vouloir fermer l'application ?", "Confirmer", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
					this.Close();
			}
		}

        #region Effets sur les onglets (désactivé pour le moment)
        private void EffetOnglet(Canvas ChoixCnv)
        {
            CnvOnglet1.Children.Clear();
            CnvOnglet2.Children.Clear();
            CnvOnglet3.Children.Clear();
            CnvOnglet4.Children.Clear();

            Rectangle EffetBordure = new Rectangle();
            EffetBordure.Fill = Brushes.DarkBlue;
            EffetBordure.Width = ImgOnglet2.ActualWidth; //les onglets ont tous la même taille, donc OK
            EffetBordure.Height = 2;
            EffetBordure.StrokeThickness = 2;

            ChoixCnv.Children.Add(EffetBordure);
            Canvas.SetLeft(EffetBordure, 0);
            Canvas.SetTop(EffetBordure, 0);
        }

        private void ImgOnglet1_Click(object sender, RoutedEventArgs e)
        {
            EffetOnglet(CnvOnglet1);
        }

        private void ImgOnglet2_Click(object sender, RoutedEventArgs e)
        {
            EffetOnglet(CnvOnglet2);
        }

        private void ImgOnglet3_Click(object sender, RoutedEventArgs e)
        {
            EffetOnglet(CnvOnglet3);
        }

        private void ImgOnglet4_Click(object sender, RoutedEventArgs e)
        {
            EffetOnglet(CnvOnglet4);
        }
        #endregion

        public string[] _prochainEvent1, _prochainEvent2, _prochainEvent3;
        public List<ShowViewEvenement> ProchainEvenements = new List<ShowViewEvenement>();


        public void Button_Click(object sender, RoutedEventArgs e)
        {
            List<C_ViewEvenement> Evenements = new G_ViewEvenement(vm.config.sChConn).Lire_DateNextEvents(DateTime.Now);
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
                _prochainEvent1 = ConstruitStringEvents(0);
                tbTest.Text += _prochainEvent1[3];
                if (nbreEventsFound > 1)
                {
                    _prochainEvent2 = ConstruitStringEvents(1);
                    if (nbreEventsFound > 2)
                        _prochainEvent3 = ConstruitStringEvents(2);
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
