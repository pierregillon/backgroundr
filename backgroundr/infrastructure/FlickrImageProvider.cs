using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backgroundr.application;
using backgroundr.domain;
using FlickrNet;

namespace backgroundr.infrastructure
{
    public class FlickrImageProvider : IImageProvider
    {
        private readonly BackgroundrParameters _parameters;

        public FlickrImageProvider(BackgroundrParameters parameters)
        {
            _parameters = parameters;
        }

        public async Task<IReadOnlyCollection<string>> GetImageUrls()
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

                return photoCollection
                    .Select(x => x.Large2048Url ?? x.LargeUrl)
                    .ToArray();
            });
        }
    }
}