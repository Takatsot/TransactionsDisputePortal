using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using TransactionsDisputePortal.Application.Common.Models;
using TransactionsDisputePortal.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Application.Common.Mappings
{
    /// <summary>
    /// AutoMapper profile for Dispute entity mappings - descriptions from GetReasonDescription/GetStatusDescription
    /// Note: Actual mapping happens in Infrastructure layer with database lookups
    /// </summary>
    public class DisputeMappingProfile : Profile
    {
        public DisputeMappingProfile()
        {
            CreateMap<Dispute, DisputeDto>()
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason.ToString()))
                .ForMember(dest => dest.ReasonDescription, opt => opt.Ignore()) // Set by Infrastructure
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.StatusDescription, opt => opt.Ignore()) // Set by Infrastructure
                .ForMember(dest => dest.Transaction, opt => opt.MapFrom(src => src.Transaction));

            CreateMap<Dispute, DisputeDetailDto>()
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason.ToString()))
                .ForMember(dest => dest.ReasonDescription, opt => opt.Ignore()) // Set by Infrastructure
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.StatusDescription, opt => opt.Ignore()) // Set by Infrastructure
                .ForMember(dest => dest.Transaction, opt => opt.MapFrom(src => src.Transaction));
        }
    }
}
