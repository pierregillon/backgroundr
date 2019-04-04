using System;
using System.Collections.Generic;
using System.Linq;
using backgroundr.application;
using backgroundr.view.windows.parameters;

namespace backgroundr.view.design
{
    public class ParametersDesignViewModel
    {
        public string UserId { get; set; } = "148722902@N09";
        public string Tags { get; set; } = "best top";
        public string OAuthAccessToken { get; set; } = "my-oauth-access-token";
        public string OAuthAccessTokenSecret { get; set; } = "xxxxxxxxaaaaaa";

        public FlickrPrivateAccess PrivateAccess { get; set; } = new FlickrPrivateAccess {
            UserName = "Pierre GILLON"
        };

        public bool AutomaticallyStart { get; set; } = true;

        public IList<RefreshPeriod> Periods { get; set; } = new List<RefreshPeriod> {
            new RefreshPeriod(TimeSpan.FromHours(1)),
            new RefreshPeriod(TimeSpan.FromHours(2)),
            new RefreshPeriod(TimeSpan.FromHours(4))
        };

        public RefreshPeriod SelectedPeriod
        {
            get { return Periods.First(); }
            set { }
        }
    }
}