using FeatureSwitch;
using FeatureSwitch.Strategies;

namespace TT.Web
{
    [AppSettings(Key = "Features.Chat.V2")]
    public class ChatV2 : BaseFeature {}
}