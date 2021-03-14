using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Pixel.Utils;

namespace Pixel.Forms
{
    partial class MainForm
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
            new ComponentResourceManager(typeof(MainForm));

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.statusBar = new StatusBar();
            this.searchPhotosTextBox = new TextBox();
            this.previousPageLabel = new Label();
            this.nextPageLabel = new Label();
            this.showFavoritesButton = new Button();
            this.aboutAppButton = new Button();
            this.photoLabel1 = new Label();
            this.photoLabel2 = new Label();
            this.photoLabel3 = new Label();
            this.photoLabel4 = new Label();
            //
            // statusBar
            //
            this.statusBar.Size = new Size(634, 20);
            this.statusBar.SizingGrip = false;
            this.statusBar.Font = new Font("Segoe UI", 8F);
            //
            // searchPhotosTextBox
            //
            this.searchPhotosTextBox.Location = new Point(479, 13);
            this.searchPhotosTextBox.Size = new Size(400, 22);
            this.searchPhotosTextBox.Font = new Font("Segoe UI", 8F);
            this.searchPhotosTextBox.TextAlign = HorizontalAlignment.Center;
            this.searchPhotosTextBox.Multiline = false;
            this.searchPhotosTextBox.WordWrap = false;
            this.searchPhotosTextBox.KeyDown += new KeyEventHandler(searchPhotosTextBox_KeyDown);
            this.searchPhotosTextBox.TabIndex = 2;
            //
            // previousPageLabel
            //
            this.previousPageLabel.Location = new Point(20, 44);
            this.previousPageLabel.Size = new Size(24, 200);
            this.previousPageLabel.Image = (Image) _resources.GetObject("$this.leftArrow");
            this.previousPageLabel.ImageAlign = ContentAlignment.MiddleCenter;
            this.previousPageLabel.UseCompatibleTextRendering = true;
            this.previousPageLabel.TabIndex = 0;
            this.previousPageLabel.Click += new EventHandler(this.previousPageLabel_Click);
            //
            // nextPageLabel
            //
            this.nextPageLabel.Location = new Point(1314, 44);
            this.nextPageLabel.Size = new Size(24, 200);
            this.nextPageLabel.Image = (Image) _resources.GetObject("$this.rightArrow");
            this.nextPageLabel.ImageAlign = ContentAlignment.MiddleCenter;
            this.nextPageLabel.UseCompatibleTextRendering = true;
            this.nextPageLabel.TabIndex = 1;
            this.nextPageLabel.Click += new EventHandler(this.nextPageLabel_Click);
            //
            // showFavoritesButton
            //
            this.showFavoritesButton.Location = new Point(1204, 12);
            this.showFavoritesButton.Size = new Size(120, 24);
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
            this.aboutAppButton.Location = new Point(1322, 12);
            this.aboutAppButton.Size = new Size(24, 24);
            this.aboutAppButton.Text = "i";
            this.aboutAppButton.Font = new Font("Verdana", 9F, FontStyle.Bold);
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
            this.photoLabel1.Location = new Point(64, 44);
            this.photoLabel1.Size = new Size(300, 200);
            this.photoLabel1.Click += new EventHandler(photoLabel1_Click);
            this.photoLabel1.Paint += new PaintEventHandler(this.photoLabel1_Paint);
            // 
            // photoLabel2
            //
            this.photoLabel2.Name = "photoLabel2";
            this.photoLabel2.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.photoLabel2.TextAlign = ContentAlignment.MiddleCenter;
            this.photoLabel2.Location = new Point(374, 44);
            this.photoLabel2.Size = new Size(300, 200);
            this.photoLabel2.Click += new EventHandler(photoLabel2_Click);
            this.photoLabel2.Paint += new PaintEventHandler(this.photoLabel2_Paint);
            // 
            // photoLabel3
            //
            this.photoLabel3.Name = "photoLabel3";
            this.photoLabel3.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.photoLabel3.TextAlign = ContentAlignment.MiddleCenter;
            this.photoLabel3.Location = new Point(684, 44);
            this.photoLabel3.Size = new Size(300, 200);
            this.photoLabel3.Click += new EventHandler(photoLabel3_Click);
            this.photoLabel3.Paint += new PaintEventHandler(this.photoLabel3_Paint);
            // 
            // photoLabel4
            //
            this.photoLabel4.Name = "photoLabel4";
            this.photoLabel4.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.photoLabel4.TextAlign = ContentAlignment.MiddleCenter;
            this.photoLabel4.Location = new Point(994, 44);
            this.photoLabel4.Size = new Size(300, 200);
            this.photoLabel4.Click += new EventHandler(photoLabel4_Click);
            this.photoLabel4.Paint += new PaintEventHandler(this.photoLabel4_Paint);
            // 
            // MainForm
            // 
            this.components = new Container();
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.ClientSize = new Size(1358, 274);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.searchPhotosTextBox);
            this.Controls.Add(this.previousPageLabel);
            this.Controls.Add(this.nextPageLabel);
            this.Controls.Add(this.showFavoritesButton);
            this.Controls.Add(this.aboutAppButton);
            this.Controls.Add(this.photoLabel1);
            this.Controls.Add(this.photoLabel2);
            this.Controls.Add(this.photoLabel3);
            this.Controls.Add(this.photoLabel4);
            this.Icon = ((Icon) (_resources.GetObject("$this.appIcon")));
            this.MaximizeBox = false;
            this.Text = Application.ProductName;
            this.ResumeLayout(false);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            CenterToScreen();
        }

        #endregion

        private StatusBar statusBar;
        private TextBox searchPhotosTextBox;
        private Label previousPageLabel;
        private Label nextPageLabel;
        private Button showFavoritesButton;
        private Button aboutAppButton;
        private Label photoLabel1;
        private Label photoLabel2;
        private Label photoLabel3;
        private Label photoLabel4;
    }
}