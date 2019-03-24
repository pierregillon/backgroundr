using System;

namespace backgroundr.application
{
    public class BackgroundrParameters
    {
        public string UserId { get; set; }
        public string Tags { get; set; }
        public string ApiToken { get; set; }
        public string ApiSecret { get; set; }
        public string OAuthAccessToken { get; set; }
        public string OAuthAccessTokenSecret { get; set; }
        public DateTime? BackgroundImageLastRefreshDate { get; set; }
        public TimeSpan RefreshPeriod { get; set; } = TimeSpan.FromDays(1);
    }
}