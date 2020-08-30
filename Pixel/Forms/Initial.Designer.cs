using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Pixel.Util;

namespace Pixel.Forms
{
    partial class Initial
    {
        private readonly ComponentResourceManager _resources =
            new ComponentResourceManager(typeof(Main));
        
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.BackgroundImage = ImageUtil.ResizeImageAndKeepRatio((Image) _resources.GetObject("$this.appLogo"), 200, 200);
            this.ClientSize = new System.Drawing.Size(200, 200);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Icon = ((Icon) (_resources.GetObject("$this.appIcon")));
            this.MaximizeBox = false;
            this.MaximizeBox = false;
            this.Text = Application.ProductName;
            this.Load += new System.EventHandler(this.AppLoad);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.ResumeLayout();
        }

        #endregion
    }
}