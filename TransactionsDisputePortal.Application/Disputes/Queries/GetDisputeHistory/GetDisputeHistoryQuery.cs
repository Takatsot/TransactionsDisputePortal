using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TransactionsDisputePortal.Application.Common.Exceptions;
using TransactionsDisputePortal.Application.Common.Models;
using TransactionsDisputePortal.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Application.Disputes.Queries.GetDisputeHistory
{
    /// <summary>
    /// Query to get the history of a dispute
    /// </summary>
    public class GetDisputeHistoryQuery : IRequest<List<DisputeHistoryDto>>
    {
        public Guid DisputeId { get; set; }
        public Guid CustomerId { get; set; }
    }

    public class GetDisputeHistoryQueryHandler : IRequestHandler<GetDisputeHistoryQuery, List<DisputeHistoryDto>>
    {
        private readonly IDisputeRepository _disputeRepository;
        private readonly IDisputeHistoryRepository _disputeHistoryRepository;
        private readonly IMapper _mapper;

        public GetDisputeHistoryQueryHandler(
            IDisputeRepository disputeRepository,
            IDisputeHistoryRepository disputeHistoryRepository,
            IMapper mapper)
        {
            _disputeRepository = disputeRepository ?? throw new ArgumentNullException(nameof(disputeRepository));
            _disputeHistoryRepository = disputeHistoryRepository ?? throw new ArgumentNullException(nameof(disputeHistoryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<DisputeHistoryDto>> Handle(GetDisputeHistoryQuery request, CancellationToken cancellationToken)
        {
            // Verify dispute exists and belongs to customer
            var dispute = await _disputeRepository.FindAsync(
                d => d.Id == request.DisputeId && d.CustomerId == request.CustomerId,
                cancellationToken);

            if (dispute == null)
            {
                throw new NotFoundException(nameof(Domain.Entities.Dispute), request.DisputeId);
            }

            // Get history ordered by date
            var history = await _disputeHistoryRepository.FindAllAsync(
                h => h.DisputeId == request.DisputeId,
                q => q.OrderBy(h => h.ChangedDate),
                cancellationToken);

            return _mapper.Map<List<DisputeHistoryDto>>(history);
        }
    }
}
