using FeatureSwitch;
using FeatureSwitch.Strategies;

namespace TT.Domain
{
    [AppSettings(Key = "Features.Chat.V2")]
    public class ChatV2 : BaseFeature {}

    [AppSettings(Key = "Features.Misc.AprilFools2016")]
    public class AprilFools2016 : BaseFeature { }
}