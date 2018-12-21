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

//using Projet_AFFICHEURFERMETTE.MDF.Classes;
//using Projet_AFFICHEURFERMETTE.MDF.Acces;
//using Projet_AFFICHEURFERMETTE.MDF.Gestion;


namespace AfficheurFermette
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		private string sChConn = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + System.AppDomain.CurrentDomain.BaseDirectory + "AfficheurFermette.mdf;Integrated Security=True;Connect Timeout=30";

        public string DisplayedImage
        {
            get { return @"pack://application:,,,/Images/test.png"; }
        }

        //ImageViewer1.Source = new BitmapImage(new Uri("Creek.jpg", UriKind.Relative));


        public MainWindow()
        {
            InitializeComponent();

			string[] stab = sChConn.Split('=');
			string[] stab2 = stab[2].Split(';');

			OpenFileDialog dlgChargerDB = new OpenFileDialog();
			if (!System.IO.File.Exists(stab2[0]))
			{
				bool boucle = false;
				do
				{
					if (MessageBox.Show("La base de donnée par défaut est introuvable.\nSouhaitez-vous indiquer un autre emplacement ?", "Base de données introuvable", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
					{
						dlgChargerDB.Filter = "Fichier de base de données Microsoft SQL|*.mdf|Tous fichiers|*.*";
						if (dlgChargerDB.ShowDialog() == true)
						{
							sChConn = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + dlgChargerDB.FileName + ";Integrated Security=True";
							boucle = false;
						}
						else
							boucle = true;
					}
					else
					{
						boucle = false;
						this.Close();
					}
				}
				while (boucle);
			}

			if (System.IO.File.Exists(stab2[0]) || System.IO.File.Exists(dlgChargerDB.FileName))
			{

			}

            //Initialisation des effets sur les onglets
            /*
            Rectangle EffetBordure = new Rectangle();
            EffetBordure.Fill = Brushes.Black;
            EffetBordure.Width = ImgOngletRepas.ActualWidth;
            EffetBordure.Height = 10;
            EffetBordure.StrokeThickness = 2;

            CnvOnglet1.Children.Add(EffetBordure);
            Canvas.SetLeft(EffetBordure, 0);
            Canvas.SetTop(EffetBordure, 0);
            */
        }

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.WindowStyle = WindowStyle.None;
			this.WindowState = WindowState.Maximized;
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				// On veut modifier les données => Lancement d'un nouvelle fenêtre
				MessageBox.Show("Test");
			}
			else if (e.Key == Key.Escape)
			{
				// On veut quitter le programme
				if (MessageBox.Show("Êtes-vous sûrs de vouloir fermer l'application ?", "Confirmer", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
					this.Close();
			}
		}

        private void ImgOngletRepas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OngletActif_Nom.Content = "Repas du jour";

            EffetOnglet(CnvOnglet1);
        }

        private void ImgOngletActu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OngletActif_Nom.Content = "Actualités";
            EffetOnglet(CnvOnglet2);
        }

        private void ImgOngletAtelier_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OngletActif_Nom.Content = "Ateliers";
            EffetOnglet(CnvOnglet3);
        }

        private void ImgOngletEduc_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OngletActif_Nom.Content = "Educateurs présent aujourd'hui";
            EffetOnglet(CnvOnglet4);
        }

        private void EffetOnglet(Canvas ChoixCnv)
        {
            CnvOnglet1.Children.Clear();
            CnvOnglet2.Children.Clear();
            CnvOnglet3.Children.Clear();
            CnvOnglet4.Children.Clear();

            Rectangle EffetBordure = new Rectangle();
            EffetBordure.Fill = Brushes.Black;
            EffetBordure.Width = ImgOnglet1.ActualWidth; //les onglets ont tous la même taille, donc OK
            EffetBordure.Height = 10;
            EffetBordure.StrokeThickness = 2;

            ChoixCnv.Children.Add(EffetBordure);
            Canvas.SetLeft(EffetBordure, 0);
            Canvas.SetTop(EffetBordure, 0);
        }

    }
}
