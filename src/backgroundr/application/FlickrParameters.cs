using System;

namespace backgroundr.application
{
    public class FlickrParameters
    {
        public string UserId { get; set; }
        public string Tags { get; set; }
        public DateTime? BackgroundImageLastRefreshDate { get; set; }
        public TimeSpan RefreshPeriod { get; set; } = TimeSpan.FromDays(1);
        public FlickrPrivateAccess PrivateAccess { get; set; }
    }

    public class FlickrPrivateAccess
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string OAuthAccessToken { get; set; }
        public string OAuthAccessTokenSecret { get; set; }
    }
}