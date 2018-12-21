using System;
using System.Collections.Generic;
using System.Data;
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
using System.Collections.ObjectModel;

using Projet_AFFICHEURFERMETTE.MDF.Acces;
using Projet_AFFICHEURFERMETTE.MDF.Classes;
using Projet_AFFICHEURFERMETTE.MDF.Gestion;
using ShowableData;
using System.ComponentModel;
using System.IO;
using ModifieurFermette.Models;

namespace ModifieurFermette
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        public ObservableCollection<ShowViewMenuDuJour> MenusAff;
        public ObservableCollection<ShowViewEvenement> EvenementsAff;
        public ObservableCollection<ShowPersonne> PersonnesAff;
        public List<C_ViewMenuDuJour> Menus;
        public List<C_ViewEvenement> Evenements;
        public List<C_Personne> Personnes;

        private bool IsAllItemsEvenementsSelected, IsAllItemsPersonnesSelected, IsAllItemsMenuDuJourSelected;

        public ConfigClass config;

        public MainWindow()
		{
			InitializeComponent();
            // On vérifie si le fichier de config existe
            if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "config.xml"))
                config = ConfigClass.DeserializeFromFile(System.AppDomain.CurrentDomain.BaseDirectory + "config.xml"); // Si oui, on le charge
            else
                config = new ConfigClass(); // sinon on créée une nouvelle config
            // On récupère uniquement le chemin d'accès du fichier à partir du ConnectionString
            string[] stab = config.sChConn.Split('=');
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
                                config.sChConn = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + dlgChargerDB.FileName + ";Integrated Security=True";
                                boucle = false;
                                // On sauvegarde la config
                                config.SerializeToFile(System.AppDomain.CurrentDomain.BaseDirectory + "config.xml");
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

                ChargerDonnees();
            }
		}

        private void ChargerDonnees()
        {
            // Extraction des données de la DB
            Menus = new G_ViewMenuDuJour(config.sChConn).Lire("");
            Evenements = new G_ViewEvenement(config.sChConn).Lire("");
            Personnes = new G_Personne(config.sChConn).Lire("");

            // Placement dans des Oservables
            MenusAff = new ObservableCollection<ShowViewMenuDuJour>();
            EvenementsAff = new ObservableCollection<ShowViewEvenement>();
            PersonnesAff = new ObservableCollection<ShowPersonne>();
            foreach (C_ViewMenuDuJour TmpMenu in Menus)
            { MenusAff.Add(new ShowViewMenuDuJour(TmpMenu)); }
            foreach (C_ViewEvenement TmpEvenement in Evenements)
            { EvenementsAff.Add(new ShowViewEvenement(TmpEvenement)); }
            foreach (C_Personne TmpPersonne in Personnes)
            { PersonnesAff.Add(new ShowPersonne(TmpPersonne)); }

            // Mise à jour des DataGrid
            DGmenus.ItemsSource = MenusAff;
            DGevenements.ItemsSource = EvenementsAff;
            dgpersonnes.ItemsSource = PersonnesAff;
        }
    }
}
