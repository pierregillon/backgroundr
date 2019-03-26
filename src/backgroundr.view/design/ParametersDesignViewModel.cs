using System;
using System.Collections.Generic;
using System.Linq;
using backgroundr.view.viewmodels;

namespace backgroundr.view.design
{
    public class ParametersDesignViewModel
    {
        public string UserId { get; set; } = "148722902@N09";
        public string Tags { get; set; } = "best top";
        public string Token { get; set; } = "my-token-abcdef1234";
        public string TokenSecret { get; set; } = "xxxxxxxxxxqfqsdf";
        public string OAuthAccessToken { get; set; } = "my-oauth-access-token";
        public string OAuthAccessTokenSecret { get; set; } = "xxxxxxxxaaaaaa";
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
