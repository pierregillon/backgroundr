using System.Diagnostics;
using System.Runtime.InteropServices;
using backgroundr.domain;
using FlickrNet;

namespace backgroundr.infrastructure
{
    public class FlickrAuthenticationService
    {
        private readonly Flickr _flickr;
        private OAuthRequestToken _requestToken;
        private readonly IEncryptor _encryptor;

        public FlickrAuthenticationService(FlickrApiCredentials flickrApiCredentials, IEncryptor encryptor)
        {
            _flickr = new Flickr(flickrApiCredentials.ApiToken, flickrApiCredentials.ApiSecret);
            _encryptor = encryptor;
        }

        public void AuthenticateUserInBrowser()
        {
            _requestToken = _flickr.OAuthGetRequestToken("oob");
            var url = _flickr.OAuthCalculateAuthorizationUrl(_requestToken.Token, AuthLevel.Read);
            OpenBrowser(url);
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

        public static void OpenBrowser(string url)
        {
            try {
                Process.Start(url);
            }
            catch {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                    Process.Start("open", url);
                }
                else {
                    throw;
                }
            }
        }
    }
}