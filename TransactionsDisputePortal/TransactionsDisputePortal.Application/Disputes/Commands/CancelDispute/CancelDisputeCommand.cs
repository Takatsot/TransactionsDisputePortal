using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TransactionsDisputePortal.Application.Common.Exceptions;
using TransactionsDisputePortal.Application.Common.Interfaces;
using TransactionsDisputePortal.Application.Common.Models;
using TransactionsDisputePortal.Domain.Common.Interfaces;
using TransactionsDisputePortal.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Application.Disputes.Commands.CancelDispute
{
    /// <summary>
    /// Command to cancel an existing dispute
    /// </summary>
    public class CancelDisputeCommand : IRequest<DisputeDto>
    {
        public Guid DisputeId { get; set; }
        public Guid CustomerId { get; set; }
        public string? Reason { get; set; }
    }

    public class CancelDisputeCommandHandler : IRequestHandler<CancelDisputeCommand, DisputeDto>
    {
        private readonly IDisputeRepository _disputeRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public CancelDisputeCommandHandler(
            IDisputeRepository disputeRepository,
            ITransactionRepository transactionRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _disputeRepository = disputeRepository ?? throw new ArgumentNullException(nameof(disputeRepository));
            _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        }

        public async Task<DisputeDto> Handle(CancelDisputeCommand request, CancellationToken cancellationToken)
        {
            // Find the dispute (EF Core will track it automatically)
            var dispute = await _disputeRepository.FindAsync(
                d => d.Id == request.DisputeId && d.CustomerId == request.CustomerId,
                cancellationToken);

            if (dispute == null)
            {
                throw new NotFoundException(nameof(Domain.Entities.Dispute), request.DisputeId);
            }

            // Get current user
            var currentUser = await _currentUserService.GetAsync();
            var currentUserId = currentUser?.Id ?? request.CustomerId.ToString();
            
            // Load and restore transaction if it's disputed
            var transaction = await _transactionRepository.FindAsync(
                t => t.Id == dispute.TransactionId,
                cancellationToken);
            
            if (transaction != null && transaction.Status == Domain.Entities.TransactionStatus.Disputed)
            {
                transaction.RestoreToCompleted(currentUserId);
            }
            
            // Cancel the dispute (history entry disabled to avoid EF tracking issues)
            dispute.Cancel(
                request.Reason ?? "Cancelled by customer",
                currentUserId);

            // Save all changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Return the updated dispute
            return _mapper.Map<DisputeDto>(dispute);
        }
    }
}
