﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using backgroundr.cqrs;
using backgroundr.infrastructure;
using FlickrNet;

namespace backgroundr.application
{
    public class RandomlyChangeOsBackgroundImageHander : ICommandHandler<RandomlyChangeOsBackgroundImage>
    {
        private readonly ImageBackgroundManager _backgroundManager;
        private readonly BackgroundrParameters _parameters;

        public RandomlyChangeOsBackgroundImageHander(
            ImageBackgroundManager backgroundManager,
            BackgroundrParameters parameters)
        {
            _backgroundManager = backgroundManager;
            _parameters = parameters;
        }

        public async Task Handle(RandomlyChangeOsBackgroundImage command)
        {
            var photoUrl = await FindImage();
            if (string.IsNullOrEmpty(photoUrl) == false) {
                var localUrl = await DownloadImage(photoUrl);
                _backgroundManager.ChangeBackground(localUrl, PicturePosition.Center);
            }
        }

        private async Task<string> FindImage()
        {
            return await Task.Run(() => {
                var flickr = new Flickr(_parameters.Token, _parameters.TokenSecret) {
                    OAuthAccessToken = _parameters.OAuthAccessToken,
                    OAuthAccessTokenSecret = _parameters.OAuthAccessTokenSecret
                };
                flickr.AuthOAuthCheckToken();

                var photoCollection = flickr.PhotosSearch(new PhotoSearchOptions {
                    Tags = _parameters.Tags,
                    UserId = _parameters.UserId,
                    PerPage = 500,
                    ContentType = ContentTypeSearch.PhotosOnly,
                    MediaType = MediaType.Photos,
                    SortOrder = PhotoSearchSortOrder.Relevance
                });

                if (photoCollection.Any() == false) {
                    return null;
                }

                var random = new Random((int) DateTime.Now.Ticks);
                var imageIndex = random.Next(photoCollection.Count);
                var photo = photoCollection.ElementAt(imageIndex);
                return photo.Large2048Url ?? photo.LargeUrl;
            });
        }
        private static async Task<string> DownloadImage(string photoUrl)
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(photoUrl);
            var httpWebReponse = (HttpWebResponse) httpWebRequest.GetResponse();
            var stream = httpWebReponse.GetResponseStream();
            var tempFilePath = Path.GetTempFileName();
            using (var fileStream = File.Create(tempFilePath)) {
                await stream.CopyToAsync(fileStream);
            }
            return tempFilePath;
        }
    }
}