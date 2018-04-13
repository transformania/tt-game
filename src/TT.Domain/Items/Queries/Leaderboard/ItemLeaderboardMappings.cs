using AutoMapper;
using TT.Domain.Extensions;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Entities;
using TT.Domain.Items.Queries.Leaderboard.DTOs;
using TT.Domain.Players.DTOs;
using TT.Domain.Players.Entities;

namespace TT.Domain.Items.Queries.Leaderboard
{
    public class ItemLeaderboardMappings : Profile
    {
        public ItemLeaderboardMappings()
        {
            CreateMissingTypeMaps = false;

            // Query projection
            CreateMap<Player, ItemLeaderboardPlayerDetail>();
            CreateMap<Item, ItemLeaderboardItemDetail>();
            CreateMap<ItemSource, ItemLeaderboardItemSourceDetail>();
            CreateMap<InanimateXP, ItemLeaderboardInanimateXPDetail>();
            CreateMap<Item, ItemLeaderboardDetail>()
                .MapForMember(dst => dst.FormerPlayer, src => src.FormerPlayer)
                .MapForMember(dst => dst.Item, src => src)
                .MapForMember(dst => dst.ItemSource, src => src.ItemSource)
                .MapForMember(dst => dst.ItemXP, src => src.FormerPlayer.ItemXP);

            // Post query proccessing
            CreateMap<ItemLeaderboardDetail, ItemLeaderboardDetail>()
                .ConstructUsing((src, context) => new ItemLeaderboardDetail(context.Mapper));

            // Legacy mapping
            CreateMap<ItemLeaderboardItemSourceDetail, ItemSourceDetail>(MemberList.Source);
            CreateMap<ItemLeaderboardInanimateXPDetail, InanimateXPDetail>(MemberList.Source);
            CreateMap<ItemLeaderboardPlayerDetail, PlayerDetail>(MemberList.Source);
            CreateMap<ItemLeaderboardItemDetail, ItemFormerPlayerDetail>(MemberList.Source);
            CreateMap<ItemLeaderboardDetail, ItemFormerPlayerDetail>(MemberList.None)
                .ResolveForTopLevel(src => src.Item)
                .MapForPath(dst => dst.FormerPlayer.ItemXP, src => src.ItemXP)
                .MapForMember(dst => dst.FormerPlayer, src => src.FormerPlayer)
                .MapForMember(dst => dst.ItemSource, src => src.ItemSource);
        }
    }
}
