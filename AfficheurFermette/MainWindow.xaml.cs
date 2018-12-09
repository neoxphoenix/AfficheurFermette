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

using Projet_AFFICHEURFERMETTE.MDF.Classes;
using Projet_AFFICHEURFERMETTE.MDF.Acces;
using Projet_AFFICHEURFERMETTE.MDF.Gestion;

namespace AfficheurFermette
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		private string sChConn = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=E:\Server\Nextcloud\Cours\Informatique\3e bac\Compléments progra(Pata)\PROJET AFFICHEUR\AfficheurFermette.mdf;Integrated Security=True;Connect Timeout=30";
		public MainWindow()
        {
            InitializeComponent();
			string[] stab = sChConn.Split('=');
			string[] stab2 = stab[2].Split(';');
			if (!System.IO.File.Exists(stab2[0]))
			{
				bool boucle = false;
				OpenFileDialog dlgChargerDB = new OpenFileDialog();
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
		}
    }
}
