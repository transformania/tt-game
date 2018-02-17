using AutoMapper;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Entities;

namespace TT.Domain.Items.Mappings
{
    public class InanimateXPMappings : Profile
    {
        public InanimateXPMappings()
        {
            CreateMap<InanimateXP, InanimateXPDetail>();
        }
    }
}