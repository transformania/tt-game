
using TT.Domain.Forms.Entities;

namespace TT.Tests.Builders.Form
{
    public class FormSourceBuilder : Builder<FormSource, int>
    {
        public FormSourceBuilder()
        {
            Instance = Create();
            With(f => f.Id, 7);
            With(f => f.FriendlyName, "Regular Form!");
        }
    }
}
