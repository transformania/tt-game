using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IDbStaticFormRepository
    {

        IQueryable<DbStaticForm> DbStaticForms { get; }

        void SaveDbStaticForm(DbStaticForm DbStaticForm);

        void DeleteDbStaticForm(int DbStaticFormId);

    }
}