namespace TT.Domain.Models
{
    public class JewdewfaeEncounter
    {
        public int Id { get; set; }
        public string dbLocationName { get; set; }
        public string IntroText { get; set; }
        public string CorrectFormText { get; set; }
        public string FailureText { get; set; }
        public int RequiredFormSourceId { get; set; }
        public bool IsLive { get; set; }
    }
}