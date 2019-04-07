using System;
using System.Text;

namespace backgroundr.infrastructure
{
    public class FlickrApiCredentials
    {
        public string ApiToken { get; }
        public string ApiSecret { get; }

        public FlickrApiCredentials(string apiToken, string apiSecret)
        {
            ApiToken = Encoding.UTF8.GetString(Convert.FromBase64String(apiToken));
            ApiSecret = Encoding.UTF8.GetString(Convert.FromBase64String(apiSecret));
        }
    }
}