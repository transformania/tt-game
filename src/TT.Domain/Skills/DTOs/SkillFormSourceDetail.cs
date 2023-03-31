using TT.Domain.Effects.DTOs;
using TT.Domain.Forms.DTOs;

namespace TT.Domain.Skills.DTOs
{
    public class SkillSourceFormSourceDetail
    {
        public int Id { get; set; }
        public bool IsArchived { get; set; }
        public bool Bookmarked { get; set; }
        public FormSourceNameDetail SkillSource { get; set; }
    }

    public class FormSourceNameDetail
    {
        public int Id { get; set; }
        public string FriendlyName { get; set; }
        public string Description { get; set; }
        public EffectSourceDetail GivesEffectSource { get; set; }
        public FormNameDescriptionDetail FormSource { get; set; }
        public string MobilityType { get; set; }
    }

    public class LearnableSkillsDetail
    {
        public int Id { get; set; }
        public string FriendlyName { get; set; }
        public string MobilityType { get; set; }
        public FormNameDescriptionDetail FormSource { get; set; }

    }
}
