using System;

namespace backgroundr.application
{
    public class BackgroundrParameters
    {
        public string UserId { get; set; }
        public string Tags { get; set; }
        public string Token { get; set; }
        public string TokenSecret { get; set; }
        public string OAuthAccessToken { get; set; }
        public string OAuthAccessTokenSecret { get; set; }
        public DateTime? BackgroundImageLastRefreshDate { get; set; }
    }
}