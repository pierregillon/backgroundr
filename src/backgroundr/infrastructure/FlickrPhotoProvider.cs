using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backgroundr.domain;
using FlickrNet;

namespace backgroundr.infrastructure
{
    public class FlickrPhotoProvider : IPhotoProvider
    {
        private readonly FlickrParameters _flickrParameters;
        private readonly IEncryptor _encryptor;
        private readonly FlickrApiCredentials _apiCredentials;

        public FlickrPhotoProvider(FlickrApiCredentials apiCredentials, FlickrParameters flickrParameters, IEncryptor encryptor)
        {
            _apiCredentials = apiCredentials;
            _flickrParameters = flickrParameters;
            _encryptor = encryptor;
        }

        public async Task<IReadOnlyCollection<string>> GetPhotos()
        {
            return await Task.Run(() => {
                var flickr = new Flickr(_apiCredentials.ApiToken, _apiCredentials.ApiSecret);
                if (_flickrParameters.PrivateAccess != null) {
                    flickr.OAuthAccessToken = _flickrParameters.PrivateAccess.OAuthAccessToken;
                    flickr.OAuthAccessTokenSecret = _encryptor.Decrypt(_flickrParameters.PrivateAccess.OAuthAccessTokenSecret);
                    flickr.AuthOAuthCheckToken();
                }

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