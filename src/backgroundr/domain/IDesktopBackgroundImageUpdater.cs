using backgroundr.infrastructure;

namespace backgroundr.domain
{
    public interface IDesktopBackgroundImageUpdater
    {
        void ChangeBackgroundImage(string backgroundPath, PicturePosition picturePosition);
    }
}