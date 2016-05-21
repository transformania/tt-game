
namespace TT.Tests.Builders.Form
{
    public class FormSourceBuilder : Builder<TT.Domain.Entities.Forms.FormSource, int>
    {
        public FormSourceBuilder()
        {
            Instance = Create();
            With(f => f.Id, 7);
            With(f => f.FriendlyName, "Regular Form!");
        }
    }
}
