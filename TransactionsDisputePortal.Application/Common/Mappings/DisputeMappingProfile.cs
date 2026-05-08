using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using TransactionsDisputePortal.Application.Common.Models;
using TransactionsDisputePortal.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Application.Common.Mappings
{
    /// <summary>
    /// AutoMapper profile for Dispute entity mappings
    /// </summary>
    public class DisputeMappingProfile : Profile
    {
        public DisputeMappingProfile()
        {
            CreateMap<Dispute, DisputeDto>()
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason.ToString()))
                .ForMember(dest => dest.ReasonDescription, opt => opt.MapFrom(src => GetReasonDescription(src.Reason)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.StatusDescription, opt => opt.MapFrom(src => GetStatusDescription(src.Status)))
                .ForMember(dest => dest.Transaction, opt => opt.MapFrom(src => src.Transaction));

            CreateMap<Dispute, DisputeDetailDto>()
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason.ToString()))
                .ForMember(dest => dest.ReasonDescription, opt => opt.MapFrom(src => GetReasonDescription(src.Reason)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.StatusDescription, opt => opt.MapFrom(src => GetStatusDescription(src.Status)))
                .ForMember(dest => dest.Transaction, opt => opt.MapFrom(src => src.Transaction));
        }

        private static string GetReasonDescription(DisputeReason reason)
        {
            return reason switch
            {
                DisputeReason.UnauthorizedTransaction => "Unauthorized Transaction",
                DisputeReason.IncorrectAmount => "Incorrect Amount",
                DisputeReason.DuplicateCharge => "Duplicate Charge",
                DisputeReason.ProductNotReceived => "Product Not Received",
                DisputeReason.ProductDefective => "Product Defective",
                DisputeReason.ServiceNotProvided => "Service Not Provided",
                DisputeReason.Fraudulent => "Fraudulent",
                DisputeReason.Other => "Other",
                _ => reason.ToString()
            };
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
