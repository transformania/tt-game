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

    [AppSettings(Key = "Features.Misc.AprilFools2016")]
    public class AprilFools2016 : BaseFeature { }

    [AppSettings(Key = "Features.EnableMotorcycleBoss")]
    public class EnableMotorcycleBoss : BaseFeature { }
}