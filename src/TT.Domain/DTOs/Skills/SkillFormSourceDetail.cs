using TT.Domain.DTOs.Forms;

namespace TT.Domain.DTOs.Skills
{
    public class SkillSourceFormSourceDetail
    {
        public int Id { get; set; }

        public FormSourceNameDetail SkillSource { get; set; }
    }

    public class FormSourceNameDetail
    {
        public int Id { get; set; }
        public string dbName { get; set; }
        public string FriendlyName { get; set; }
        public FormNameDescriptionDetail FormSource { get; set; }
        public string MobilityType { get; set; }
    }

    public class LearnableSkillsDetail
    {
        public int Id { get; set; }
        public string dbName { get; set; }
        public string FriendlyName { get; set; }
        public string MobilityType { get; set; }
        public FormNameDescriptionDetail FormSource { get; set; }

    }
}
