using System;
using backgroundr.domain;
using backgroundr.infrastructure;

namespace backgroundr.daemon
{
    internal class FakeDesktopBackgroundImageUpdater : IDesktopBackgroundImageUpdater
    {
        public void ChangeBackgroundImage(string backgroundPath, PicturePosition style)
        {
            Console.WriteLine($"* Changing background to {backgroundPath} position {style}");
        }
    }
}