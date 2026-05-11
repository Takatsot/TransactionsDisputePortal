using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using System.Linq;
using TransactionsDisputePortal.Application.Common.Models;
using TransactionsDisputePortal.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Application.Common.Mappings
{
    /// <summary>
    /// AutoMapper profile for Transaction entity mappings
    /// </summary>
    public class TransactionMappingProfile : Profile
    {
        public TransactionMappingProfile()
        {
            CreateMap<Transaction, TransactionDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<Transaction, TransactionDetailDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Dispute, opt => opt.MapFrom(src => src.Disputes.FirstOrDefault(d => d.IsActive)));

            CreateMap<Transaction, TransactionSummaryDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
        }
    }
}
