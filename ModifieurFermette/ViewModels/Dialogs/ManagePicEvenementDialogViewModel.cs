using Microsoft.Win32;
using Projet_AFFICHEURFERMETTE.MDF.Classes;
using Projet_AFFICHEURFERMETTE.MDF.Gestion;
using ShowableData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModifieurFermette.ViewModels.Dialogs
{
    class ManagePicEvenementDialogViewModel : ObservableData
    {
        // === Liste des photos ===
        private ObservableCollection<C_PhotoEvenement> _PhotosEvenement;

        // == Ajout d'une photo ===
        private ICommand _AddPicCmd;

        // === Retrait d'une photo ===
        // La combobox utilise PhotosEvenement comme liste
        private C_PhotoEvenement _SelectedPhotoToRemove;
        private ICommand _RemovePicCmd;

        // === Manipulation DB ===
        private readonly string sChConn;
        private readonly int IDevenement;

        public ManagePicEvenementDialogViewModel(ShowViewEvenement evenement, string sChConn)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait; // Curseur d'attente

            this.sChConn = sChConn;
            this.IDevenement = evenement.ID;
            PhotosEvenement = new ObservableCollection<C_PhotoEvenement>(new G_ViewEvenement(sChConn).LirePhotosEvenement(evenement.ID));

            AddPicCmd = new RelayCommand(Exec => ExecuteAddPic());
            RemovePicCmd = new RelayCommand(Exec => ExecuteRemovePic(), CanExec => CanExecRemovePic());

            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow; // Retour au curseur normal
        }

        private void ExecuteAddPic()
        {
            OpenFileDialog PicDlg = new OpenFileDialog
            {
                Filter = "Photo (*.PNG;*.JPG;*.jpeg)|*.PNG;*.JPG;*.jpeg"
            };
            if (PicDlg.ShowDialog() == true)
            {
                // Sauvegarde de la photo dans le dossier "~\Images\Evenements\"
                string PicFullPath = PicDlg.FileName;
                string FileName = Path.GetFileName(PicFullPath); // On récupère uniquement le nom du fichier et son extension du chemin entré dans le dialog
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\Images\\Evenements"); // On génère le chemin du dossier "~\Images\Evenements\"
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
                File.Copy(PicFullPath, path); // Et on copie le fichier sélectionné dans "~\Images\Personnes\"

                // MàJ de la DB
                int ID = new G_PhotoEvenement(sChConn).Ajouter(this.IDevenement, path, false);
                // MàJ locale
                PhotosEvenement.Add(new C_PhotoEvenement(ID, this.IDevenement, path, false));
            }
        }

        private bool CanExecRemovePic()
        {
            return SelectedPhotoToRemove != null;
        }

        private void ExecuteRemovePic()
        {
            try
            {
                // Suppression dans le dossier
                File.Delete(SelectedPhotoToRemove.Photo);
            }
            catch { }
            // MàJ DB
            new G_PhotoEvenement(sChConn).Supprimer(SelectedPhotoToRemove.ID);
            // MàJ locale
            PhotosEvenement.Remove(SelectedPhotoToRemove);
            SelectedPhotoToRemove = null;
        }

        #region Accesseurs
        public ObservableCollection<C_PhotoEvenement> PhotosEvenement
        {
            get => _PhotosEvenement;
            set
            {
                if (_PhotosEvenement != value)
                {
                    _PhotosEvenement = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand AddPicCmd { get => _AddPicCmd; set => _AddPicCmd = value; }

        public C_PhotoEvenement SelectedPhotoToRemove
        {
            get => _SelectedPhotoToRemove;
            set
            {
                if (_SelectedPhotoToRemove != value)
                {
                    _SelectedPhotoToRemove = value;
                    OnPropertyChanged();
                }
            }
        }
        public ICommand RemovePicCmd { get => _RemovePicCmd; set => _RemovePicCmd = value; }
        #endregion
    }
}
