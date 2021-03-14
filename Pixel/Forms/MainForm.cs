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
using Pixel.Utils;
using Unsplasharp;
using Unsplasharp.Models;

namespace Pixel.Forms
{
    public partial class MainForm : Form
    {
        private readonly UnsplasharpClient _client = new UnsplasharpClient(AES.Decrypt(
            "/JKkbIdAWvZXGGQk5hcjtonTgGYPeRHOBrR0ZreaCroiLWfgcPF0qv/Gl/gmlBkD/B8eSPVjkD4YjZe8tGiRWTuHWe8am/v8l4mGtF1zAAI=",
            $"{Application.CompanyName}\\{Application.ProductName}"));

        private List<Photo> _photos;
        private readonly List<Image> _images;
        private Color _borderColor;
        private int _borderThickness;
        private int _mainPageCounter = 1;
        private int _currentPageCounter = 1;
        private int _photoLabelId;
        private bool _isSearched;
        private bool _isAuthorCollectionShown;
        private string _searchQuery;
        private bool _isShowFavoritesButtonClicked;
        private List<string> _favorites;
        private int _favoritesCounter;
        private string _author;
        private bool _isFavoriteSaved;

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
            _photoLabelId = 4;
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
                statusBar.Text = "	" + $"{photo.Description.Truncate(180, "...")} — by {photo.User.Name} — {photo.Width}(w)*{photo.Height}(h)";
            }
            else
            {
                statusBar.Text = null;
            }
        }

        private async void GetPhotos(int page)
        {
            EnableControls(false);
            _photos = await _client.ListPhotos(page, 4);
            DisplayPhotos();
        }

        private async void GetRandomPhotos()
        {
            EnableControls(false);
            _mainPageCounter = 1;
            _photos.Clear();
            _photos = await _client.GetRandomPhoto(4);
            DisplayPhotos();
        }

        private async void GetSearchedPhotos(int page)
        {
            EnableControls(false);
            _photos = await _client.SearchPhotos(_searchQuery, page, 4);
            DisplayPhotos();
        }

        private async void GetAuthorsPhotos(string author, int page)
        {
            EnableControls(false);
            _photos = await _client.ListUserPhotos(author, page, 4);
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
                if (photoLabel.Name.Equals(labelName))
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
                if (i.Equals(4))
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
                    try
                    {
                        if (_photos[_photoLabelId] != null)
                        {
                            _isFavoriteSaved = _favorites.All(favoriteId => favoriteId != _photos[_photoLabelId].Id);

                        }
                    }
                    catch 
                    {
                        // ignored
                    }
                }
            }
        }
        
        private void CreateContextMenu()
        {
            var photoContextMenu = new ContextMenu();
            photoContextMenu.MenuItems.Add("Preview photo", previewPhotoMenuItem_Click);
            photoContextMenu.MenuItems.Add("View photo", viewPhotoMenuItem_Click);
            photoContextMenu.MenuItems.Add("-");
            photoContextMenu.MenuItems.Add("Add photo to favorites", addFavoriteMenuItem_Click);
            photoContextMenu.MenuItems.Add("Remove photo from favorites", removeFavoriteMenuItem_Click);
            photoContextMenu.MenuItems.Add("-");
            photoContextMenu.MenuItems.Add("Show all photos (author)", showAuthorPhotos_Click);

            var photoLabels = GetPhotoLabels();
            foreach (var photoLabel in photoLabels)
            {
                photoLabel.ContextMenu = photoContextMenu;
            }
            
            var mainContextMenu = new ContextMenu();
            mainContextMenu.MenuItems.Add("Go to first page (home)", goToFirstPageMenuItem_Click);
            mainContextMenu.MenuItems.Add("Go to current page (home)", goToPageMenuItem_Click);
            mainContextMenu.MenuItems.Add("Delete 'photos' directory", deleteTempDirMenuItem_Click);
            mainContextMenu.MenuItems.Add("Refresh your favorites", refreshFavoritesMenuItem_Click);
            ContextMenu = mainContextMenu;
            
            var statusBarContextMenu = new ContextMenu();
            statusBarContextMenu.MenuItems.Add("Show full description", showFullDescriptionMenuItem_Click);
            statusBar.ContextMenu = statusBarContextMenu;
        }
        
        private void EnableControls(bool state)
        {
            previousPageLabel.Enabled = state;
            nextPageLabel.Enabled = state;
            showFavoritesButton.Enabled = state;
            searchPhotosTextBox.Focus();
        }
        
        private void searchPhotosTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            _photos.Clear();
            RefreshPhotoLabels();
            _currentPageCounter = 1;
            
            if (e.KeyCode.Equals(Keys.Enter))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                if (searchPhotosTextBox.Text.Equals("#random"))
                {
                    GetRandomPhotos();
                }
                else
                {
                    _searchQuery = searchPhotosTextBox.Text;
                    GetSearchedPhotos(_mainPageCounter);
                    _isSearched = true;
                }
                _isShowFavoritesButtonClicked = false;
            }
        }

        private void previousPageLabel_Click(object sender, EventArgs e)
        {
            RefreshPhotoLabels();

            if (_isShowFavoritesButtonClicked)
            {
                if (_favorites.Count > 4)
                {
                    if (_favoritesCounter.Equals(4))
                    {
                        previousPageLabel.Enabled = false;
                        nextPageLabel.Select();
                    }
                    else
                    {
                        nextPageLabel.Enabled = true;
                        _favoritesCounter -= _photos.Count + 4;
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
                if (_mainPageCounter.Equals(0) || _currentPageCounter.Equals(0))
                {
                    //
                }
                else
                {
                    if (_isSearched)
                    {
                        GetSearchedPhotos(--_currentPageCounter);
                    }
                    else if (_isAuthorCollectionShown)
                    {
                        GetAuthorsPhotos(_author, --_currentPageCounter);
                    }
                    else
                    {
                        GetPhotos(--_mainPageCounter);
                    }
                }
            }
        }

        private void nextPageLabel_Click(object sender, EventArgs e)
        {
            RefreshPhotoLabels();

            if (_isSearched)
            {
                GetSearchedPhotos(++_currentPageCounter);
            }
            else if (_isAuthorCollectionShown)
            {
                GetAuthorsPhotos(_author, ++_currentPageCounter);
            }
            else
            {
                if (_isShowFavoritesButtonClicked)
                {
                    if (!previousPageLabel.Enabled)
                    {
                        previousPageLabel.Enabled = true;
                    }

                    if (_favoritesCounter != _favorites.Count)
                    {
                        ShowFavorites();
                    }
                    else
                    {
                        nextPageLabel.Enabled = false;
                        previousPageLabel.Select();
                    }
                }
                else
                {
                    GetPhotos(++_mainPageCounter);
                }
            }
        }

        private void addFavoriteMenuItem_Click(object sender, EventArgs e)
        {
            if (!_isFavoriteSaved)
            {
                MessageBox.Show("This photo already exists in your collection.",
                    $"—{Application.ProductName}—", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
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
            }
        }

        private void removeFavoriteMenuItem_Click(object sender, EventArgs e)
        {
            if (_isShowFavoritesButtonClicked)
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
            else
            {
                MessageBox.Show("To proceed switch to Favorites.",
                    $"—{Application.ProductName}—", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void showFavoritesButton_Click(object sender, EventArgs e)
        {
            _favoritesCounter = 0;
            _favorites = Favorites.ReadFavorites();
            _isSearched = false;
            _isAuthorCollectionShown = false;
            RefreshPhotoLabels();
            
            if (_favorites.Count > 0)
            {
                _photos.Clear();
                _isShowFavoritesButtonClicked = true;
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
                       "github.com/skyffx/Pixel\r\n\r\n" +
                       "This amazing app is based on:\r\n" +
                       "Unsplasharp — github.com/rootasjey/unsplasharp\r\n" +
                       "TinyJson — github.com/zanders3/json\r\n\r\n" +
                       "App icon from flaticon.com\r\n\r\n\r\n" +
                       "—2021—"
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
                Size = new Size(400, 460),
                StartPosition = FormStartPosition.CenterParent
            };

            Enabled = false;
            about.ShowDialog(this);
            Enabled = true;
        }

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
                    Size = new Size(previewPhoto.Width, previewPhoto.Height),
                    StartPosition = FormStartPosition.CenterParent
                };

                Enabled = false;
                preview.ShowDialog(this);
                Enabled = true;
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
            _isShowFavoritesButtonClicked = false;
            _isSearched = false;
            _isAuthorCollectionShown = false;
            _mainPageCounter = 1;
            GetPhotos(_mainPageCounter);
        }
        
        private void goToPageMenuItem_Click(object sender, EventArgs e)
        {
            _isShowFavoritesButtonClicked = false;
            _isSearched = false;
            _isAuthorCollectionShown = false;
            GetPhotos(_mainPageCounter);
        }

        private void deleteTempDirMenuItem_Click(object sender, EventArgs e)
        {
            if (Directory.Exists("photos"))
            {
                var selectedOption = MessageBox.Show("Are you sure?",
                    $"—{Application.ProductName}—", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (selectedOption.Equals(DialogResult.Yes))
                {
                    Directory.Delete("photos", true);
                    if (!Directory.Exists("photos"))
                    {
                        statusBar.Text = "	" + "— Success —";
                    }
                }
            }
        }

        private void refreshFavoritesMenuItem_Click(object sender, EventArgs e)
        {
            Enabled = false;
            
            try
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
            catch
            {
                // ignored
            }

            Enabled = true;
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
        
        private void showAuthorPhotos_Click(object sender, EventArgs e) 
        {
            _author = _photos[_photoLabelId].User.Username;
            EnableControls(false);
            _photos.Clear();
            RefreshPhotoLabels();
            _currentPageCounter = 1;
            _isAuthorCollectionShown = true;
            GetAuthorsPhotos(_author, _currentPageCounter);
        }
        
        private void progress_ProgressChanged(object sender, float progress)
        {
            statusBar.Text = "	" + $"— {(int)progress}% —";
        }
        
        private void AppTips()
        {
            var toolTip = new ToolTip {AutoPopDelay = 4000, InitialDelay = 100, ReshowDelay = 100, ShowAlways = true};
            toolTip.SetToolTip(searchPhotosTextBox, "Type #random to show random photos");
            toolTip.SetToolTip(previousPageLabel, "Go to previous page");
            toolTip.SetToolTip(nextPageLabel, "Go to next page");
            toolTip.SetToolTip(showFavoritesButton, "Show favorites");
            toolTip.SetToolTip(aboutAppButton, $"About {Application.ProductName}");
        }
        
        public MainForm(List<Photo> photos, List<Image> images)
        {
            CenterToScreen();
            InitializeComponent();
            SendMessage(searchPhotosTextBox.Handle, EM_SETCUEBANNER, 1, "Search photos (type words and hit Enter)");
            CreateContextMenu();
            _photos = photos;
            _images = images;
            DisplayPhotos();
            CountFavorites();
            AppTips();
            Select();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)]string lParam);
        private const int EM_SETCUEBANNER = 0x1501; 
    }
}