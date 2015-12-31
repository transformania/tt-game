using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.ViewModels
{
    public class FairyChallengeViewModel
    {
        public List<FairyChallengeBag> FairyChallengeBags { get; set; }
    }

    public class FairyChallengeBag
    {
        public string dbLocationName { get; set; }
        public string IntroText { get; set; }
        public string CorrectFormText { get; set; }
        public string FailureText { get; set; }
        public string RequiredForm { get; set; }
    }
}