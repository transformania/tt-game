using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IPlayerRepository
    {

        IQueryable<Player> Players { get; }

        IQueryable<DbStaticForm> DbStaticForms { get; }

        void SavePlayer(Player Player);

        void DeletePlayer(int PlayerId);

        void SaveDbStaticForm(DbStaticForm DbStaticForm);

        void DeleteDbStaticForm(int DbStaticFormId);

    }
}