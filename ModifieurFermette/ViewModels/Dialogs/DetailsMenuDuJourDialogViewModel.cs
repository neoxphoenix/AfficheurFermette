using ShowableData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModifieurFermette.ViewModels.Dialogs
{
    class DetailsMenuDuJourDialogViewModel : ObservableData
    {
        private string _Date, _Potage, _Plat, _Dessert;
        private int _ID;

        public DetailsMenuDuJourDialogViewModel(ShowViewMenuDuJour menu)
        {
            ID = menu.ID;
            Date = menu.Date;
            Potage = menu.eNom;
            Plat = menu.pNom;
            Dessert = menu.dNom;
        }

        public string Date
        {
            get { return _Date; }
            set
            {
                _Date = value;
                OnPropertyChanged();
            }
        }
        public string Potage
        {
            get { return _Potage; }
            set
            {
                _Potage = value;
                OnPropertyChanged();
            }
        }
        public string Plat
        {
            get { return _Plat; }
            set
            {
                _Plat = value;
                OnPropertyChanged();
            }
        }
        public string Dessert
        {
            get { return _Dessert; }
            set
            {
                _Dessert = value;
                OnPropertyChanged();
            }
        }
        public int ID
        {
            get { return _ID; }
            set
            {
                _ID = value;
                OnPropertyChanged();
            }
        }
    }
}
