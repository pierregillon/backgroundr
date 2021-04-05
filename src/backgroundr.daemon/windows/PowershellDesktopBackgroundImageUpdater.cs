using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using backgroundr.domain;
using backgroundr.infrastructure;

namespace backgroundr.daemon.windows
{
    public class PowershellDesktopBackgroundImageUpdater : IDesktopBackgroundImageUpdater
    {
        public async Task ChangeBackgroundImage(string backgroundPath, PicturePosition picturePosition)
        {
            try {
                await RunPowershellScript(GetScript(backgroundPath, picturePosition));
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                throw;
            }
        }

        private static async Task RunPowershellScript(string script)
        {
            var arguments = script;
            var process = new Process {
                StartInfo = new ProcessStartInfo(@"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe", arguments) {
                    WorkingDirectory = Environment.CurrentDirectory,
                    CreateNoWindow = true,
                    RedirectStandardError = true
                }
            };
            process.Start();
            process.WaitForExit();

            if (process.ExitCode != 0) {
                throw new Exception("An error occurred when changing background image : " + await process.StandardError.ReadToEndAsync());
            }
        }

        private static string GetScript(string backgroundPath, PicturePosition picturePosition)
        {
            const string updateWallpaperCsharpFunc = @"
using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Wallpaper {
    public enum Style : int {
        Center, Stretch, Fill, Fit, Tile
    }
    public class Setter {
        public const int SetDesktopWallpaper = 20;
        public const int UpdateIniFile = 0x01;
        public const int SendWinIniChange = 0x02;

        [DllImport(""user32.dll"", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public static void SetWallpaper(string path, Wallpaper.Style style)
        {
            SystemParametersInfo(SetDesktopWallpaper, 0, path, UpdateIniFile | SendWinIniChange);
            RegistryKey key = Registry.CurrentUser.OpenSubKey(""Control Panel\\Desktop"", true);
            switch (style)
            {
                case Style.Tile:
                    key.SetValue(@""WallpaperStyle"", ""0"");
                    key.SetValue(@""TileWallpaper"", ""1"");
                    break;
                case Style.Center:
                    key.SetValue(@""WallpaperStyle"", ""0"");
                    key.SetValue(@""TileWallpaper"", ""0"");
                    break;
                case Style.Stretch:
                    key.SetValue(@""WallpaperStyle"", ""2"");
                    key.SetValue(@""TileWallpaper"", ""0"");
                    break;
                case Style.Fill:
                    key.SetValue(@""WallpaperStyle"", ""10"");
                    key.SetValue(@""TileWallpaper"", ""0"");
                    break;
                case Style.Fit:
                    key.SetValue(@""WallpaperStyle"", ""6"");
                    key.SetValue(@""TileWallpaper"", ""0"");
                    break;
            }
            key.Close();
        }
    }
}";

            var updateWallpaperPsFunction = @"
Function Update-Wallpaper {
    Param (
        [Parameter(Mandatory =$true)] $Path,
        [ValidateSet('Center', 'Stretch', 'Fill', 'Tile', 'Fit')] $Style
    )
    Add-Type -TypeDefinition {'[CODE]'} -ErrorAction Stop 
    [Wallpaper.Setter]::SetWallpaper( $Path, $Style )
}";

            var execCode = updateWallpaperPsFunction
                .Replace("[CODE]", updateWallpaperCsharpFunc)
                .Replace(Environment.NewLine, string.Empty);

            //var setBackgroundCommand = SetDesktopItemPropertyPowershellCommand("wallpaper", $"\"{backgroundPath}\"");
            //var setPicturePositionCommand = GetPicturePositionCommands(picturePosition);
            //var refreshCommand = "rundll32.exe user32.dll, UpdatePerUserSystemParameters";

            var call = $"Update-Wallpaper '{backgroundPath}' 'Stretch'";

            return string.Join(Environment.NewLine, new[] { execCode, call });
        }

        private static string SetDesktopItemPropertyPowershellCommand(string key, string value)
        {
            return $@"Set-ItemProperty -path 'HKCU:\Control Panel\Desktop\' -name {key} -value {value}";
        }

        private static IEnumerable<string> GetPicturePositionCommands(PicturePosition picturePosition)
        {
            static string SetPicturePositionCommand(string value) => SetDesktopItemPropertyPowershellCommand("PicturePosition", value);
            static string SetTileWallpaperCommand(string value) => SetDesktopItemPropertyPowershellCommand("TileWallpaper", value);

            switch (picturePosition) {
                case PicturePosition.Tile:
                    yield return SetPicturePositionCommand("0");
                    yield return SetTileWallpaperCommand("1");
                    break;
                case PicturePosition.Center:
                    yield return SetPicturePositionCommand("0");
                    yield return SetTileWallpaperCommand("0");
                    break;
                case PicturePosition.Stretch:
                    yield return SetPicturePositionCommand("2");
                    yield return SetTileWallpaperCommand("0");
                    break;
                case PicturePosition.Fit:
                    yield return SetPicturePositionCommand("6");
                    yield return SetTileWallpaperCommand("0");
                    break;
                case PicturePosition.Fill:
                    yield return SetPicturePositionCommand("10");
                    yield return SetTileWallpaperCommand("0");
                    break;
                case PicturePosition.Extend:
                    yield return SetPicturePositionCommand("6");
                    yield return SetTileWallpaperCommand("0");
                    break;
            }
        }
    }
}