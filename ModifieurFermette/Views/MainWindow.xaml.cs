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
using System.Xml;

namespace ModifieurFermette
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        private string sChConn = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\picho\Nextcloud\Cours\Informatique\2e bac\POO\Q2\db\Location_DVD.mdf;Integrated Security=True";
        public ObservableCollection<ShowViewMenuDuJour> MenusAff;
        public ObservableCollection<ShowViewEvenement> EvenementsAff;
        public ObservableCollection<ShowPersonne> PersonnesAff;
        public List<C_ViewMenuDuJour> Menus;
        public List<C_ViewEvenement> Evenements;
        public List<C_Personne> Personnes;

        private bool IsAllItemsEvenementsSelected, IsAllItemsPersonnesSelected, IsAllItemsMenuDuJourSelected;

        public MainWindow()
		{
			InitializeComponent();
            XmlWriter xmlWr = new XmlWriter.Create();
            string[] stab = sChConn.Split('=');
            string[] stab2 = stab[2].Split(';');
            OpenFileDialog dlgChargerDB = new OpenFileDialog();
            if (!System.IO.File.Exists(stab2[0]))
            {
                bool boucle = false;
                do
                {
                    // TODO: Vérifier que le fichier est valide
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
                ChargerDonnees();
            }
		}

        private void ChargerDonnees()
        {
            // Extraction des données de la DB
            Menus = new G_ViewMenuDuJour(sChConn).Lire("");
            Evenements = new G_ViewEvenement(sChConn).Lire("");
            Personnes = new G_Personne(sChConn).Lire("");

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
