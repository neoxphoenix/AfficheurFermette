using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using Projet_AFFICHEURFERMETTE.MDF.Classes;
using Projet_AFFICHEURFERMETTE.MDF.Gestion;
using ShowableData;

namespace ModifieurFermette.ViewModels.Dialogs
{
    public class AddPersonneDialogViewModel : ObservableData
    {
        /* ===== Affichage ===== */
        private string _BtnPicText;

        /* ===== Données entrées par l'user ===== */
        private string _Nom, _Prenom, _PicFullPath;
        private DateTime _Date;
        private bool _SelectedRole;

        /* ===== Validation ===== */
        private bool _Validated;

        /* ===== Commandes ===== */
        private ICommand _SelectPic;

        public AddPersonneDialogViewModel()
        {
            SelectPic = new RelayCommand(Exec => ExecuteSelectPic());
            BtnPicText = "Sélectionner une photo de profil";
            Date = DateTime.Parse("1990-01-01");
        }

        private void ExecuteSelectPic()
        {
            OpenFileDialog PicDlg = new OpenFileDialog
            {
                Filter = "Photo (*.PNG;*.JPG;*.jpeg)|*.PNG;*.JPG;*.jpeg"
            };
            if (PicDlg.ShowDialog() == true)
            {
                PicFullPath = PicDlg.FileName;
                BtnPicText = System.IO.Path.GetFileNameWithoutExtension(PicDlg.FileName) + " sélectionnée";
            }
        }

        private void IsAllItemsValid()
        {
            if (String.IsNullOrWhiteSpace(Nom) || String.IsNullOrWhiteSpace(Prenom) || String.IsNullOrWhiteSpace(PicFullPath))
                Validated = false;
            else
                Validated = true;
        }

        public string BtnPicText
        {
            get { return _BtnPicText; }
            set
            {
                if (this._BtnPicText != value)
                {
                    this._BtnPicText = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Nom
        {
            get { return _Nom; }
            set
            {
                if (this._Nom != value)
                {
                    this._Nom = value;
                    OnPropertyChanged();
                    IsAllItemsValid();
                }
            }
        }
        public string Prenom
        {
            get { return _Prenom; }
            set
            {
                if (this._Prenom != value)
                {
                    this._Prenom = value;
                    OnPropertyChanged();
                    IsAllItemsValid();
                }
            }
        }
        public string PicFullPath
        {
            get { return _PicFullPath; }
            set
            {
                if (this._PicFullPath != value)
                {
                    this._PicFullPath = value;
                    IsAllItemsValid();
                }
            }
        }
        public DateTime Date
        {
            get { return _Date; }
            set
            {
                if (this._Date != value)
                {
                    this._Date = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool SelectedRole
        {
            get { return _SelectedRole; }
            set
            {
                if (this._SelectedRole != value)
                {
                    this._SelectedRole = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool Validated
        {
            get { return _Validated; }
            set
            {
                if (this._Validated != value)
                {
                    this._Validated = value;
                    OnPropertyChanged();
                }
            }
        }
        public ICommand SelectPic
        {
            get
            {
                return _SelectPic;
            }
            set
            {
                _SelectPic = value;
            }
        }
    }
}
