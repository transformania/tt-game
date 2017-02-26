using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IDbStaticFormRepository
    {

        IQueryable<DbStaticForm> DbStaticForms { get; }

        void SaveDbStaticForm(DbStaticForm DbStaticForm);

        void DeleteDbStaticForm(int DbStaticFormId);

    }
}