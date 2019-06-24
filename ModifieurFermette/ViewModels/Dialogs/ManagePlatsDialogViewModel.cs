using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShowableData;
using ModifieurFermette.Models;
using Projet_AFFICHEURFERMETTE.MDF.Classes;
using Projet_AFFICHEURFERMETTE.MDF.Gestion;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
using Microsoft.Win32;

namespace ModifieurFermette.ViewModels.Dialogs
{
    class ManagePlatsDialogViewModel : ObservableData
    {
        private ObservableCollection<ExtendedPlat> _Plats;
        private ExtendedPlat _SelectedPlat;
        private BitmapImage _Photo;
        private string _ChangePicTxt;
        private ICommand _DeletePlatCmd, _ChangePicCmd;
        private readonly string sChConn;

        public ManagePlatsDialogViewModel(string sChConn)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait; // Curseur d'attente
            this.sChConn = sChConn;
            List<C_Plat> TmpPlats = new G_Plat(sChConn).Lire("");
            List<C_Menu> TmpMenus = new G_Menu(sChConn).Lire("");
            Plats = new ObservableCollection<ExtendedPlat>();
            foreach(C_Plat plat in TmpPlats)
            {
                // Ajoute les plats à l'observableCollection en précisant si ce plat est utilisé ou non dans un menu
                Plats.Add(new ExtendedPlat(plat, TmpMenus.Any(menu => menu.IDpotage == plat.ID || menu.IDplat == plat.ID || menu.IDdessert == plat.ID)));
            }
            ChangePicTxt = "Veuillez sélectionner un plat";
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;

            DeletePlatCmd = new RelayCommand(Exec => ExecuteDeletePlat(), CanExec => CanExecDeletePlat());
            ChangePicCmd = new RelayCommand(Exec => ExecuteChangePic(), CanExec => CanExecChangePic());
        }

        private void ExecuteChangePic()
        {
            OpenFileDialog PicDlg = new OpenFileDialog
            {
                Filter = "Photo (*.PNG;*.JPG;*.jpeg)|*.PNG;*.JPG;*.jpeg"
            };
            if (PicDlg.ShowDialog() == true)
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait; // Curseur d'attente

                // Supprime l'éventuelle photo précédente
                Photo = null;
                if (File.Exists(SelectedPlat.Photo))
                    File.Delete(SelectedPlat.Photo);
                // copie la nouvelle photo dans le répertoire du programme
                SelectedPlat.Photo = SaveFile(PicDlg.FileName);
                new G_Plat(sChConn).Modifier(SelectedPlat.ID, SelectedPlat.nom, SelectedPlat.Type, SelectedPlat.Photo);
                ChangePic(SelectedPlat.Photo);
                ChangePicTxt = "Changer de photo";

                Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
            }
        }

        private string SaveFile(string PathToPic)
        {
            string FileName = Path.GetFileName(PathToPic); // On récupère uniquement le nom du fichier et son extension du chemin entré dans le dialog
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\Images\\Plats"); // On génère le chemin du dossier "~\Images\Plats\"
            Directory.CreateDirectory(path); // Si les dossiers n'existent pas encore, ils sont créés
            path = Path.Combine(path, FileName); // On rajoute le nom du fichier au path
                                                 // Vérification qu'un fichier du même nom n'existe pas déjà
            string TestPath = path;
            int Count = 0;
            while (File.Exists(TestPath))
            {
                string tempFileName = string.Format("{0}({1})", Path.GetFileNameWithoutExtension(path), Count++);
                TestPath = Path.Combine(Path.GetDirectoryName(path), tempFileName + Path.GetExtension(path));
            }
            path = TestPath;
            File.Copy(PathToPic, path); // Et on copie le fichier sélectionné dans "~\Images\Personnes\"
            return path;
        }

        private void ChangePic(string filePath) // permet d'afficher une photo mais de pouvoir en supprimer le fichier par après
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(filePath);
            image.EndInit();
            Photo = image;
        }

        private bool CanExecChangePic()
        {
            return SelectedPlat != null;
        }

        private bool CanExecDeletePlat()
        {
            return SelectedPlat != null && !SelectedPlat.IsUsed;
        }

        private void ExecuteDeletePlat()
        {
            new G_Plat(sChConn).Supprimer(SelectedPlat.ID);
            // Supprime la photo associée
            Photo = null;
            if (File.Exists(SelectedPlat.Photo))
                File.Delete(SelectedPlat.Photo);
            Plats.Remove(SelectedPlat);
            SelectedPlat = null;
        }

        public ObservableCollection<ExtendedPlat> Plats
        {
            get { return _Plats; }
            set
            {
                if (this._Plats != value)
                {
                    this._Plats = value;
                    OnPropertyChanged();
                }
            }
        }
        public ExtendedPlat SelectedPlat
        {
            get { return _SelectedPlat; }
            set
            {
                if (_SelectedPlat != value)
                {
                    _SelectedPlat = value;
                    OnPropertyChanged();
                    if (_SelectedPlat == null)
                    {
                        ChangePicTxt = "Veuillez sélectionner un plat";
                        Photo = null;
                    }
                    else if (String.IsNullOrEmpty(_SelectedPlat.Photo))
                    {
                        ChangePicTxt = "Ajouter une photo";
                        Photo = null;
                    }
                    else if (File.Exists(_SelectedPlat.Photo))
                    {
                        ChangePicTxt = "Changer de photo";
                        ChangePic(_SelectedPlat.Photo);
                    }
                    else
                    {
                        ChangePicTxt = "Image introuvable... Cliquez pour ajouter une photo";
                        ChangePic(System.AppDomain.CurrentDomain.BaseDirectory + @"Resources\Images\errorimg.png");
                    }
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
        public ICommand DeletePlatCmd
        {
            get { return _DeletePlatCmd; }
            set { _DeletePlatCmd = value; }
        }

        public ICommand ChangePicCmd { get => _ChangePicCmd; set => _ChangePicCmd = value; }
        public string ChangePicTxt { get => _ChangePicTxt; set { _ChangePicTxt = value; OnPropertyChanged(); } }
    }
}
