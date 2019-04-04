using System.Diagnostics;
using backgroundr.application;
using backgroundr.domain;
using FlickrNet;

namespace backgroundr.infrastructure
{
    public class FlickrAuthenticationService
    {
        private readonly Flickr _flickr;
        private OAuthRequestToken _requestToken;
        private readonly IEncryptor _encryptor;

        public FlickrAuthenticationService(IEncryptor encryptor, FlickrApiCredentials flickrApiCredentials)
        {
            _encryptor = encryptor;
            _flickr = new Flickr(flickrApiCredentials.ApiToken, flickrApiCredentials.ApiSecret);
        }

        public void AuthenticateUserInBrowser()
        {
            _requestToken = _flickr.OAuthGetRequestToken("oob");
            var url = _flickr.OAuthCalculateAuthorizationUrl(_requestToken.Token, AuthLevel.Read);
            Process.Start(url);
        }

        public FlickrPrivateAccess FinalizeAuthentication(string verifier)
        {
            var accessToken = _flickr.OAuthGetAccessToken(_requestToken, verifier);
            return new FlickrPrivateAccess {
                UserId = accessToken.UserId,
                UserName = accessToken.FullName,
                OAuthAccessToken = accessToken.Token,
                OAuthAccessTokenSecret = _encryptor.Encrypt(accessToken.TokenSecret)
            };
        }
    }
}