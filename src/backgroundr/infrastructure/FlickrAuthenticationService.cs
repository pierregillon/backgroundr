using System.Diagnostics;
using backgroundr.application;
using backgroundr.domain;
using FlickrNet;

namespace backgroundr.infrastructure
{
    public class FlickrAuthenticationService
    {
        private const string ApiToken = "a023233ad75a2e7ae38a1b1aa92ff751";
        private const string ApiSecret = "abd048b37b9e44f9";

        private readonly Flickr _flickr;
        private OAuthRequestToken _requestToken;
        private readonly IEncryptor _encryptor;

        public FlickrAuthenticationService(IEncryptor encryptor)
        {
            _encryptor = encryptor;
            _flickr = new Flickr(ApiToken, ApiSecret);
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