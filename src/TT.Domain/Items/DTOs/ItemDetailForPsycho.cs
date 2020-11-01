namespace TT.Domain.Items.DTOs
{
    public class ItemDetailForPsycho
    {
        public int Id { get;  set; }
        public ItemSourceForPsycho ItemSource { get;  set; }
        public PlayerForPsycho Owner { get;  set; }
        public PlayerForPsycho FormerPlayer { get;  set; }
    }

    public class ItemSourceForPsycho
    {
        public string ItemType { get; set; }
        public string FriendlyName { get; set; }
    }

    public class PlayerForPsycho
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nickname { get; set; }
        public int DonatorLevel { get; set; }
        public string FullName
        {
            get
            {
                return DonatorLevel < 2 || string.IsNullOrEmpty(Nickname) ?
                    $"{FirstName} {LastName}" :
                    $"{FirstName} '{Nickname}' {LastName}";
            }
        }
    }
}