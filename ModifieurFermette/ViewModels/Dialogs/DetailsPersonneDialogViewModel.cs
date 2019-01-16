using ShowableData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ModifieurFermette.ViewModels.Dialogs
{
    class DetailsPersonneDialogViewModel : ObservableData
    {
        private int _ID;
        private string _Nom, _Prenom, _Date, _Role;
        private BitmapImage _Photo;

        public DetailsPersonneDialogViewModel(ShowPersonne personne)
        {
            ID = personne.ID;
            Nom = personne.Nom;
            Prenom = personne.Prenom;
            Date = personne.DateNaissance;
            Role = personne.Role;
            if (File.Exists(personne.Photo))
                Photo = new BitmapImage(new Uri(personne.Photo));
            else
                Photo = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + @"Resources\Images\errorimg.png"));
        }

        public int ID
        {
            get { return _ID; }
            set
            {
                if (_ID != value)
                {
                    _ID = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Nom
        {
            get { return _Nom; }
            set
            {
                if (_Nom != value)
                {
                    _Nom = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Prenom
        {
            get { return _Prenom; }
            set
            {
                if (_Prenom != value)
                {
                    _Prenom = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Date
        {
            get { return _Date; }
            set
            {
                if (_Date != value)
                {
                    _Date = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Role
        {
            get { return _Role; }
            set
            {
                if (_Role != value)
                {
                    _Role = value;
                    OnPropertyChanged();
                }
            }
        }
        public BitmapImage Photo
        {
            get { return _Photo; }
            set
            {
                if (_Photo != value)
                {
                    _Photo = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
