using TT.Domain.DTOs.Item;

namespace TT.Domain.DTOs.Forms
{
    public class FormNameDescriptionDetail
    {
        public string FriendlyName { get; set; }
        public string Description { get; set; }
        public string PortraitUrl { get; set; }

        public ItemSourceNameDescription ItemSource { get; set; }
    }

}
