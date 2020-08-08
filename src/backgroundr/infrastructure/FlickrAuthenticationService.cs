using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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

        public async Task AuthenticateUserInBrowser()
        {
            _requestToken = await _flickr.OAuthRequestTokenAsync("oob");
            var url = _flickr.OAuthCalculateAuthorizationUrl(_requestToken.Token, AuthLevel.Read);
            OpenBrowser(url);
        }

        public async Task<FlickrPrivateAccess> FinalizeAuthentication(string verifier)
        {
            var accessToken = await _flickr.OAuthAccessTokenAsync(_requestToken.Token, _requestToken.TokenSecret, verifier);
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