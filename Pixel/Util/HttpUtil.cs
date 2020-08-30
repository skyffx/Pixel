using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Pixel.Extensions;

namespace Pixel.Util
{
    public static class HttpUtil
    {
        private static readonly HttpClient _client = new HttpClient();
        private static HttpResponseMessage _response;
        private static Image _image;

        public static void InitHttpClient()
        {
            _client.DefaultRequestHeaders.UserAgent.ParseAdd($"{Application.ProductName}/{Application.ProductVersion.Remove(3)}");
        }

        public static async Task<HttpResponseMessage> Get(string uri)
        {
            try
            {
                _response = await _client.GetAsync(uri);
                _response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException httpRequestException)
            {
                MessageBox.Show(httpRequestException.Message,
                    $"—{Application.ProductName}—", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
            
            return _response;
        }
        
        public static async Task<Image> StreamUrlToImage(string imageUrl)
        {
            try
            {
                var imageBytes = await _client.GetByteArrayAsync(imageUrl);
                _image = Image.FromStream(new MemoryStream(imageBytes));
            }
            catch (HttpRequestException httpRequestException)
            {
                MessageBox.Show(httpRequestException.Message,
                    $"—{Application.ProductName}—", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
            
            return _image;
        }
        
        public static async Task<Image> StreamUrlToImageAndResize(string imageUrl, int imageWidth, int imageHeight)
        {
            try
            {
                var imageBytes = await _client.GetByteArrayAsync(imageUrl);
                _image = Image.FromStream(new MemoryStream(ImageUtil.ResizeImageFromStream(imageBytes, imageWidth, imageHeight)));
            }
            catch (HttpRequestException httpRequestException)
            {
                MessageBox.Show(httpRequestException.Message,
                    $"—{Application.ProductName}—", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
            
            return _image;
        }

        public static async void DownloadFile(string url, string destination, Progress<float> progress, bool openAfterDownload)
        {
            try
            {
                /*https://gist.github.com/dalexsoto/9fd3c5bdbe9f61a717d47c5843384d11*/
                using(var file = new FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await _client.DownloadDataAsync(url, file, progress);
                }

                if (openAfterDownload)
                {
                    var process = new Process {StartInfo = {FileName = destination}};
                    process.Start();
                }
            }
            catch (HttpRequestException httpRequestException)
            {
                MessageBox.Show(httpRequestException.Message,
                    $"—{Application.ProductName}—", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }
    }
}