using System.Threading.Tasks;
using backgroundr.infrastructure;

namespace backgroundr.domain
{
    public interface IDesktopBackgroundImageUpdater
    {
        Task ChangeBackgroundImage(string backgroundPath, PicturePosition picturePosition);
    }
}