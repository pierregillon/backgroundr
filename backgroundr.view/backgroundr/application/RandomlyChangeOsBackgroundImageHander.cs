using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using backgroundr.cqrs;
using backgroundr.domain;
using backgroundr.infrastructure;
using FlickrNet;

namespace backgroundr.application
{
    public class RandomlyChangeOsBackgroundImageHander : ICommandHandler<RandomlyChangeOsBackgroundImage>
    {
        private const string TOKEN_FILE_PATH = ".flickr";

        private readonly IFileService _fileService;

        public RandomlyChangeOsBackgroundImageHander(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task Handle(RandomlyChangeOsBackgroundImage command)
        {
            var accesToken = _fileService.Deserialize<OAuthAccessToken>(TOKEN_FILE_PATH);
            var flickr = new Flickr("a023233ad75a2e7ae38a1b1aa92ff751", "abd048b37b9e44f9") {
                OAuthAccessToken = accesToken.Token,
                OAuthAccessTokenSecret = accesToken.TokenSecret
            };
            flickr.AuthOAuthCheckToken();

            await Task.Run(() => {
                var photoCollection = flickr.PhotosSearch(new PhotoSearchOptions {
                    Tags = "best",
                    UserId = "148722902@N07",
                    PerPage = 500,
                    Extras = PhotoSearchExtras.Large2048Url,
                    ContentType = ContentTypeSearch.PhotosOnly,
                    MediaType = MediaType.Photos
                });

                if (photoCollection.Any() == false) {
                    return;
                }

                var random = new Random((int) DateTime.Now.Ticks);
                var imageIndex = random.Next(photoCollection.Count);
                var photo = photoCollection.ElementAt(imageIndex);
                var localUrl = DownloadImage(photo.Large2048Url);

                var imageBackgroundManager = new ImageBackgroundManager();
                imageBackgroundManager.ChangeBackground(localUrl, PicturePosition.Center);
            });
        }

        private static string DownloadImage(string photoUrl)
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(photoUrl);
            var httpWebReponse = (HttpWebResponse) httpWebRequest.GetResponse();
            var stream = httpWebReponse.GetResponseStream();
            var tempFilePath = Path.GetTempFileName();
            using (var fileStream = File.Create(tempFilePath)) {
                stream.CopyTo(fileStream);
            }
            return tempFilePath;
        }
    }
}