using System.Collections.Generic;
using System.Web.Configuration;

namespace TT.Web.Services
{
    public interface IFeatureService
    {
        bool IsFeatureEnabled(string feature);
    }

    public class FeatureService : IFeatureService
    {
        private Dictionary<string, bool> Flags { get; set; }

        public FeatureService()
        {
            var keys = new [] { Features.ChatV2, Features.UseCaptcha };
            Flags = new Dictionary<string, bool>();

            foreach (var key in keys)
            {
                Flags.Add(key, bool.Parse(WebConfigurationManager.AppSettings[key]));
            }
        }

        public bool IsFeatureEnabled(string feature)
        {
            return Flags.TryGetValue(feature, out var flag) && flag;
        }
    }

    public static class Features
    {
        public const string ChatV2 = "Features.Chat.V2";
        public const string UseCaptcha = "Features.UseCaptcha";
    }
}