using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Pixel.Util;

namespace Pixel.Forms
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        
        public ComponentResourceManager _resources =
            new ComponentResourceManager(typeof(Main));

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.statusBar = new StatusBar();
            this.searchPhotosTextBox = new TextBox();
            this.previousPageButton = new Button();
            this.nextPageButton = new Button();
            this.addRemoveFavoriteButton = new Button();
            this.showFavoritesButton = new Button();
            this.aboutAppButton = new Button();
            this.photoLabel1 = new Label();
            this.photoLabel2 = new Label();
            this.photoLabel3 = new Label();
            this.photoLabel4 = new Label();
            this.photoLabel5 = new Label();
            this.photoLabel6 = new Label();
            //
            // statusBar
            //
            this.statusBar.Size = new Size(634, 20);
            this.statusBar.SizingGrip = false;
            this.statusBar.Font = new Font("Segoe UI", 8F);
            //
            // searchPhotosTextBox
            //
            this.searchPhotosTextBox.Location = new Point(150, 13);
            this.searchPhotosTextBox.Size = new Size(312, 22);
            this.searchPhotosTextBox.Font = new Font("Segoe UI", 8F);
            this.searchPhotosTextBox.TextAlign = HorizontalAlignment.Center;
            this.searchPhotosTextBox.Multiline = false;
            this.searchPhotosTextBox.WordWrap = false;
            this.searchPhotosTextBox.KeyDown += new KeyEventHandler(searchPhotosTextBox_KeyDown);
            this.searchPhotosTextBox.TabIndex = 2;
            //
            // previousPageButton
            //
            this.previousPageButton.Location = new Point(12, 12);
            this.previousPageButton.Size = new Size(70, 24);
            this.previousPageButton.Image = (Image) _resources.GetObject("$this.leftArrow");
            this.previousPageButton.ImageAlign = ContentAlignment.MiddleCenter;
            this.previousPageButton.UseCompatibleTextRendering = true;
            this.previousPageButton.UseVisualStyleBackColor = true;
            this.previousPageButton.TabIndex = 0;
            this.previousPageButton.Click += new EventHandler(this.previousPageButton_Click);
            //
            // nextPageButton
            //
            this.nextPageButton.Location = new Point(80, 12);
            this.nextPageButton.Size = new Size(70, 24);
            this.nextPageButton.Image = (Image) _resources.GetObject("$this.rightArrow");
            this.nextPageButton.ImageAlign = ContentAlignment.MiddleCenter;
            this.nextPageButton.UseCompatibleTextRendering = true;
            this.nextPageButton.UseVisualStyleBackColor = true;
            this.nextPageButton.TabIndex = 1;
            this.nextPageButton.Click += new EventHandler(this.nextPageButton_Click);
            //
            // addRemoveFavoriteButton
            //
            this.addRemoveFavoriteButton.Location = new Point(462, 12);
            this.addRemoveFavoriteButton.Size = new Size(30, 24);
            this.addRemoveFavoriteButton.Image = ImageUtil.ResizeImageAndKeepRatio((Image) _resources.GetObject("$this.add"), 12, 12);
            this.addRemoveFavoriteButton.ImageAlign = ContentAlignment.MiddleCenter;
            this.addRemoveFavoriteButton.UseCompatibleTextRendering = true;
            this.addRemoveFavoriteButton.UseVisualStyleBackColor = true;
            this.addRemoveFavoriteButton.TabIndex = 3;
            this.addRemoveFavoriteButton.Click += new EventHandler(this.addRemoveFavoriteButton_Click);
            //
            // showFavoritesButton
            //
            this.showFavoritesButton.Location = new Point(490, 12);
            this.showFavoritesButton.Size = new Size(102, 24);
            this.showFavoritesButton.Text = "Favorites";
            this.showFavoritesButton.Font = new Font("Segoe UI", 8F);
            this.showFavoritesButton.TextAlign = ContentAlignment.TopCenter;
            this.showFavoritesButton.UseVisualStyleBackColor = true;
            this.showFavoritesButton.UseCompatibleTextRendering = true;
            this.showFavoritesButton.TabIndex = 4;
            this.showFavoritesButton.Click += new EventHandler(this.showFavoritesButton_Click);
            //
            // aboutAppButton
            //
            this.aboutAppButton.Location = new Point(590, 12);
            this.aboutAppButton.Size = new Size(32, 24);
            this.aboutAppButton.Text = "A";
            this.aboutAppButton.Font = new Font("Segoe UI", 9F);
            this.aboutAppButton.UseCompatibleTextRendering = true;
            this.aboutAppButton.UseVisualStyleBackColor = true;
            this.aboutAppButton.TextAlign = ContentAlignment.TopCenter;
            this.aboutAppButton.TabIndex = 5;
            this.aboutAppButton.Click += new EventHandler(this.aboutAppButton_Click);
            // 
            // photoLabel1
            //
            this.photoLabel1.Name = "photoLabel1";
            this.photoLabel1.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.photoLabel1.TextAlign = ContentAlignment.MiddleCenter;
            this.photoLabel1.Location = new Point(12, 44);
            this.photoLabel1.Size = new Size(300, 200);
            this.photoLabel1.Click += new EventHandler(photoLabel1_Click);
            this.photoLabel1.Paint += new PaintEventHandler(this.photoLabel1_Paint);
            // 
            // photoLabel2
            //
            this.photoLabel2.Name = "photoLabel2";
            this.photoLabel2.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.photoLabel2.TextAlign = ContentAlignment.MiddleCenter;
            this.photoLabel2.Location = new Point(322, 44);
            this.photoLabel2.Size = new Size(300, 200);
            this.photoLabel2.Click += new EventHandler(photoLabel2_Click);
            this.photoLabel2.Paint += new PaintEventHandler(this.photoLabel2_Paint);
            // 
            // photoLabel3
            //
            this.photoLabel3.Name = "photoLabel3";
            this.photoLabel3.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.photoLabel3.TextAlign = ContentAlignment.MiddleCenter;
            this.photoLabel3.Location = new Point(12, 256);
            this.photoLabel3.Size = new Size(300, 200);
            this.photoLabel3.Click += new EventHandler(photoLabel3_Click);
            this.photoLabel3.Paint += new PaintEventHandler(this.photoLabel3_Paint);
            // 
            // photoLabel4
            //
            this.photoLabel4.Name = "photoLabel4";
            this.photoLabel4.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.photoLabel4.TextAlign = ContentAlignment.MiddleCenter;
            this.photoLabel4.Location = new Point(322, 256);
            this.photoLabel4.Size = new Size(300, 200);
            this.photoLabel4.Click += new EventHandler(photoLabel4_Click);
            this.photoLabel4.Paint += new PaintEventHandler(this.photoLabel4_Paint);
            // 
            // photoLabel5
            //
            this.photoLabel5.Name = "photoLabel5";
            this.photoLabel5.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.photoLabel5.TextAlign = ContentAlignment.MiddleCenter;
            this.photoLabel5.Location = new Point(12, 468);
            this.photoLabel5.Size = new Size(300, 200);
            this.photoLabel5.Click += new EventHandler(photoLabel5_Click);
            this.photoLabel5.Paint += new PaintEventHandler(this.photoLabel5_Paint);
            // 
            // photoLabel6
            //
            this.photoLabel6.Name = "photoLabel6";
            this.photoLabel6.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.photoLabel6.TextAlign = ContentAlignment.MiddleCenter;
            this.photoLabel6.Location = new Point(322, 468);
            this.photoLabel6.Size = new Size(300, 200);
            this.photoLabel6.Click += new EventHandler(photoLabel6_Click);
            this.photoLabel6.Paint += new PaintEventHandler(this.photoLabel6_Paint);
            // 
            // Main
            // 
            this.components = new Container();
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.ClientSize = new Size(634, 700);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.searchPhotosTextBox);
            this.Controls.Add(this.previousPageButton);
            this.Controls.Add(this.nextPageButton);
            this.Controls.Add(this.addRemoveFavoriteButton);
            this.Controls.Add(this.showFavoritesButton);
            this.Controls.Add(this.aboutAppButton);
            this.Controls.Add(this.photoLabel1);
            this.Controls.Add(this.photoLabel2);
            this.Controls.Add(this.photoLabel3);
            this.Controls.Add(this.photoLabel4);
            this.Controls.Add(this.photoLabel5);
            this.Controls.Add(this.photoLabel6);
            this.Icon = ((Icon) (_resources.GetObject("$this.appIcon")));
            this.MaximizeBox = false;
            this.Text = Application.ProductName;
            this.ResumeLayout(false);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            //this.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2);
            CenterToScreen();
        }

        #endregion

        private StatusBar statusBar;
        private TextBox searchPhotosTextBox;
        private Button previousPageButton;
        private Button nextPageButton;
        private Button addRemoveFavoriteButton;
        private Button showFavoritesButton;
        private Button aboutAppButton;
        private Label photoLabel1;
        private Label photoLabel2;
        private Label photoLabel3;
        private Label photoLabel4;
        private Label photoLabel5;
        private Label photoLabel6;
    }
}