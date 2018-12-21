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
using ModifieurFermette.ViewModels;

namespace ModifieurFermette
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        private MainWindowViewModel vm;

        public MainWindow()
		{
			InitializeComponent();
            vm = new MainWindowViewModel();
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
                vm.ChargerDonnees();
                DGevenements.ItemsSource = vm.EvenementsAff;
                dgpersonnes.ItemsSource = vm.PersonnesAff;
                DGmenus.ItemsSource = vm.MenusAff;
            }
		}
    }
}
