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
using System.IO;

using Projet_AFFICHEURFERMETTE.MDF.Acces;
using Projet_AFFICHEURFERMETTE.MDF.Classes;
using Projet_AFFICHEURFERMETTE.MDF.Gestion;
using Microsoft.Win32;

namespace ModifieurFermette
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        private string sChConn = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\picho\Nextcloud\Cours\Informatique\2e bac\POO\Q2\db\Location_DVD.mdf;Integrated Security=True";
        private DataTable dtEvenements, dtPersonnes, dtMenus;

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
                RemplirDGVs();
            }
		}

        private void RemplirDGVs()
        {
            RemplirDGVevenement();
            RemplirDGVpersonnes();
            RemplirDGVmenus();
        }
        private void RemplirDGVevenement()
        {

        }
        private void RemplirDGVpersonnes()
        {

        }
        private void RemplirDGVmenus()
        {
            // Création de la table
            dtMenus = new DataTable();
            dtMenus.Columns.Add(new DataColumn("ID", System.Type.GetType("System.Int32")));
            dtMenus.Columns.Add(new DataColumn("Date", System.Type.GetType("System.DateTime")));
            dtMenus.Columns.Add("Entrée");
            dtMenus.Columns.Add("Plat");
            dtMenus.Columns.Add("Dessert");

            List<C_ViewMenuDuJour> Menus = new G_ViewMenuDuJour(sChConn).Lire("");
            foreach(C_ViewMenuDuJour menu in Menus)
            {
                dtMenus.Rows.Add(menu.ID, menu.Date, menu.eNom, menu.pNom, menu.dNom);
            }
            DGVmenus.DataContext = dtMenus.DefaultView;
        }
    }
}
