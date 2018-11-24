using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using FlickrNet;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace backgroundr.view
{
    public class NotifyIconViewModel
    {
        private const string TOKEN_FILE_PATH = ".flickr";

        private readonly IFileService _fileService;

        public ICommand RandomlyChangeBackgroundImageCommand
        {
            get
            {
                return new DelegateCommand {
                    CanExecuteFunc = () => true,
                    CommandAction = RandomlyChangeBackgroundImage
                };
            }
        }
        public ICommand OpenParametersWindowCommand
        {
            get
            {
                return new DelegateCommand {
                    CanExecuteFunc = () => Application.Current.MainWindow == null,
                    CommandAction = () => {
                        Application.Current.MainWindow = new MainWindow();
                        Application.Current.MainWindow.Show();
                    }
                };
            }
        }
        public ICommand ExitApplicationCommand
        {
            get { return new DelegateCommand { CommandAction = () => Application.Current.Shutdown() }; }
        }

        public NotifyIconViewModel()
        {
            _fileService = new FileService();
        }

        public void RandomlyChangeBackgroundImage()
        {
            var accesToken = _fileService.Deserialize<OAuthAccessToken>(TOKEN_FILE_PATH);
            var flickr = new Flickr("a023233ad75a2e7ae38a1b1aa92ff751", "abd048b37b9e44f9") {
                OAuthAccessToken = accesToken.Token,
                OAuthAccessTokenSecret = accesToken.TokenSecret
            };
            flickr.AuthOAuthCheckToken();

            var photoCollection = flickr.PhotosSearch(new PhotoSearchOptions {
                Tags = "best",
                UserId = "148722902@N07",
                PerPage = 500,
                Extras = PhotoSearchExtras.Large2048Url,
                ContentType = ContentTypeSearch.PhotosOnly,
                MediaType = MediaType.Photos
            }).Where(x=>x.OriginalWidth > x.OriginalHeight).ToArray();

            if (photoCollection.Any() == false) {
                return;
            }

            var random = new Random((int)DateTime.Now.Ticks);
            var imageIndex = random.Next(photoCollection.Length);
            var photo = photoCollection.ElementAt(imageIndex);
            var localUrl = DownloadImage(photo.Large2048Url);
            
            var imageBackgroundManager = new ImageBackgroundManager();
            imageBackgroundManager.ChangeBackground(localUrl, PicturePosition.Center);
        }
        private static string DownloadImage(string photoUrl)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(photoUrl);
            var httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse();
            var stream = httpWebReponse.GetResponseStream();
            var tempFilePath = Path.GetTempFileName();
            using (var fileStream = File.Create(tempFilePath))
            {
                stream.CopyTo(fileStream);
            }
            return tempFilePath;
        }
    }


    /// <summary>
    /// Simplistic delegate command for the demo.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        public Action CommandAction { get; set; }
        public Func<bool> CanExecuteFunc { get; set; }

        public void Execute(object parameter)
        {
            CommandAction();
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    public interface IFileService
    {
        T Deserialize<T>(string filePath);
        void Serialize<T>(T obj, string filePath);
        bool Exists(string filePath);
    }

    public class FileService : IFileService
    {
        public T Deserialize<T>(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public void Serialize<T>(T obj, string filePath)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
        public bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }
    }

    public enum PicturePosition
    {
        Tile, Center, Stretch, Fit, Fill
    }

    internal sealed class NativeMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int SystemParametersInfo(
            int uAction,
            int uParam,
            String lpvParam,
            int fuWinIni);
    }

    public class ImageBackgroundManager 
    {
        public void ChangeBackground(string backgroundPath, PicturePosition style) 
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            switch (style)
            {
                case PicturePosition.Tile:
                    key.SetValue(@"PicturePosition", "0");
                    key.SetValue(@"TileWallpaper", "1");
                    break;
                case PicturePosition.Center:
                    key.SetValue(@"PicturePosition", "0");
                    key.SetValue(@"TileWallpaper", "0");
                    break;
                case PicturePosition.Stretch:
                    key.SetValue(@"PicturePosition", "2");
                    key.SetValue(@"TileWallpaper", "0");
                    break;
                case PicturePosition.Fit:
                    key.SetValue(@"PicturePosition", "6");
                    key.SetValue(@"TileWallpaper", "0");
                    break;
                case PicturePosition.Fill:
                    key.SetValue(@"PicturePosition", "10");
                    key.SetValue(@"TileWallpaper", "0");
                    break;
            }
            key.Close();

            const int SET_DESKTOP_BACKGROUND = 20;
            const int UPDATE_INI_FILE = 1;
            const int SEND_WINDOWS_INI_CHANGE = 2;
            NativeMethods.SystemParametersInfo(SET_DESKTOP_BACKGROUND, 0, backgroundPath, UPDATE_INI_FILE | SEND_WINDOWS_INI_CHANGE);
        }
    }
}