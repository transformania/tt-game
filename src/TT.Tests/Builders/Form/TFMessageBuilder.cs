using TT.Domain.Entities.Forms;

namespace TT.Tests.Builders.Form
{
    public class TFMessageBuilder : Builder<TFMessage, int>
    {
        public TFMessageBuilder()
        {
            Instance = Create();
            With(f => f.Id, 7);
        }
    }
}
