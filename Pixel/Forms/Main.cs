using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Pixel.Common;
using Pixel.Encryption;
using Pixel.Extensions;
using Pixel.Util;
using Unsplasharp;
using Unsplasharp.Models;

namespace Pixel.Forms
{
    public partial class Main : Form
    {
        private readonly UnsplasharpClient _client = new UnsplasharpClient(AES.Decrypt(
            "/JKkbIdAWvZXGGQk5hcjtonTgGYPeRHOBrR0ZreaCroiLWfgcPF0qv/Gl/gmlBkD/B8eSPVjkD4YjZe8tGiRWTuHWe8am/v8l4mGtF1zAAI=",
            $"{Application.CompanyName}\\{Application.ProductName}"));

        private List<Photo> _photos;
        private readonly List<Image> _images;
        private Color _borderColor;
        private int _borderThickness;
        private int _pageCounter = 1;
        private int _photoLabelId;
        private bool _isSearched;
        private string _searchQuery;
        private bool _isShowFavoritesButtonClicked;
        private List<string> _favorites;
        private int _favoritesCounter;

        private IEnumerable<Label> GetPhotoLabels()
        {
            return Controls.OfType<Label>()
                .Where(control => control.Name.StartsWith("photoLabel"))
                .ToList();
        }
        
        private void DisplayPhotos()
        {
            Dictionary.Clear();
            statusBar.Text = null;
            _photoLabelId = 6;
            var photoLabels = GetPhotoLabels();
            
            foreach (var (photoLabel, i) in IndexExtension.LoopIndex(photoLabels))
            {
                try
                {
                    photoLabel.Image = _images.Count > 0 ? _images[i] : Task.Run(() => HttpUtil.StreamUrlToImage(_photos[i].Urls.Small)).Result;
                    
                    if (!_isShowFavoritesButtonClicked)
                    {
                        if (_photos[i] != null)
                        {
                            if (_favorites != null)
                            {
                                Dictionary.Put(i, _favorites.All(favoriteId => favoriteId != _photos[i].Id));
                            }
                        }
                    }
                }
                catch
                {
                    photoLabel.Image = ImageUtil.ResizeImageAndKeepRatio((Image) _resources.GetObject("$this.noPhoto"), 300, 200);
                }
            }

            if (_images.Count > 0) { _images.Clear(); }
            EnableControls(true);
        }

        private void DisplayPhotoInfo()
        {
            if (_photos.ElementAtOrDefault(_photoLabelId) != null)
            {
                var photo = _photos[_photoLabelId];
                statusBar.Text = "	" + $"{photo.Description.Truncate(90, "...")} — by {photo.User.Name} — {photo.Width}(w)*{photo.Height}(h)";
            }
            else
            {
                statusBar.Text = null;
            }
        }

        private async void GetPhotos(int page)
        {
            EnableControls(false);
            addRemoveFavoriteButton.Enabled = true;
            _photos = await _client.ListPhotos(page, 6);
            DisplayPhotos();
        }

        private async void GetRandomPhotos()
        {
            EnableControls(false);
            _pageCounter = 1;
            _isShowFavoritesButtonClicked = false;
            addRemoveFavoriteButton.Image = ImageUtil.ResizeImageAndKeepRatio((Image) _resources.GetObject("$this.add"), 12, 12);
            _photos.Clear();

            var ids = new List<string>();
            while (ids.Count < 6)
            {
                var photo = await _client.GetRandomPhoto();
                while (!ids.Contains(photo.Id))
                {
                    ids.Add(photo.Id);
                    break;
                }
            }

            foreach (var photoId in ids)
            {
                _photos.Add(Task.Run(() => GetPhoto(photoId)).Result);
            }
            
            DisplayPhotos();
        }

        private async void GetSearchedPhotos(int page)
        {
            EnableControls(false);
            _photos = await _client.SearchPhotos(_searchQuery, page, 6);
            DisplayPhotos();
        }
        
        private async Task<Photo> GetPhoto(string id)
        {
            return await _client.GetPhoto(id);
        }

        private void RefreshPhotoLabel(string labelName)
        {
            var photoLabels = GetPhotoLabels();
            
            foreach (var photoLabel in photoLabels)
            {
                if (photoLabel.Name == labelName)
                {
                    _borderColor = Color.Orange;
                    _borderThickness = 2;
                }
                else
                {
                    _borderColor = Color.Transparent;
                    _borderThickness = 0;
                }
                
                photoLabel.Invalidate();
                photoLabel.Update();
            }
        }
        
