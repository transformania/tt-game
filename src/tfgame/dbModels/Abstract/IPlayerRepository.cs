﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
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