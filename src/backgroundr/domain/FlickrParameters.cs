using System;

namespace backgroundr.domain
{
    public class FlickrParameters
    {
        public string UserId { get; set; }
        public string Tags { get; set; }
        public DateTime? BackgroundImageLastRefreshDate { get; set; }
        public TimeSpan RefreshPeriod { get; set; } = TimeSpan.FromDays(1);
        public FlickrPrivateAccess PrivateAccess { get; set; }
    }
}