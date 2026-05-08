using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TransactionsDisputePortal.Application.Common.Exceptions;
using TransactionsDisputePortal.Application.Common.Models;
using TransactionsDisputePortal.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Application.Disputes.Queries.GetDisputeById
{
    /// <summary>
    /// Query to get a single dispute by ID with full details
    /// </summary>
    public class GetDisputeByIdQuery : IRequest<DisputeDetailDto>
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
    }

    public class GetDisputeByIdQueryHandler : IRequestHandler<GetDisputeByIdQuery, DisputeDetailDto>
    {
        private readonly IDisputeRepository _disputeRepository;
        private readonly IMapper _mapper;

        public GetDisputeByIdQueryHandler(IDisputeRepository disputeRepository, IMapper mapper)
        {
            _disputeRepository = disputeRepository ?? throw new ArgumentNullException(nameof(disputeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<DisputeDetailDto> Handle(GetDisputeByIdQuery request, CancellationToken cancellationToken)
        {
            var dispute = await _disputeRepository.FindByIdWithDetailsAsync(request.Id, cancellationToken);

            if (dispute == null || dispute.CustomerId != request.CustomerId)
            {
                throw new NotFoundException(nameof(Domain.Entities.Dispute), request.Id);
            }

            return _mapper.Map<DisputeDetailDto>(dispute);
        }
    }
}
