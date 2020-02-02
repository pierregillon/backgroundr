using System;
using System.IO;
using backgroundr.domain;
using backgroundr.infrastructure;

namespace backgroundr.daemon
{
    internal class FakeDesktopBackgroundImageUpdater : IDesktopBackgroundImageUpdater
    {
        public void ChangeBackgroundImage(string backgroundPath, PicturePosition picturePosition)
        {
            Console.WriteLine($"* Changing background to {Path.GetFileName(backgroundPath)} position {picturePosition}");
        }
    }
}