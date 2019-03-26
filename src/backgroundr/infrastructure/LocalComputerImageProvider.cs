using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using backgroundr.domain;

namespace backgroundr.infrastructure
{
    public class LocalComputerImageProvider : IPhotoProvider
    {
        public Task<IReadOnlyCollection<string>> GetPhotos()
        {
            return Task.Run(() => {
                var myPicturesFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                return (IReadOnlyCollection<string>) Directory.GetFiles(myPicturesFolder, "*.jpg", SearchOption.AllDirectories);
            });
        }
    }
}