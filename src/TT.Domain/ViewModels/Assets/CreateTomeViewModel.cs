
using System;

namespace TT.Domain.ViewModels.Assets
{
    public class CreateTomeViewModel
    {
        public string InitialText { get; set; }

        public CreateTomeViewModel()
        {
            InitialText = string.Concat("<span class=\"booktitle\">TITLE</span><br><br>", Environment.NewLine,
                Environment.NewLine, "<span class=\"bookauthor\">By AUTHOR</span><br><br>");
        }
    }
}
