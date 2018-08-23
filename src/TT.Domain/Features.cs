using FeatureSwitch;
using FeatureSwitch.Strategies;

namespace TT.Domain
{
    [AppSettings(Key = "Features.Chat.V2")]
    public class ChatV2 : BaseFeature {}

    [AppSettings(Key = "Features.UseCaptcha")]
    public class UseCaptcha : BaseFeature { }

    [AppSettings(Key = "Features.UseCloudflare")]
    public class UseCloudflare : BaseFeature { }
}