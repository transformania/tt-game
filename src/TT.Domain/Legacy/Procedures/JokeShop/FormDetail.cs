namespace TT.Domain.Legacy.Procedures.JokeShop
{
    public class FormDetail
    {
        public int FormSourceId { get; }
        public string FriendlyName { get; }
        public string Category { get; }

        public FormDetail(int formSourceId, string friendlyName, string category)
        {
            FormSourceId = formSourceId;
            FriendlyName = friendlyName;
            Category = category;
        }
    }

}
