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
using System.ComponentModel;
using System.IO;
using ModifieurFermette.ViewModels;
using System.Diagnostics;

// dll
using Projet_AFFICHEURFERMETTE.MDF.Acces;
using Projet_AFFICHEURFERMETTE.MDF.Classes;
using Projet_AFFICHEURFERMETTE.MDF.Gestion;
using ShowableData;

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
            if (ExisteInstance())
            {
                MessageBox.Show("Veuillez n'ouvrir qu'une seule instance du programme à la fois SVP !");
                this.Close();
            }
            InitializeComponent();
            vm = new MainWindowViewModel();
            this.DataContext = vm;
            if (vm.CloseAction == null)
                vm.CloseAction = new Action(() => this.Close()); // Permet de fermer la fenêtre depuis le ViewModel; solution de -> http://jkshay.com/closing-a-wpf-window-using-mvvm-and-minimal-code-behind/
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
                // liaison des données avec les DataGrid
                DGevenements.ItemsSource = vm.EvenementsAff;
                dgpersonnes.ItemsSource = vm.PersonnesAff;
                DGmenus.ItemsSource = vm.MenusAff;

            }
        }

        static bool ExisteInstance()
        {
            Process actu = Process.GetCurrentProcess();
            Process[] acti = Process.GetProcesses();
            foreach (Process p in acti)
                if (p.Id != actu.Id)
                    if (actu.ProcessName == p.ProcessName)
                        return true;
            return false;
        }
    }
}
