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
                    SortOrder = PhotoSearchSortOrder.Relevance,
                    Extras = PhotoSearchExtras.Large2048Url | PhotoSearchExtras.Large1600Url | PhotoSearchExtras.LargeUrl
                });

                return photoCollection
                    .Where(x => x.LargeWidth > x.LargeHeight)
                    .Select(x => x.Large2048Url ?? x.Large1600Url ?? x.LargeUrl)
                    .Where(x => string.IsNullOrEmpty(x) == false)
                    .ToArray();
            });
        }
    }
}