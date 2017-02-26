using TT.Domain.Items.DTOs;

namespace TT.Domain.Forms.DTOs
{
    public class FormNameDescriptionDetail
    {
        public string FriendlyName { get; set; }
        public string Description { get; set; }
        public string PortraitUrl { get; set; }

        public ItemSourceNameDescription ItemSource { get; set; }
    }

}
