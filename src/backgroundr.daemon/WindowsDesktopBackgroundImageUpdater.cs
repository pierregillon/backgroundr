using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using backgroundr.domain;
using backgroundr.infrastructure;

namespace backgroundr.daemon
{
    public class WindowsDesktopBackgroundImageUpdater : IDesktopBackgroundImageUpdater
    {
        public void ChangeBackgroundImage(string backgroundPath, PicturePosition picturePosition)
        {
            var script = GetScript(backgroundPath, picturePosition);

            var process = new Process {
                StartInfo = new ProcessStartInfo(@"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe", script) {
                    WorkingDirectory = Environment.CurrentDirectory,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            process.WaitForExit();
            
            if (process.ExitCode != 0) {
                throw new Exception("An error occurred when changing background image : " + process.StandardError.ReadLine());
            }
        }

        private static string GetScript(string backgroundPath, PicturePosition picturePosition)
        {
            var setBackgroundCommand = SetDesktopItemPropertyPowershellCommand("wallpaper", $"\"{backgroundPath}\"");
            var setPicturePositionCommand = GetPicturePositionCommands(picturePosition);
            var refreshCommand = "rundll32.exe user32.dll, UpdatePerUserSystemParameters";
            var commands = new[] { setBackgroundCommand }.Concat(setPicturePositionCommand).Union(new[] { refreshCommand });
            return string.Join(Environment.NewLine, commands);
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