using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Pixel.Encryption;
using Pixel.Utils;
using Unsplasharp;

namespace Pixel.Forms
{
    public partial class InitialForm : Form
    {
        private static readonly UnsplasharpClient _client = new UnsplasharpClient(AES.Decrypt(
            "/JKkbIdAWvZXGGQk5hcjtonTgGYPeRHOBrR0ZreaCroiLWfgcPF0qv/Gl/gmlBkD/B8eSPVjkD4YjZe8tGiRWTuHWe8am/v8l4mGtF1zAAI=",
            $"{Application.CompanyName}\\{Application.ProductName}"));

        private static async Task GetPhotos()
        {
            try
            {
                var photos =  await _client.ListPhotos(1, 4);
                var images = new List<Image>();
                await Task.Run(() =>
                {
                    for (var i = 0; i < 4; i++)
                    {
                        images.Add(Task.Run(() => HttpUtil.StreamUrlToImage(photos[i].Urls.Small)).Result);
                    }
                    
                    var appMainThread = new Thread(() => new MainForm(photos, images).ShowDialog());
                    appMainThread.SetApartmentState(ApartmentState.STA);
                    appMainThread.Start();
                    Application.Exit();
                });
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message,
                    $"—{Application.ProductName}—", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }
        
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x00080000;
                return cp;
            }
        }
        
        private void UpdateFormDisplay(Image backgroundImage)
        {
            var screenDc = API.GetDC(IntPtr.Zero);
            var memDc = API.CreateCompatibleDC(screenDc);
            var hBitmap = IntPtr.Zero;
            var oldBitmap = IntPtr.Zero;

            try
            {
                var bmp = new Bitmap(backgroundImage);
                hBitmap = bmp.GetHbitmap(Color.FromArgb(0));
                oldBitmap = API.SelectObject(memDc, hBitmap);
                
                var size = bmp.Size;
                var pointSource = new Point(0, 0);
                var topPos = new Point(this.Left, this.Top);
                
                var blend = new API.BLENDFUNCTION
                {
                    BlendOp = API.AC_SRC_OVER,
                    BlendFlags = 0,
                    SourceConstantAlpha = 255,
                    AlphaFormat = API.AC_SRC_ALPHA
                };

                API.UpdateLayeredWindow(this.Handle, screenDc, ref topPos, ref size, memDc, ref pointSource, 0, ref blend, API.ULW_ALPHA);
                
                bmp.Dispose();
                API.ReleaseDC(IntPtr.Zero, screenDc);
                if (hBitmap != IntPtr.Zero)
                {
                    API.SelectObject(memDc, oldBitmap);
                    API.DeleteObject(hBitmap);
                }
                API.DeleteDC(memDc);
            }
            catch
            {
                // ignored
            }
        }
        
        private void AppLoad(object sender, EventArgs e)
        {
            UpdateFormDisplay(BackgroundImage);
        }
        
        public InitialForm()
        {
            InitializeComponent();
            HttpUtil.InitHttpClient();
            Task.Run(GetPhotos);
        }
    }
    
    internal class API
    {
        public const byte AC_SRC_OVER = 0x00;
        public const byte AC_SRC_ALPHA = 0x01;
        public const Int32 ULW_ALPHA = 0x00000002;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize,
            IntPtr hdcSrc, ref Point pprSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);
        
        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
        
        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool DeleteDC(IntPtr hdc);
        
        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool DeleteObject(IntPtr hObject);
    }
}