        private void RefreshPhotoLabels()
        {
            var photoLabels = GetPhotoLabels();
            
            foreach (var photoLabel in photoLabels)
            {
                _borderColor = Color.Transparent;
                _borderThickness = 0;
                photoLabel.Invalidate();
                photoLabel.Update();
            }
        }

        private static void PrintPhotoLabelBorder(Label photoLabel, Color borderColor, int borderThickness, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics,
                photoLabel.ClientRectangle, borderColor, borderThickness,
                ButtonBorderStyle.Solid, borderColor, borderThickness,
                ButtonBorderStyle.Solid, borderColor, borderThickness,
                ButtonBorderStyle.Solid, borderColor, borderThickness,
                ButtonBorderStyle.Solid);
        }

        private void CountFavorites()
        {
            if (Favorites.CountFavorites() > 0)
            {
                _favorites = Favorites.ReadFavorites();
                showFavoritesButton.Text = $"Favorites ({Favorites.CountFavorites()})";
            }
            else
            {
                if (_isShowFavoritesButtonClicked)
                {
                    showFavoritesButton.Text = "Favorites";
                    _photos.Clear();
                    RefreshPhotoLabels();
                    GetPhotos(1);
                    _isShowFavoritesButtonClicked = false;
                    addRemoveFavoriteButton.Image = ImageUtil.ResizeImageAndKeepRatio((Image) _resources.GetObject("$this.add"), 12, 12);
                }
            }
        }
        
        private void ShowFavorites()
        {
            EnableControls(false);
            _photos.Clear();
            var i = 0;
            
            for (var f = _favoritesCounter; f < _favorites.Count; f++)
            {
                if (i == 6)
                {
                    break;
                }
                
                _photos.Add(Task.Run(() => GetPhoto(_favorites[f])).Result);
                _favoritesCounter++;
                i++;
            }
            
            DisplayPhotos();
        }

        private void CheckFavorite()
        {
            if (!_isShowFavoritesButtonClicked)
            {
                if (Favorites.CountFavorites() > 0)
                {
                    if (_photos[_photoLabelId] != null)
                    {
                        addRemoveFavoriteButton.Enabled = _favorites.All(favoriteId => favoriteId != _photos[_photoLabelId].Id);
                    }
                }
            }
        }
        
        private void CreateContextMenu()
        {
            var photoContextMenu = new ContextMenu();
            photoContextMenu.MenuItems.Add("Preview photo", previewPhotoMenuItem_Click);
            photoContextMenu.MenuItems.Add("View photo", viewPhotoMenuItem_Click);

            var photoLabels = GetPhotoLabels();
            foreach (var photoLabel in photoLabels)
            {
                photoLabel.ContextMenu = photoContextMenu;
            }
            
            var refreshPhotosButtonContextMenu = new ContextMenu();
            refreshPhotosButtonContextMenu.MenuItems.Add("Go to first page", goToFirstPageMenuItem_Click);
            refreshPhotosButtonContextMenu.MenuItems.Add("Delete temporary 'photos' directory", deleteTempDirMenuItem_Click);
            refreshPhotosButtonContextMenu.MenuItems.Add("Refresh your favorites", refreshFavoritesMenuItem_Click);
            refreshPhotosButton.ContextMenu = refreshPhotosButtonContextMenu;
            
            var statusBarContextMenu = new ContextMenu();
            statusBarContextMenu.MenuItems.Add("Show full description", showFullDescriptionMenuItem_Click);
            statusBar.ContextMenu = statusBarContextMenu;
        }
        
        private void EnableControls(bool state)
        {
            previousPageButton.Enabled = state;
            nextPageButton.Enabled = state;
            refreshPhotosButton.Enabled = state;
            addRemoveFavoriteButton.Enabled = state;
            showFavoritesButton.Enabled = state;
            searchPhotosTextBox.Focus();
        }
        
        //
        
