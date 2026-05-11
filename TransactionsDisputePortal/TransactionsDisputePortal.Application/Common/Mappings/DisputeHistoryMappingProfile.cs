using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using TransactionsDisputePortal.Application.Common.Models;
using TransactionsDisputePortal.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Application.Common.Mappings
{
    /// <summary>
    /// AutoMapper profile for DisputeHistory entity mappings
    /// </summary>
    public class DisputeHistoryMappingProfile : Profile
    {
        public DisputeHistoryMappingProfile()
        {
            CreateMap<DisputeHistory, DisputeHistoryDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.StatusDescription, opt => opt.MapFrom(src => GetStatusDescription(src.Status)));
        }

        private static string GetStatusDescription(DisputeStatus status)
        {
            return status switch
            {
                DisputeStatus.Pending => "Pending",
                DisputeStatus.UnderReview => "Under Review",
                DisputeStatus.Approved => "Approved",
                DisputeStatus.Rejected => "Rejected",
                DisputeStatus.Cancelled => "Cancelled",
                _ => status.ToString()
            };
        }
    }
}
