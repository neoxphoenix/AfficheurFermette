using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Projet_AFFICHEURFERMETTE.MDF.Acces;
using Projet_AFFICHEURFERMETTE.MDF.Classes;
using Projet_AFFICHEURFERMETTE.MDF.Gestion;
using ShowableData;

namespace ModifieurFermette.ViewModels
{
    class MainWindowViewModel
    {
        public ObservableCollection<ShowViewMenuDuJour> MenusAff;
        public ObservableCollection<ShowViewEvenement> EvenementsAff;
        public ObservableCollection<ShowPersonne> PersonnesAff;
        public List<C_ViewMenuDuJour> Menus;
        public List<C_ViewEvenement> Evenements;
        public List<C_Personne> Personnes;

        private bool IsAllItemsEvenementsSelected, IsAllItemsPersonnesSelected, IsAllItemsMenuDuJourSelected;

        public ConfigClass config;

        public MainWindowViewModel() { }

        public void ChargerDonnees()
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
        }
    }
}
