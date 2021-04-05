using System.Threading.Tasks;
using backgroundr.domain;
using backgroundr.infrastructure;

namespace backgroundr.daemon.linux
{
    public class LinuxCommandLineDesktopBackgroundImageUpdater : IDesktopBackgroundImageUpdater
    {
        public async Task ChangeBackgroundImage(string backgroundPath, PicturePosition picturePosition)
        {
            var arguments = new[] {
                "gsettings",
                "set",
                "org.gnome.desktop.background",
                "picture-uri",
                backgroundPath
            };

            await string.Join(' ', arguments).Bash();
        }
    }
}