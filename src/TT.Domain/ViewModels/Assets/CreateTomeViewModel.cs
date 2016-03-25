
using System;
using TT.Domain.DTOs.Assets;
using TT.Domain.DTOs.Item;
using TT.Domain.Entities.Item;

namespace TT.Domain.ViewModels.Assets
{
    public class CreateTomeViewModel
    {
        public TomeDetail TomeDetail { get; private set; }

        public CreateTomeViewModel()
        {
            TomeDetail = new TomeDetail
            {
                Text = string.Concat("<span class=\"booktitle\">TITLE</span><br><br>", Environment.NewLine, Environment.NewLine, "<span class=\"bookauthor\">By AUTHOR</span><br><br>"),
                BaseItem = new ItemSourceDetail()
            };
        }

       

    }
}
