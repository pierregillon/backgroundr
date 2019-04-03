using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backgroundr.application;
using backgroundr.domain;
using FlickrNet;

namespace backgroundr.infrastructure
{
    public class FlickrPhotoProvider : IPhotoProvider
    {
        private const string ApiToken = "a023233ad75a2e7ae38a1b1aa92ff751";
        private const string ApiSecret = "abd048b37b9e44f9";

        private readonly FlickrParameters _flickrParameters;
        private readonly IEncryptor _encryptor;

        public FlickrPhotoProvider(FlickrParameters flickrParameters, IEncryptor encryptor)
        {
            _flickrParameters = flickrParameters;
            _encryptor = encryptor;
        }

        public async Task<IReadOnlyCollection<string>> GetPhotos()
        {
            return await Task.Run(() => {
                var flickr = new Flickr(ApiToken, ApiSecret) {
                    OAuthAccessToken = _flickrParameters.OAuthAccessToken,
                    OAuthAccessTokenSecret = _encryptor.Decrypt(_flickrParameters.OAuthAccessTokenSecret)
                };
                flickr.AuthOAuthCheckToken();

                var photoCollection = flickr.PhotosSearch(new PhotoSearchOptions {
                    Tags = _flickrParameters.Tags,
                    UserId = _flickrParameters.UserId,
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