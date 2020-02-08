using System;
using System.Runtime.InteropServices;
using backgroundr.domain;
using backgroundr.infrastructure;
using Microsoft.Win32;

namespace backgroundr.daemon.windows
{
    public class DirectRegistryDesktopBackgroundImageUpdater : IDesktopBackgroundImageUpdater
    {
        public const int SetDesktopWallpaper = 20;
        public const int UpdateIniFile = 0x01;
        public const int SendWinIniChange = 0x02;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public void ChangeBackgroundImage(string backgroundPath, PicturePosition picturePosition)
        {
            SystemParametersInfo(SetDesktopWallpaper, 0, backgroundPath, UpdateIniFile | SendWinIniChange);

            RegistryKey key = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true);
            switch (picturePosition) {
                case PicturePosition.Tile:
                    key.SetValue(@"WallpaperStyle", "0");
                    key.SetValue(@"TileWallpaper", "1");
                    break;
                case PicturePosition.Center:
                    key.SetValue(@"WallpaperStyle", "0");
                    key.SetValue(@"TileWallpaper", "0");
                    break;
                case PicturePosition.Extend:
                case PicturePosition.Stretch:
                    key.SetValue(@"WallpaperStyle", "2");
                    key.SetValue(@"TileWallpaper", "0");
                    break;
                case PicturePosition.Fill:
                    key.SetValue(@"WallpaperStyle", "10");
                    key.SetValue(@"TileWallpaper", "0");
                    break;
                case PicturePosition.Fit:
                    key.SetValue(@"WallpaperStyle", "6");
                    key.SetValue(@"TileWallpaper", "0");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(picturePosition), picturePosition, null);
            }

            key.Close();
        }
    }
}