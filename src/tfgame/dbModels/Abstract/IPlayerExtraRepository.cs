using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IPlayerExtraRepository
    {

        IQueryable<PlayerExtra> PlayerExtras { get; }

        void SavePlayerExtra(PlayerExtra PlayerExtra);

        void DeletePlayerExtra(int PlayerExtraId);

    }
}