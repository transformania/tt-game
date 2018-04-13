using AutoMapper;
using TT.Domain.Items.DTOs;

namespace TT.Domain.Items.Queries.Leaderboard.DTOs
{
    public class ItemLeaderboardDetail
    {
        public ItemLeaderboardDetail()
        {
        }

        internal ItemLeaderboardDetail(IMapper mapper)
        {
            castMapper = mapper;
        }

        public ItemLeaderboardItemSourceDetail ItemSource { get; set; }

        public ItemLeaderboardInanimateXPDetail ItemXP { get; set; }

        public ItemLeaderboardPlayerDetail FormerPlayer { get; set; }

        public ItemLeaderboardItemDetail Item { get; set; }

        private readonly IMapper castMapper;
        public static explicit operator ItemFormerPlayerDetail(ItemLeaderboardDetail compositeDetail)
        {
            return compositeDetail.castMapper.Map<ItemLeaderboardDetail, ItemFormerPlayerDetail>(compositeDetail);
        }
    }
}
