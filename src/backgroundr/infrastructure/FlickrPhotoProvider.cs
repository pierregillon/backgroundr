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
        private readonly Parameters _parameters;
        private readonly IEncryptor _encryptor;

        public FlickrPhotoProvider(Parameters parameters, IEncryptor encryptor)
        {
            _parameters = parameters;
            _encryptor = encryptor;
        }

        public async Task<IReadOnlyCollection<string>> GetPhotos()
        {
            return await Task.Run(() => {
                var flickr = new Flickr(_parameters.ApiToken, _encryptor.Decrypt(_parameters.ApiSecret)) {
                    OAuthAccessToken = _parameters.OAuthAccessToken,
                    OAuthAccessTokenSecret = _encryptor.Decrypt(_parameters.OAuthAccessTokenSecret)
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