using TT.Domain.Models;

namespace TT.Domain.ViewModels.NPCs
{
    public class JewdewfaeEncounterViewModel
    {
        public JewdewfaeEncounter JewdewfaeEncounter { get; set; }
        public bool IsInWrongForm { get; set; }
        public bool IsTired { get; set; }
        public bool ShowSuccess { get; set; }
        public bool HadRecentInteraction { get; set; }
        public decimal XPGain { get; set; }
        public string SpellsLearned { get; set; }
    }
}