        private void searchPhotosTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            _photos.Clear();
            RefreshPhotoLabels();
            _pageCounter = 1;
            
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                _searchQuery = searchPhotosTextBox.Text;
                GetSearchedPhotos(_pageCounter);
                _isSearched = true;
                _isShowFavoritesButtonClicked = false;
                addRemoveFavoriteButton.Image = ImageUtil.ResizeImageAndKeepRatio((Image) _resources.GetObject("$this.add"), 12, 12);
            }
        }

        private void previousPageButton_Click(object sender, EventArgs e)
        {
            RefreshPhotoLabels();

            if (_isShowFavoritesButtonClicked)
            {
                if (_favorites.Count > 6)
                {
                    if (_favoritesCounter == 6)
                    {
                        previousPageButton.Enabled = false;
                        nextPageButton.Select();
                    }
                    else
                    {
                        nextPageButton.Enabled = true;
                        _favoritesCounter -= _photos.Count + 6;
                        ShowFavorites();
                    }
                }
                else
                {
                    showFavoritesButton.Select();
                }
            }
            else
            {
                if (_pageCounter == 1)
                {
                    refreshPhotosButton.Select();
                }
                else
                {
                    if (_isSearched)
                    {
                        GetSearchedPhotos(--_pageCounter);
                    }
                    else
                    {
                        GetPhotos(--_pageCounter);
                    }
                }
            }
        }

        private void nextPageButton_Click(object sender, EventArgs e)
        {
            RefreshPhotoLabels();

            if (_isSearched)
            {
                GetSearchedPhotos(++_pageCounter);
            }
            else
            {
                if (_isShowFavoritesButtonClicked)
                {
                    if (!previousPageButton.Enabled)
                    {
                        previousPageButton.Enabled = true;
                    }

                    if (_favoritesCounter != _favorites.Count)
                    {
                        ShowFavorites();
                    }
                    else
                    {
                        nextPageButton.Enabled = false;
                        previousPageButton.Select();
                    }
                }
                else
                {
                    GetPhotos(++_pageCounter);
                }
            }
        }
        
        private void refreshPhotosButton_Click(object sender, EventArgs e)
        {
            RefreshPhotoLabels();
            GetRandomPhotos();
        }
        
        private void addRemoveFavoriteButton_Click(object sender, EventArgs e)
        {
            if (!_isShowFavoritesButtonClicked)
            {
                if (_photos.ElementAtOrDefault(_photoLabelId) != null)
                {
                    var id = _photos[_photoLabelId].Id;
                    Favorites.AddFavorite(id);
                    CountFavorites();
                    CheckFavorite();
                }
                else
                {
                    MessageBox.Show("Please to select a photo.",
                        $"—{Application.ProductName}—", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                if (_photos.ElementAtOrDefault(_photoLabelId) != null)
                {
                    Favorites.RemoveFavorite(_photos[_photoLabelId].Id);
                    CountFavorites();
                    RefreshPhotoLabels();
                }
                else
                {
                    MessageBox.Show("Please to select a photo.",
                        $"—{Application.ProductName}—", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void showFavoritesButton_Click(object sender, EventArgs e)
        {
            _favoritesCounter = 0;
            _favorites = Favorites.ReadFavorites();
            _isSearched = false;
            RefreshPhotoLabels();
            
            if (_favorites.Count > 0)
            {
                _photos.Clear();
                _isShowFavoritesButtonClicked = true;
                addRemoveFavoriteButton.Image = ImageUtil.ResizeImageAndKeepRatio((Image) _resources.GetObject("$this.remove"), 12, 12);
                addRemoveFavoriteButton.Enabled = true;
                _favorites.Reverse();
                ShowFavorites();
            }
        }

        private void aboutAppButton_Click(object sender, EventArgs e)
        {
            var logoLabel = new Label
            {
                Location = new Point(5, 40),
                Size = new Size(376, 128),
                ImageAlign = ContentAlignment.MiddleCenter,
                Image = ImageUtil.ResizeImageAndKeepRatio((Image) _resources.GetObject("$this.appLogo"), 128, 128)
            };
            
            var infoLabel = new Label
            {
                Location = new Point(5, 200),
                Size = new Size(376, 200),
                Font = new Font("Segoe UI", 8F, FontStyle.Regular),
                TextAlign = ContentAlignment.TopCenter,
                Text = $"{Application.ProductName} {Application.ProductVersion.Remove(3)}\r\n\r\n" +
                       "Made with love for all by skyffx\r\n" +
                       "(Wojciech Piekielniak, wojciech.piekielniak@protonmail.com)\r\n\r\n" +
                       "github.com/skyffx/Pixel\r\n\r\n" +
                       "This amazing app is based on:\r\n" +
                       "Unsplasharp — github.com/rootasjey/unsplasharp\r\n" +
                       "TinyJson — github.com/zanders3/json\r\n\r\n" +
                       "App icon from flaticon.com\r\n\r\n\r\n" +
                       "—2020—"
            };
            
            var about = new Form
            {
                Controls = { logoLabel, infoLabel },
                BackColor = Color.White,
                MaximizeBox = false,
                MinimizeBox = false,
                ShowIcon = false,
                ShowInTaskbar = false,
                Text = $"—{Application.ProductName}—",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Size = new Size(400, 480),
            };
            
            about.ShowDialog();
        }

        //

        private void photoLabel1_Click(object sender, EventArgs e)
        {
            _photoLabelId = 0;
            RefreshPhotoLabel(photoLabel1.Name);
            DisplayPhotoInfo();
            CheckFavorite();
        }
        
        private void photoLabel2_Click(object sender, EventArgs e)
        {
            _photoLabelId = 1;
            RefreshPhotoLabel(photoLabel2.Name);
            DisplayPhotoInfo();
            CheckFavorite();
        }
        
        private void photoLabel3_Click(object sender, EventArgs e)
        {
            _photoLabelId = 2;
            RefreshPhotoLabel(photoLabel3.Name);
            DisplayPhotoInfo();
            CheckFavorite();
        }
        
        private void photoLabel4_Click(object sender, EventArgs e)
        {
            _photoLabelId = 3;
            RefreshPhotoLabel(photoLabel4.Name);
            DisplayPhotoInfo();
            CheckFavorite();
        }
        
        private void photoLabel5_Click(object sender, EventArgs e)
        {
            _photoLabelId = 4;
            RefreshPhotoLabel(photoLabel5.Name);
            DisplayPhotoInfo();
            CheckFavorite();
        }
        
        private void photoLabel6_Click(object sender, EventArgs e)
        {
            _photoLabelId = 5;
            RefreshPhotoLabel(photoLabel6.Name);
            DisplayPhotoInfo();
            CheckFavorite();
        }
        
        //
        
        private void PrintFavoriteStar(PaintEventArgs e)
        {
            e.Graphics.DrawImage(new Bitmap((Image) _resources.GetObject("$this.star")), 266, 10);
        }

        private void photoLabel1_Paint(object sender, PaintEventArgs e)
        {
            PrintPhotoLabelBorder(photoLabel1, _borderColor, _borderThickness, e);
            if (!Dictionary.Get(0)) { PrintFavoriteStar(e); }
        }
        
        private void photoLabel2_Paint(object sender, PaintEventArgs e)
        {
            PrintPhotoLabelBorder(photoLabel2, _borderColor, _borderThickness, e);
            if (!Dictionary.Get(1)) { PrintFavoriteStar(e); }
        }
        
        private void photoLabel3_Paint(object sender, PaintEventArgs e)
        {
            PrintPhotoLabelBorder(photoLabel3, _borderColor, _borderThickness, e);
            if (!Dictionary.Get(2)) { PrintFavoriteStar(e); }
        }
        
        private void photoLabel4_Paint(object sender, PaintEventArgs e)
        {
            PrintPhotoLabelBorder(photoLabel4, _borderColor, _borderThickness, e);
            if (!Dictionary.Get(3)) { PrintFavoriteStar(e); }
        }
        
        private void photoLabel5_Paint(object sender, PaintEventArgs e)
        {
            PrintPhotoLabelBorder(photoLabel5, _borderColor, _borderThickness, e);
            if (!Dictionary.Get(4)) { PrintFavoriteStar(e); }
        }
        
        private void photoLabel6_Paint(object sender, PaintEventArgs e)
        {
            PrintPhotoLabelBorder(photoLabel6, _borderColor, _borderThickness, e);
            if (!Dictionary.Get(5)) { PrintFavoriteStar(e); }
        }
        
        //
        
        private void previewPhotoMenuItem_Click(object sender, EventArgs e)
        {
            if (_photos.ElementAtOrDefault(_photoLabelId) != null)
            {
                var previewPhoto = Task.Run(() => HttpUtil.StreamUrlToImage(_photos[_photoLabelId].Urls.Small)).Result;
                var previewLabel = new Label
                {
                    Location = new Point(0, 0),
                    Size = new Size(previewPhoto.Width, previewPhoto.Height),
                    Image = previewPhoto,
                    Dock = DockStyle.Fill
                };
                var preview = new Form
                {
                    Controls = { previewLabel },
                    BackColor = Color.White,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    ShowIcon = false,
                    ShowInTaskbar = false,
                    Text = $"—{Application.ProductName}—",
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    Size = new Size(previewPhoto.Width, previewPhoto.Height)
                };
                preview.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please to select a photo.",
                    $"—{Application.ProductName}—", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
        private void viewPhotoMenuItem_Click(object sender, EventArgs e)
        {
            if (_photos.ElementAtOrDefault(_photoLabelId) != null)
            {
                Directory.CreateDirectory("photos");
                var url = _photos[_photoLabelId].Urls.Raw;
                var destination = $"photos\\{_photos[_photoLabelId].Id}.jpg";
                var progress = new Progress<float> ();
                progress.ProgressChanged += progress_ProgressChanged;
                HttpUtil.DownloadFile(url, destination, progress, true);
            }
            else
            {
                MessageBox.Show("Please to select a photo.",
                    $"—{Application.ProductName}—", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
        private void goToFirstPageMenuItem_Click(object sender, EventArgs e)
        {
            addRemoveFavoriteButton.Image = ImageUtil.ResizeImageAndKeepRatio((Image) _resources.GetObject("$this.add"), 12, 12);
            _isShowFavoritesButtonClicked = false;
            _isSearched = false;
            _pageCounter = 1;
            GetPhotos(1);
        }

        private void deleteTempDirMenuItem_Click(object sender, EventArgs e)
        {
            if (Directory.Exists("photos"))
            {
                /*var dirSize = Directory.GetFiles("photos", "*", SearchOption.AllDirectories)
                    .Sum(t => new FileInfo(t).Length);*/
                
                var selectedOption = MessageBox.Show("Are you sure?",
                    $"—{Application.ProductName}—", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (selectedOption == DialogResult.Yes)
                {
                    Directory.Delete("photos", true);
                    statusBar.Text = "	" + "— Success —";
                }
            }
        }

        private void refreshFavoritesMenuItem_Click(object sender, EventArgs e)
        {
            if (_favorites.Count > 0)
            {
                File.Copy("favorites.json", "favorites.json.bak");
                var tempFavorites = _favorites.ToList();
                _favorites.Clear();
                float max = tempFavorites.Count;
                float current = 1;

                foreach (var favorite in tempFavorites)
                {
                    try
                    {
                        var id = Task.Run(() => GetPhoto(favorite).Result.Id);
                        if (!string.IsNullOrEmpty(id.Result))
                        {
                            _favorites.Add(id.Result);
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                
                    statusBar.Text = "	" + $"— {(int)((current++ / max) * 100f)}% —";
                }
                Favorites.WriteFavorites(_favorites);
                CountFavorites();
                if (File.Exists("favorites.json.bak"))
                {
                    File.Delete("favorites.json.bak");
                }
            }
        }

        private void showFullDescriptionMenuItem_Click(object sender, EventArgs e)
        {
            var photoDescription = _photos[_photoLabelId].Description;
            if (!string.IsNullOrWhiteSpace(photoDescription))
            {
                MessageBox.Show(photoDescription, $"—{Application.ProductName}—");
            }
            else
            {
                MessageBox.Show("Selected photo does not have description!", $"—{Application.ProductName}—",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
        private void progress_ProgressChanged(object sender, float progress)
        {
            statusBar.Text = "	" + $"— {(int)progress}% —";
        }

        //
        
        private void ShowTips()
        {
            var toolTip = new ToolTip {AutoPopDelay = 4000, InitialDelay = 100, ReshowDelay = 100, ShowAlways = true};
            toolTip.SetToolTip(refreshPhotosButton, "Show new random photos\nMouse right click for more options");
            toolTip.SetToolTip(previousPageButton, "Go to previous page");
            toolTip.SetToolTip(nextPageButton, "Go to next page");
            toolTip.SetToolTip(addRemoveFavoriteButton, "Add photo to favorites\nRemove photo from favorites");
            toolTip.SetToolTip(showFavoritesButton, "Show saved favorites");
            toolTip.SetToolTip(aboutAppButton, $"About {Application.ProductName}");
        }
        
        public Main(List<Photo> photos, List<Image> images)
        {
            CenterToScreen();
            InitializeComponent();
            SendMessage(searchPhotosTextBox.Handle, EM_SETCUEBANNER, 1, "Search photos (type words and hit Enter)");
            CreateContextMenu();
            _photos = photos;
            _images = images;
            DisplayPhotos();
            CountFavorites();
            ShowTips();
            Select();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)]string lParam);
        private const int EM_SETCUEBANNER = 0x1501; 
    }
}