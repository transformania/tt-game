using FeatureSwitch;
using FeatureSwitch.Strategies;

namespace TT.Web
{
    [AppSettings(Key = "Features.Chat.V2")]
    public class ChatOverhaul : BaseFeature {}
}