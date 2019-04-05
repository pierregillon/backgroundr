namespace backgroundr.infrastructure
{
    public class FlickrApiCredentials
    {
        public string ApiToken { get; }
        public string ApiSecret { get; }

        public FlickrApiCredentials(string apiToken, string apiSecret)
        {
            ApiToken = apiToken;
            ApiSecret = apiSecret;
        }
    }
}